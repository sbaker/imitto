using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Channels;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Settings;

namespace IMitto.Net;

public interface IMittoEventListener
{
	Task PollForEventsAsync(ConnectionContext context, CancellationToken token);
}

public class MittoEventListener : IMittoEventListener
{
	private readonly ILogger<MittoEventListener> _logger;
	private readonly MittoConnectionOptions _options;
	private readonly IMittoChannelWriterProvider<ClientNotificationModel> _channelWriter;

	public MittoEventListener(ILogger<MittoEventListener> logger, IOptions<MittoConnectionOptions> options, IMittoChannelWriterProvider<ClientNotificationModel> channelWriter)
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

			var message = await context.Socket.ReadAsync<ClientNotificationRequest>(token);

			if (message == null) { continue; }

			_logger.LogTrace("Listening for events: received {connectionId}", context.ConnectionId);

			var writer = _channelWriter.GetWriter();

			if (await writer.WaitToWriteAsync(token))
			{
				await writer.WriteAsync(message.Body.Notification, token);
			}
		}

		_logger.LogTrace("Listening for events: end {connectionId}", context.ConnectionId);
	}
}