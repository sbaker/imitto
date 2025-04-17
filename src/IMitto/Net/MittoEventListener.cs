using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Channels;
using IMitto.Settings;
using IMitto.Net.Server;
using IMitto.Protocols.Requests;

namespace IMitto.Net;

public class MittoEventListener : IMittoEventListener
{
	private readonly ILogger<MittoEventListener> _logger;
	private readonly MittoConnectionOptions _options;
	private readonly IMittoChannelWriterProvider<ServerEventNotificationsContext> _channelWriter;

	public MittoEventListener(
		ILogger<MittoEventListener> logger,
		IOptions<MittoConnectionOptions> options,
		IMittoChannelWriterProvider<ServerEventNotificationsContext> channelWriter)
	{
		_logger = logger;
		_options = options.Value;
		_channelWriter = channelWriter;
	}

	public async Task PollForEventsAsync(ConnectionContext context, CancellationToken token)
	{
		_logger.LogTrace("Listening for events: start {connectionId}", context.ConnectionId);

		while (!token.IsCancellationRequested)
		{
			await Task.Delay(_options.TaskDelayMilliseconds, token).Await();

			if (!context.Socket.DataAvailable) { continue; }

			try
			{
				var message = await context.Socket.ReadAsync<EventNotificationRequest>(token).Await();

				if (message == null) { continue; }

				_logger.LogTrace("Listening for events: received {connectionId}", context.ConnectionId);

				var writer = _channelWriter.GetWriter();

				if (await writer.WaitToWriteAsync(token).Await())
				{
					var eventContext = new ServerEventNotificationsContext(context.ConnectionId, message.Body.Content!);

					await writer.WriteAsync(eventContext, token).Await();
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error while polling for events: {connectionId}", context.ConnectionId);
			}
		}

		_logger.LogTrace("Listening for events: end {connectionId}", context.ConnectionId);
	}
}