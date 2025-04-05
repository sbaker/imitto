namespace IMitto.Local;

public interface ILocalEventAggregator : IEventAggregator
{
	ISubscription Subscribe(EventId eventId, Action<EventContext> callback);

	ISubscription Subscribe<TData>(EventId eventId, Action<EventContext<TData>> callback);
}
