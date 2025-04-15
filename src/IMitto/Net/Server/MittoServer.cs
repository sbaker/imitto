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

	private IMittoServerConnection? Connection { get; set; }

	private static Task<MittoSocket> AcceptSocket(IMittoServerConnection connection, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		return connection.AcceptAsync(token);
	}

	protected override Task RunInternalAsync(CancellationToken token = default)
	{
		_logger.LogTrace("Run Server: start");

		_eventManagerTask = _eventManager.RunAsync(token);

		var middleware = CreateMittoListenerPipeline();

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
							await Connection.ConnectAsync(TokenSource.Token).Await();
						}

						var socket = await AcceptSocket(Connection, TokenSource.Token).Await();

						_logger.LogTrace("Accepting Connections: end;");

						var connectionContext = new ConnectionContext(
							_eventManager,
							socket,
							TokenSource.Token);

						await _eventManager.PublishServerEventAsync(ServerEventConstants.ConnectionReceivedEvent, connectionContext, connectionContext.ConnectionId, token).Await();

						_logger.LogTrace("Initializing client/server workflow: start {connectionId}", connectionContext.ConnectionId);

						StartBackgroundTask(ct => middleware.HandleAsync(connectionContext, ct), token);
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

	private IMiddlewareHandler<ConnectionContext> CreateMittoListenerPipeline()
	{
		var innerHandler = CreateRequestPipeline();
		return new MiddlewareBuilder<ConnectionContext>()
			.Add(new MittoServerConnectionContextMiddlewareHandler(Options, _logger, innerHandler))
			.Build();
	}

	private IMiddlewareHandler<MittoConnectionContext> CreateRequestPipeline()
	{
		return new MiddlewareBuilder<MittoConnectionContext>()
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);
				if (context.Request.Header.Path == MittoPaths.Auth)
				{
					return _requestHandler.HandleAuthenticationAsync(context, ct);
				}

				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);
				if (context.Request.Header.Path == MittoPaths.Topics && context.Request.Header.Action == MittoEventType.Consume)
				{
					return _requestHandler.HandleAuthorizationAsync(context, ct);
				}
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Add((next, context, ct) => {
				_logger.LogTrace("Server Request Listener: start {connectionId}", context.ConnectionId);
				if (context.Request.Header.Path == MittoPaths.Topics && context.Request.Header.Action == MittoEventType.Produce)
				{
					return _requestHandler.HandleEventNotificationAsync(context, ct);
				}
				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
				return next(context, ct);
			})
			.Build();
	}

	public sealed override async Task StopAsync(CancellationToken? token = null)
	{
		using var cts = TokenSource ?? CancellationTokenSource.CreateLinkedTokenSource(token ?? default);

		await Task.WhenAll(
			StopBackgroundTask(EventManagerTask, cts.Token),
			StopBackgroundTask(StartedTask, cts.Token)
		).Await();

		async Task StopBackgroundTask(Task task, CancellationToken token)
		{
			if (!task.IsCompleted)
			{
				try
				{
					cts.CancelAfter(TimeSpan.FromSeconds(5));
					await task.WaitAsync(cts.Token).Await();
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
