using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Net.Models;
using IMitto.Hosting;
using System.Net.Sockets;

namespace IMitto.Net.Clients;

public class MittoClient(
	IOptions<MittoClientOptions> options,
	ILogger<MittoClient> logger,
	IMittoEventDispatcher eventDispatcher) : MittoHost<MittoClientOptions>(logger, options), IMittoClient
{
	private readonly MittoClientOptions _options = options.Value;
	private readonly IMittoEventDispatcher _eventDispatcher = eventDispatcher;
	private readonly ILogger<MittoClient> _logger = logger;

	protected IMittoClientConnection? Connection { get; set; }

	protected override Task RunInternalAsync(CancellationToken token)
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

	protected Task StartEventLoopAsync(IMittoClientConnection connection, MittoStatus response, CancellationToken token)
	{
		ArgumentNullException.ThrowIfNull(response, nameof(response));

		return Task.Run(async () =>
		{
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
				switch (e)
				{
					case TaskCanceledException tce:
						_logger.LogWarning(tce, "Task canceled");
						break;
					case SocketException se:
						_logger.LogError(se, "Socket error: {error}", se.SocketErrorCode);
						break;
					case IOException ioe:
						_logger.LogError(ioe, "IO error: {error}", ioe.Message);
						break;
					default:
						_logger.LogError(e, "An unexpected error.");
						break;
				}

				throw;
			}
		}, token);
		
		//var delayedTask = Task.Delay(20000, token).ContinueWith(task => {
		//	// TODO: TESTCODE: Add code to publish an event after 20s.
		//}, token);
	}
}
