namespace IMitto.Local;

public static class EventAggregatorExtensions
{
	public static ISubscription Subscribe(this IEventAggregator eventAggregator, EventId eventId, Action<EventContext> callback)
	{
		var subscription = new ActionSubscription(eventId, eventAggregator, callback);
		eventAggregator.Subscribe(subscription);
		return subscription;
	}

	public static ISubscription Subscribe<T>(this IEventAggregator eventAggregator, EventId eventId, Action<EventContext<T>> callback)
	{
		var subscription = new ActionSubscription<T>(eventId, eventAggregator, callback);
		eventAggregator.Subscribe(subscription);
		return subscription;
	}
}
