﻿using IMitto.Local;

namespace IMitto;

public static class Subscribe
{
	private static IMittoEvents _aggregator = new MittoEvents();

	public static IMittoEvents UseAggregator(IMittoEvents aggregator, bool disposeExisting = true)
	{
		var existing = Interlocked.Exchange(ref _aggregator, aggregator);

		if (disposeExisting)
		{
			existing.Dispose();
		}

		return existing;
	}

	public static ISubscription ToLocalEvent<TData>(EventId eventId, Action<EventContext<TData>> callback)
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