using Transmitto.Net.Models;

namespace Transmitto.Net;

public interface ITransmittoClientConnection : ITransmittoConnection
{
	Task ConnectAsync(CancellationToken token = default);

	Task<TransmittoStatus> AuthenticateAsync(CancellationToken token = default);
	
	Task<EventNotificationsModel> WaitForEventsAsync(CancellationToken token);
}
