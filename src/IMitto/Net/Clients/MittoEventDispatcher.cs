using IMitto.Protocols.Models;

namespace IMitto.Net.Clients;

public abstract class MittoEventDispatcher : IMittoEventDispatcher
{
	public abstract ValueTask DispatchAsync(EventNotificationsModel package);
}
