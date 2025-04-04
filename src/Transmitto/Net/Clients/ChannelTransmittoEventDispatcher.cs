using Microsoft.Extensions.Logging;
using Transmitto.Channels;
using Transmitto.Net.Models;

namespace Transmitto.Net.Clients;

public class ChannelTransmittoEventDispatcher : TransmittoEventDispatcher
{
	private readonly ITransmittoChannelProvider<EventNotificationsModel> _channelProvider;

	public ChannelTransmittoEventDispatcher(
		ILogger<TransmittoEventDispatcher> logger,
		ITransmittoChannelProvider<EventNotificationsModel> channelProvider) : base(logger)
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