using Transmitto.Net;

namespace Transmitto.Server;

public interface IServerEventAggregator : IEventAggregator
{
	ISubscription Subscribe(string topic, ConnectionContext context);
}