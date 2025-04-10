using Microsoft.Extensions.Logging;
using IMitto.Channels;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public class ChannelMittoEventDispatcher : MittoEventDispatcher
{
	private readonly IMittoChannelProvider<EventNotificationsModel> _channelProvider;

	public ChannelMittoEventDispatcher(
		ILogger<MittoEventDispatcher> logger,
		IMittoChannelProvider<EventNotificationsModel> channelProvider)
	{
		_channelProvider = channelProvider;
	}

	public override ValueTask DispatchAsync(EventNotificationsModel package)
	{
		return new ValueTask(Task.Run(() => {
			var channelWriter = _channelProvider.GetWriter();
			return channelWriter.WriteAsync(package);
		}));
	}
}