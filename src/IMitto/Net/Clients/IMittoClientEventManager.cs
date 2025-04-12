using IMitto.Local;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public interface IMittoClientEventManager : IMittoLocalEvents
{
	Task WaitForClientEventsAsync(IMittoClientConnection connection, CancellationToken token);

	Task WaitForServerEventsAsync(IMittoClientConnection connection, CancellationToken token);
}
