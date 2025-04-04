using Transmitto.Net.Server;

namespace Transmitto.Server;

public class ServerEventSubscription<T> : Subscription<Action<ServerEventContext<T>>>
{
	public ServerEventSubscription(EventId eventId, IEventAggregator aggregator, Action<ServerEventContext<T>> callback) : base(eventId, aggregator, callback)
	{
	}
}
