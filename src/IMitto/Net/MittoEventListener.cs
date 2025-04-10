using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Channels;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Settings;

namespace IMitto.Net;

public class MittoEventListener : IMittoEventListener
{
	private readonly ILogger<MittoEventListener> _logger;
	private readonly MittoConnectionOptions _options;
	private readonly IMittoChannelWriterProvider<EventNotificationsModel> _channelWriter;

	public MittoEventListener(
		ILogger<MittoEventListener> logger,
		IOptions<MittoConnectionOptions> options,
		IMittoChannelWriterProvider<EventNotificationsModel> channelWriter)
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
			await Task.Delay(_options.TaskDelayMilliseconds, token);

			if (!context.Socket.DataAvailable) { continue; }

			var message = await context.Socket.ReadAsync<EventNotificationRequest>(token);

			if (message == null) { continue; }

			_logger.LogTrace("Listening for events: received {connectionId}", context.ConnectionId);

			var writer = _channelWriter.GetWriter();

			if (await writer.WaitToWriteAsync(token))
			{
				// TODO: Add code to publish an event received from client to other clients listening on that topic.
				// TODO: Also, side note: the client that sent the package should not receive the
				// TODO: same package and publish the event locally after sending it to the server
				// TODO: but might need to verify before publishing. Then again the server should
				// TODO: respond the client with its permission (or if it applies) to publish that event locally 

				// TODO: Nothing is listening to this channel yet. Need to publish to the connected clients.
				await writer.WriteAsync(message.Body.Content!, token);
			}
		}

		_logger.LogTrace("Listening for events: end {connectionId}", context.ConnectionId);
	}
}