using IMitto.Protocols.Models;

namespace IMitto.Net.Server;

public class ServerEventNotificationsContext
{
	public ServerEventNotificationsContext(string connectionId, EventNotificationsModel message)
	{
		ConnectionId = connectionId;
		Event = message;
	}

	public string ConnectionId { get; }

	public EventNotificationsModel Event { get; }
}