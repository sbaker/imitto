namespace IMitto.Local
{
	public class LocalEventAggregator : EventAggregator, ILocalEventAggregator
	{
		public ISubscription Subscribe(EventId eventId, Action<EventContext> callback)
		{
			return SubscribeCore(new ActionSubscription(eventId, this, callback));
		}

		public virtual ISubscription Subscribe<T>(EventId eventId, Action<EventContext<T>> callback)
		{
			return SubscribeCore(new ActionSubscription<T>(eventId, this, callback));
		}
	}
}