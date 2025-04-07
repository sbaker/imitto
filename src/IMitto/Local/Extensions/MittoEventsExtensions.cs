#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto.Local;
#pragma warning restore IDE0130 // Namespace does not match folder structure

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
