namespace IMitto.Net.Server;

public class ServerEventContext(ConnectionContext context)
{
	public ConnectionContext Context { get; } = context;

	public static ServerEventContext<TEventData> For<TEventData>(EventId eventId, ConnectionContext context, TEventData data)
	{
		return new ServerEventContext<TEventData>(eventId, context, data);
	}
}

public class ServerEventContext<TEventData>(EventId eventId, ConnectionContext context, TEventData eventData) : EventContext<TEventData>(eventId, context.EventAggregator, eventData)
{
}
