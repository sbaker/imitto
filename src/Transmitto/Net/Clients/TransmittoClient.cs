using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transmitto.Channels;
using Transmitto.Net.Models;

namespace Transmitto.Net.Clients;

public class TransmittoClient : ITransmittoClient
{
	private bool _disposedValue;
	private readonly TransmittoClientOptions _options;
	private readonly ITransmittoEventDispatcher _eventDispatcher;
	private readonly ILogger<TransmittoClient> _logger;

	public TransmittoClient(
		IOptions<TransmittoClientOptions> options,
		ILogger<TransmittoClient> logger,
		ITransmittoEventDispatcher eventDispatcher)
	{
		_logger = logger;
		_options = options.Value;
		_eventDispatcher = eventDispatcher;
	}

	private CancellationTokenSource? TokenSource { get; set; }
	
	private ITransmittoClientConnection? Connection { get; set; }

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				Connection?.Dispose();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async Task RunAsync(CancellationToken token = default)
	{
		TokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

		await RunInternalAsync(TokenSource.Token);
		
		Connection?.Dispose();
	}

	private Task RunInternalAsync(CancellationToken token)
	{
		Subscribe.To<string>(0, ctx => _logger.LogInformation("Message received: {data}", ctx.Data));

		return Task.Run(async () =>
		{
			Connection = TransmittoConnection.CreateClient(_options);

			if (!Connection.IsConnected())
			{
				await Connection.ConnectAsync();
			}

			var response = await Connection.AuthenticateAsync(token);

			if (response.Success)
			{
				await StartEventLoopAsync(Connection, response, token);
			}
		}, token);
	}

	private async Task StartEventLoopAsync(ITransmittoClientConnection connection, TransmittoStatus response, CancellationToken token)
	{
		var delayedTask = Task.Delay(20000).ContinueWith(task => {
			// TODO: TESTCODE: Add code to publish an event after 20s.
		});

		while (!token.IsCancellationRequested)
		{
			try
			{
				var eventNotifications = await connection.WaitForEventsAsync(token);

				await _eventDispatcher.DispatchAsync(eventNotifications);
			}
			catch (Exception e)
			{
				_logger.LogError(e, "An un expected error.");
			}
		}

		await delayedTask;
	}
}

public interface ITransmittoEventDispatcher
{
	ValueTask DispatchAsync(EventNotificationsModel notifications);
}

public abstract class TransmittoEventDispatcher : ITransmittoEventDispatcher
{
	private readonly ILogger _logger;

	protected TransmittoEventDispatcher(ILogger logger)
	{
		_logger = logger;
	}

	public abstract ValueTask DispatchAsync(EventNotificationsModel notifications);
}

public class ChannelTransmittoEventDispatcher : TransmittoEventDispatcher
{
	private readonly ITransmittoChannelProvider<EventNotificationsModel> _queueProvider;

	public ChannelTransmittoEventDispatcher(
		ILogger<TransmittoEventDispatcher> logger,
		ITransmittoChannelProvider<EventNotificationsModel> queueProvider) : base(logger)
	{
		_queueProvider = queueProvider;
	}

	public override ValueTask DispatchAsync(EventNotificationsModel notifications)
	{
		return new ValueTask(Task.Run(() => {
			var channelWriter = _queueProvider.GetWriter();
			return channelWriter.WriteAsync(notifications);
		}));
	}
}