using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Transmitto.Channels;
using Transmitto.Net.Models;
using Transmitto.Net.Requests;
using Transmitto.Net.Settings;

namespace Transmitto.Net;

public interface ITransmittoEventListener
{
	Task PollForEventsAsync(ConnectionContext context, CancellationToken token);
}

public class TransmittoEventListener : ITransmittoEventListener
{
	private readonly ILogger<TransmittoEventListener> _logger;
	private readonly TransmittoConnectionOptions _options;
	private readonly ITransmittoChannelWriterProvider<ClientNotificationModel> _channelWriter;

	public TransmittoEventListener(ILogger<TransmittoEventListener> logger, IOptions<TransmittoConnectionOptions> options, ITransmittoChannelWriterProvider<ClientNotificationModel> channelWriter)
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