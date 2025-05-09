﻿using IMitto.Local;

namespace IMitto.Net.Server;

public interface IServerEventManager : IMittoLocalEvents
{
	Task PublishServerEventAsync<TData>(EventId eventId, ConnectionContext context, TData data, CancellationToken token);
	
	Task RunAsync(CancellationToken token);

	//ISubscription Subscribe(string topic, ConnectionContext context);
}