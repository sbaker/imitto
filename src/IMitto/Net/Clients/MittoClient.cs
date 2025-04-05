using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public class MittoClient : IMittoClient
{
	private bool _disposedValue;
	private readonly MittoClientOptions _options;
	private readonly IMittoEventDispatcher _eventDispatcher;
	private readonly ILogger<MittoClient> _logger;

	public MittoClient(
		IOptions<MittoClientOptions> options,
		ILogger<MittoClient> logger,
		IMittoEventDispatcher eventDispatcher)
	{
		_logger = logger;
		_options = options.Value;
		_eventDispatcher = eventDispatcher;
	}

	protected CancellationTokenSource? TokenSource { get; set; }

	protected IMittoClientConnection? Connection { get; set; }

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				TokenSource?.Dispose();
				Connection?.Dispose();
			}

			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async Task RunAsync(CancellationToken token = default)
	{
		TokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

		await RunInternalAsync(TokenSource.Token);
		
		Connection?.Dispose();
	}

	protected Task RunInternalAsync(CancellationToken token)
	{
		return Task.Run(async () =>
		{
			var retries = 0;

			// Try {_options.MaxConnectionRetries} times to connect and authenticate.
			while (_options.MaxConnectionRetries > retries++)
			{
				try
				{
					Connection = MittoConnection.CreateClient(_options);

					if (!Connection.IsConnected())
					{
						await Connection.ConnectAsync();
					}

					var response = await Connection.AuthenticateAsync(token);

					if (response.Success)
					{
						await StartEventLoopAsync(Connection, response, token);
					}
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Task canceled");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unknown error.");
				}

				Connection?.Dispose();
			}
		}, token);
	}

	protected async Task StartEventLoopAsync(IMittoClientConnection connection, MittoStatus response, CancellationToken token)
	{
		var delayedTask = Task.Delay(20000, token).ContinueWith(task => {
			// TODO: TESTCODE: Add code to publish an event after 20s.
		}, token);

		try
		{
			while (!token.IsCancellationRequested)
			{
				var eventNotifications = await connection.WaitForEventsAsync(token);

				await _eventDispatcher.DispatchAsync(eventNotifications);
			}
		}
		catch (Exception e)
		{
			_logger.LogError(e, "An unexpected error.");
		}

		await delayedTask;
	}
}
