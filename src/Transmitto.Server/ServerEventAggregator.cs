using Transmitto.Net;

namespace Transmitto.Server;

public sealed class ServerEventAggregator : EventAggregator, IServerEventAggregator
{
	public ServerEventAggregator() : base(SubscriberDefaults.InMemory)
	{
	}

	public ISubscription Subscribe(string topic, ConnectionContext context)
	{
		return SubscribeCore(new ReadSocketSubscription(topic, this, context));
	}
}
