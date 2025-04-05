
namespace IMitto.Net;

public interface IMittoServerConnection : IMittoConnection
{
	Task<MittoSocket> AcceptAsync(CancellationToken token = default);

	Task ConnectAsync(CancellationToken token = default);
}
