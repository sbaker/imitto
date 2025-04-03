using Microsoft.Extensions.Logging;
using Transmitto.Net;

namespace Transmitto.Server;

public class NullAuthenticationHandler(ILogger<NullAuthenticationHandler> logger) : ITransmittoAuthenticationHandler
{
	public Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default)
	{
		logger.LogTrace("Authenticated");
		return Task.FromResult(true);
	}
}