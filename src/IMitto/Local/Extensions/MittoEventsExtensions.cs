namespace IMitto.Local;

public static class MittoEventsExtensions
{
	public static ISubscription Subscribe(this IMittoEvents events, EventId eventId, Action<EventContext> callback)
	{
		var subscription = new ActionSubscription(eventId, events, callback);
		events.Subscribe(subscription);
		return subscription;
	}

	public static ISubscription Subscribe<T>(this IMittoEvents events, EventId eventId, Action<EventContext<T>> callback)
	{
		var subscription = new ActionSubscription<T>(eventId, events, callback);
		events.Subscribe(subscription);
		return subscription;
	}
}
