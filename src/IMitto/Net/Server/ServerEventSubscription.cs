using IMitto.Local;

namespace IMitto.Net.Server;

public class ServerEventSubscription<T> : DelegateSubscription<Action<ServerEventContext<T>>>
{
	public ServerEventSubscription(EventId eventId, IEventAggregator aggregator, Action<ServerEventContext<T>> callback) : base(eventId, aggregator, callback)
	{
	}
}
