using Microsoft.Extensions.Logging;
using IMitto.Channels;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public class ChannelTransmittoEventDispatcher : TransmittoEventDispatcher
{
	private readonly IMittoChannelProvider<EventNotificationsModel> _channelProvider;

	public ChannelTransmittoEventDispatcher(
		ILogger<TransmittoEventDispatcher> logger,
		IMittoChannelProvider<EventNotificationsModel> channelProvider) : base(logger)
	{
		_channelProvider = channelProvider;
	}

	public override ValueTask DispatchAsync(EventNotificationsModel notifications)
	{
		return new ValueTask(Task.Run(() => {
			var channelWriter = _channelProvider.GetWriter();
			return channelWriter.WriteAsync(notifications);
		}));
	}
}