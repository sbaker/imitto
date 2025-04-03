using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transmitto.Net;
using Transmitto.Net.Server;

namespace Transmitto.Server;

public sealed class TransmittoServer : ITransmittoServer
{
	private Task? _startedTask = null;
	private readonly ILogger<TransmittoServer> _logger;
	private readonly ITransmittoRequestHandler _requestHandler;
	private readonly IEventAggregator _eventAggregator;

	public TransmittoServer(
		IOptions<TransmittoServerOptions> options,
		ILogger<TransmittoServer> logger,
		ITransmittoRequestHandler requestHandler,
		IServerEventAggregator serverEvents)
	{
		Options = options.Value;
		_logger = logger;
		_requestHandler = requestHandler;
		_eventAggregator = serverEvents;
	}

	private CancellationTokenSource? TokenSource { get; set; }

	private Task StartedTask => _startedTask ?? Task.CompletedTask;

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

						var context = new ConnectionContext(_eventAggregator, socket);
						
						void task()
						{
							_requestHandler.HandleRequestAsync(context, token);
						}

						context.Task = new Task(task, TokenSource.Token);

						context.Task.Start();
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

		if (!StartedTask.IsCompleted)
		{
			try
			{
				cts.CancelAfter(TimeSpan.FromSeconds(5));
				await StartedTask.WaitAsync(cts.Token);
			}
			catch (TaskCanceledException)
			{
			}
		}
	}
}
