using Microsoft.Extensions.Logging;
using IMitto.Net.Server.Results;
using IMitto.Protocols;

namespace IMitto.Net.Server.Internal;

public class NullAuthenticationHandler(ILogger<NullAuthenticationHandler> logger) : IMittoAuthenticationHandler
{
	public Task<TopicAuthorizationResult> AuthorizeTopicAccessAsync(ConnectionContext context, IMittoPackage package)
	{
		logger.LogTrace("Authorized");
		return Task.FromResult(TopicAuthorizationResult.Success);
	}

	public Task<bool> HandleAuthenticationAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default)
	{
		logger.LogTrace("Authenticated");
		return Task.FromResult(true);
	}
}