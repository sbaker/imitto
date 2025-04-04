using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public interface IMittoEventDispatcher
{
	ValueTask DispatchAsync(EventNotificationsModel notifications);
}
