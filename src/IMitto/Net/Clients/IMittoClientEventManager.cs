using IMitto.Local;

namespace IMitto.Net.Clients;

public interface IMittoClientEventManager : IMittoLocalEvents
{
	Task WaitForClientEventsAsync(IMittoClientConnection connection, CancellationToken token);

	Task WaitForServerEventsAsync(IMittoClientConnection connection, CancellationToken token);
}
