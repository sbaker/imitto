using IMitto.Local;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public interface IMittoClientEventManager : IMittoLocalEvents
{
	Task HandleClientEventReceived(EventNotificationsModel clientEvent, CancellationToken token);
}
