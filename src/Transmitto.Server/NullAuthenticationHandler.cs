using Microsoft.Extensions.Logging;
using Transmitto.Net;
using Transmitto.Net.Requests;
using Transmitto.Server.Results;

namespace Transmitto.Server;

public class NullAuthenticationHandler(ILogger<NullAuthenticationHandler> logger) : ITransmittoAuthenticationHandler
{
	public Task<TopicAuthorizationResult> AuthorizeTopicAccess(ConnectionContext context, TransmittoTopicsRequest message)
	{
		logger.LogTrace("Authorized");
		return Task.FromResult(TopicAuthorizationResult.Success);
	}

	public Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default)
	{
		logger.LogTrace("Authenticated");
		return Task.FromResult(true);
	}
}