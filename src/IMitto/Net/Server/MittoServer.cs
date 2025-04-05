using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Net;

namespace IMitto.Net.Server;

public sealed class MittoServer : Disposable, IMittoServer
{
	private Task? _startedTask = null;
	private Task? _eventManagerTask = null;
	private readonly ILogger<MittoServer> _logger;
	private readonly IMittoRequestHandler _requestHandler;
	private readonly IServerEventManager _eventManager;

	public MittoServer(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServer> logger,
		IMittoRequestHandler requestHandler,
		IServerEventManager serverEvents)
	{
		Options = options.Value;
		_logger = logger;
		_requestHandler = requestHandler;
		_eventManager = serverEvents;
	}

	public bool Started => _startedTask != null && !_startedTask.IsCompleted;

	private CancellationTokenSource? TokenSource { get; set; }

	private Task StartedTask => _startedTask ?? Task.CompletedTask;

	private Task EventManagerTask => _eventManagerTask ?? Task.CompletedTask;

	public MittoServerOptions Options { get; }

	public string Name => Options.Name;

	public IMittoServerConnection? Connection { get; private set; }

	private static Task<MittoSocket> AcceptRequest(IMittoServerConnection connection, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return connection.AcceptAsync(token);
	}

	public Task RunAsync(CancellationToken token = default)
	{
		_logger.LogTrace("Run Server: start");

		_eventManagerTask = _eventManager.RunAsync(token);

		return Task.Run(async () =>
		{
			var retryCount = 0;
			TokenSource ??= CancellationTokenSource.CreateLinkedTokenSource(token);

			_logger.LogTrace("Initializing connection: start");
			Connection = MittoConnection.CreateServer(Options);
			_logger.LogTrace("Initializing connection: end");
			
			while (!token.IsCancellationRequested && Options.MaxConnectionRetries > retryCount++)
			{
				try
				{
					_logger.LogTrace("Accepting Connections: start");

					while (!token.IsCancellationRequested)
					{
						if (!Connection.IsConnected())
						{
							await Connection.ConnectAsync(TokenSource.Token);
						}

						var socket = await AcceptRequest(Connection, TokenSource.Token);

						var context = new ConnectionContext(
							_eventManager,
							socket,
							TokenSource.Token);

						_logger.LogTrace("Connection Accepted: start {connectionId}", context.ConnectionId);
						
						context.BackgroundTask = Task.Run(async () => {//.Factory.StartNew(async () => {
							await _requestHandler.HandleRequestAsync(context, token).ConfigureAwait(false);
						}, TokenSource.Token);

						context.BackgroundTask.Start();
					}

					_logger.LogTrace("Accepting Connections: end; CancellationToken requested cancel");
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Task canceled");
					break;
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Server failed to accept new connection: unknown error.");
				}
			}

			_logger.LogTrace("Run Server: end");

			Connection?.Dispose();

		}, token);
	}

	public Task StartAsync(CancellationToken? token = null)
	{
		TokenSource = token.HasValue
			? CancellationTokenSource.CreateLinkedTokenSource(token.Value)
			: new CancellationTokenSource();

		_startedTask = RunAsync(TokenSource.Token);

		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken token = default)
	{
		using var cts = TokenSource ?? CancellationTokenSource.CreateLinkedTokenSource(token);

		await Task.WhenAll(StopBackgroundTask(EventManagerTask, token), StopBackgroundTask(StartedTask, token));

		async Task StopBackgroundTask(Task task, CancellationToken token)
		{
			if (!task.IsCompleted)
			{
				try
				{
					cts.CancelAfter(TimeSpan.FromSeconds(5));
					await task.WaitAsync(cts.Token);
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Timeout waiting for task. Stop background task");
				}
			}
		}
	}

	protected override void DisposeCore()
	{
		StopAsync(TokenSource.Token).Wait();
		TokenSource?.Dispose();
		Connection?.Dispose();
	}
}
