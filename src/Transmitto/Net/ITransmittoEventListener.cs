using Microsoft.Extensions.Logging;
using Transmitto.Channels;
using Transmitto.Net.Models;
using Transmitto.Net.Requests;

namespace Transmitto.Net;

public interface ITransmittoEventListener
{
	Task PollForEventsAsync(ConnectionContext context, CancellationToken token);
}

public class TransmittoEventListener : ITransmittoEventListener
{
	private readonly ILogger<TransmittoEventListener> _logger;
	private readonly ITransmittoChannelWriterProvider<ClientNotificationModel> _channelWriter;

	public TransmittoEventListener(ILogger<TransmittoEventListener> logger, ITransmittoChannelWriterProvider<ClientNotificationModel> channelWriter)
	{
		_logger = logger;
		_channelWriter = channelWriter;
	}

	public async Task PollForEventsAsync(ConnectionContext context, CancellationToken token)
	{
		_logger.LogTrace("Listening for events: start {connectionId}", context.ConnectionId);

		while (!token.IsCancellationRequested)
		{
			await Task.Delay(100, token);

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