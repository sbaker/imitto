﻿using IMitto.Local;
using IMitto.Net.Server;

namespace IMitto.Server;

public class ServerEventSubscription<T> : DelegateSubscription<Action<ServerEventContext<T>>>
{
	public ServerEventSubscription(EventId eventId, IEventAggregator aggregator, Action<ServerEventContext<T>> callback) : base(eventId, aggregator, callback)
	{
	}
}
