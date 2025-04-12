using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Hosting;
using IMitto.Middlware;

namespace IMitto.Net.Server;

public sealed class MittoServer : MittoHost<MittoServerOptions>, IMittoServer
{
	private Task? _eventManagerTask = null;
	private readonly ILogger<MittoServer> _logger;
	private readonly IMittoRequestHandler _requestHandler;
	private readonly IServerEventManager _eventManager;

	public MittoServer(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServer> logger,
		IMittoRequestHandler requestHandler,
		IServerEventManager serverEvents) : base(logger, options)
	{
		_logger = logger;
		_requestHandler = requestHandler;
		_eventManager = serverEvents;
	}

	private Task EventManagerTask => _eventManagerTask ?? Task.CompletedTask;

	public string Name => Options.Name;

	public IMittoServerConnection? Connection { get; private set; }

	private static Task<MittoSocket> AcceptRequest(IMittoServerConnection connection, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return connection.AcceptAsync(token);
	}

	protected override Task RunInternalAsync(CancellationToken token = default)
	{
		_logger.LogTrace("Run Server: start");

		_eventManagerTask = _eventManager.RunAsync(token);
		IMiddlewareHandler<ConnectionContext> middleware = CreateMiddlewarePipeline();

		return Task.Run(async () => {
			var retryCount = 0;

			_logger.LogTrace("Initializing connection: start");
			Connection = MittoConnection.CreateServer(Options);
			_logger.LogTrace("Initializing connection: end");

			while (!token.IsCancellationRequested && Options.MaxConnectionRetries > retryCount++)
			{
				try
				{
					while (!token.IsCancellationRequested)
					{
						_logger.LogTrace("Accepting Connections: start");

						if (!Connection.IsConnected())
						{
							await Connection.ConnectAsync(TokenSource.Token);
						}

						var socket = await AcceptRequest(Connection, TokenSource.Token);

						_logger.LogTrace("Accepting Connections: end;");

						var connectionContext = new ConnectionContext(
							_eventManager,
							socket,
							TokenSource.Token);

						_logger.LogTrace("Initializing client/server workflow: start {connectionId}", connectionContext.ConnectionId);

						connectionContext.BackgroundTask = Task.Run(async () => {
							var context = new MiddlewareContext<ConnectionContext>(connectionContext);
							await middleware.HandleAsync(context, token).ConfigureAwait(false);

						}, TokenSource.Token);
					}
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

	private IMiddlewareHandler<ConnectionContext> CreateMiddlewarePipeline()
	{
		return new MiddlewareBuilder<ConnectionContext>()
			.Add(async (next, context, ct) => {
				_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);
				await _requestHandler.HandleAuthenticationAsync(context.State, ct).ConfigureAwait(false);
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);

				await next(context, ct).ConfigureAwait(false);
			})
			.Add(async (next, context, ct) => {
				_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);
				await _requestHandler.HandleAuthorizationAsync(context.State, ct).ConfigureAwait(false);
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);

				await next(context, ct).ConfigureAwait(false);
			})
			.Build();
	}

	public sealed override async Task StopAsync(CancellationToken? token = null)
	{
		using var cts = TokenSource ?? CancellationTokenSource.CreateLinkedTokenSource(token ?? default);

		await Task.WhenAll(
			StopBackgroundTask(EventManagerTask, cts.Token),
			StopBackgroundTask(StartedTask, cts.Token)
		);

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
