using Transmitto.Net;

namespace Transmitto.Server;

public interface ITransmittoAuthenticationHandler
{
	Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default);
}
