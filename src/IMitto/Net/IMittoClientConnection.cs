using IMitto.Net.Models;

namespace IMitto.Net;

public interface IMittoClientConnection : IMittoConnection
{
	Task ConnectAsync(CancellationToken token = default);

	Task<TransmittoStatus> AuthenticateAsync(CancellationToken token = default);
	
	Task<EventNotificationsModel> WaitForEventsAsync(CancellationToken token);
}
