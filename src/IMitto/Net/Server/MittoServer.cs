using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Hosting;
using IMitto.Middlware;

namespace IMitto.Net.Server;

public sealed class MittoServer : MittoHost<MittoServerOptions>, IMittoServer
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

		var middleware = new MiddlewareBuilder<ConnectionContext>()
			.Add(async (next, context, ct) =>
			{
				_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);
				await _requestHandler.HandleRequestAsync(context.State, ct).ConfigureAwait(false);
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);

				await next(context, ct).ConfigureAwait(false);
			})
			.Build();

		return Task.Run(async () =>
		{
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

						var connectionState = new ConnectionContext(
							_eventManager,
							socket,
							TokenSource.Token);

						_logger.LogTrace("Initializing client/server workflow: start {connectionId}", connectionState.ConnectionId);

						connectionState.BackgroundTask = Task.Run(async () => {
							var context = new MiddlewareContext<ConnectionContext>(connectionState);
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


//using IMitto.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace IMitto.Net.Server;

//public sealed class MittoServer : MittoHost, IMittoServer
//{
//	private readonly ILogger<MittoServer> _logger;
//	private readonly IMittoRequestHandler _requestHandler;
//	private readonly IServerEventManager _eventManager;

//	public MittoServer(
//		ILogger<MittoServer> logger,
//		IOptions<MittoServerOptions> options,
//		IMittoRequestHandler requestHandler,
//		IServerEventManager serverEvents) : base(logger)
//	{
//		_logger = logger;
//		_requestHandler = requestHandler;
//		_eventManager = serverEvents;

//		Options = options.Value;
//	}

//	public bool Running => StartedTasks.Any(t => !t.IsCompleted);

//	public MittoServerOptions Options { get; }

//	public string Name => Options.Name;

//	public IMittoServerConnection? Connection { get; private set; }

//	private static Task<MittoSocket> AcceptRequest(IMittoServerConnection connection, CancellationToken token)
//	{
//		token.ThrowIfCancellationRequested();
//		return connection.AcceptAsync(token);
//	}

//	protected override Task RunInternalAsync(CancellationToken token = default)
//	{
//		StartBackgroundTask(_eventManager.RunAsync, TokenSource.Token);

//		var runningTask = Task.Run(async () => {
//			var retryCount = 0;
//			TokenSource ??= Add(CancellationTokenSource.CreateLinkedTokenSource(token));

//			_logger.LogTrace("Initializing connection: start");
//			Connection = MittoConnection.CreateServer(Options);
//			_logger.LogTrace("Initializing connection: end");

//			while (!token.IsCancellationRequested && Options.MaxConnectionRetries > retryCount++)
//			{
//				try
//				{
//					_logger.LogTrace("Accepting Connections: start");

//					while (!token.IsCancellationRequested)
//					{
//						if (!Connection.IsConnected())
//						{
//							await Connection.ConnectAsync(TokenSource.Token);
//						}

//						var socket = await AcceptRequest(Connection, TokenSource.Token);

//						var context = new ConnectionContext(
//							_eventManager,
//							socket,
//							TokenSource.Token);

//						_logger.LogTrace("Connection Accepted: start {connectionId}", context.ConnectionId);

//						context.BackgroundTask = Task.Run(async () => {//.Factory.StartNew(async () => {
//							await _requestHandler.HandleRequestAsync(context, token).ConfigureAwait(false);
//						}, TokenSource.Token);

//						context.BackgroundTask.Start();
//					}

//					_logger.LogTrace("Accepting Connections: end; CancellationToken requested cancel");
//				}
//				catch (TaskCanceledException tce)
//				{
//					_logger.LogWarning(tce, "Task canceled");
//					break;
//				}
//				catch (Exception e)
//				{
//					_logger.LogError(e, "Server failed to accept new connection: unknown error.");
//				}
//			}

//			_logger.LogTrace("Run Server: end");

//			Connection?.Dispose();

//		}, token);

//		return runningTask;
//	}

//	protected override void DisposeCore()
//	{
//		if (Running)
//		{
//			StopAsync().Wait();
//		}

//		TokenSource?.Dispose();
//		Connection?.Dispose();
//	}
//}

