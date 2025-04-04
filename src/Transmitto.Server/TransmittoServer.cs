using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transmitto.Net;
using Transmitto.Net.Server;

namespace Transmitto.Server;

public sealed class TransmittoServer : Disposable, ITransmittoServer
{
	private Task? _startedTask = null;
	private Task? _eventManagerTask = null;
	private readonly ILogger<TransmittoServer> _logger;
	private readonly ITransmittoRequestHandler _requestHandler;
	private readonly IServerEventManager _eventManager;

	public TransmittoServer(
		IOptions<TransmittoServerOptions> options,
		ILogger<TransmittoServer> logger,
		ITransmittoRequestHandler requestHandler,
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

	public TransmittoServerOptions Options { get; }

	public string Name => Options.Name;

	public ITransmittoServerConnection? Connection { get; private set; }

	private static Task<TransmittoSocket> AcceptRequest(ITransmittoServerConnection connection, CancellationToken token)
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
			Connection = TransmittoConnection.CreateServer(Options);
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

						var eventListener = _requestHandler.GetEventListener();

						var context = new ConnectionContext(
							_eventManager,
							socket,
							eventListener,
							TokenSource.Token);

						_logger.LogTrace("Connection Accepted: start {connectionId}", context.ConnectionId);
						
						void HandleBackgroundTask()
						{
							_requestHandler.HandleRequestAsync(context, token);
						}

						context.BackgroundTask = new Task(HandleBackgroundTask, TokenSource.Token);

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
