using Microsoft.Extensions.Logging;
using IMitto.Net;
using IMitto.Net.Requests;
using IMitto.Server.Results;

namespace IMitto.Server;

public class NullAuthenticationHandler(ILogger<NullAuthenticationHandler> logger) : IMittoAuthenticationHandler
{
	public Task<TopicAuthorizationResult> AuthorizeTopicAccess(ConnectionContext context, MittoTopicsRequest message)
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