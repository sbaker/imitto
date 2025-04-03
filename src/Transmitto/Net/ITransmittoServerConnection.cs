
namespace Transmitto.Net;

public interface ITransmittoServerConnection : ITransmittoConnection
{
	Task<TransmittoSocket> AcceptAsync(CancellationToken token = default);

	Task ConnectAsync(CancellationToken token = default);
}
