using IMitto.Net.Models;
using IMitto.Net.Requests;

namespace IMitto.Net.Clients;

public interface IMittoClientConnection : IMittoConnection
{
	Task ConnectAsync(CancellationToken token = default);
	
	Task<EventNotificationsModel> WaitForEventsAsync(CancellationToken token);

	Task SendRequestAsync(AuthenticationRequest authenticationRequest, CancellationToken token);

	Task<T?> ReadResponseAsync<T>(CancellationToken token);
}
