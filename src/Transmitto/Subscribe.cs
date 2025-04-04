namespace Transmitto;

public static class Subscribe
{
	private static IEventAggregator _aggregator = new EventAggregator();

	public static IEventAggregator UseAggregator(IEventAggregator aggregator, bool disposeExisting = true)
	{
		var existing = Interlocked.Exchange(ref _aggregator, aggregator);

		if (disposeExisting)
		{
			existing.Dispose();
		}

		return existing;
	}

	public static ISubscription To<TData>(EventId eventId, Action<EventContext<TData>> callback)
	{
		return _aggregator.Subscribe(eventId, callback);
	}

	public static bool Unsubscribe(ISubscription subscription)
	{
		return _aggregator.Unsubscribe(subscription);
	}

	public static void Publish<T>(EventId eventId, T data)
	{
		_aggregator.Publish(eventId, data);
	}
}