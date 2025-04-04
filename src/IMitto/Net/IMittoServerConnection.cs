
namespace IMitto.Net;

public interface IMittoServerConnection : IMittoConnection
{
	Task<TransmittoSocket> AcceptAsync(CancellationToken token = default);

	Task ConnectAsync(CancellationToken token = default);
}
