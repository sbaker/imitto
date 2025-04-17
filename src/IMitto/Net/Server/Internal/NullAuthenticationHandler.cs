using Microsoft.Extensions.Logging;
using IMitto.Net.Server.Results;
using IMitto.Protocols.Models;
using IMitto.Protocols.Requests;

namespace IMitto.Net.Server.Internal;

public class NullAuthenticationHandler(ILogger<NullAuthenticationHandler> logger) : IMittoAuthenticationHandler
{
	public Task<TopicAuthorizationResult> AuthorizeTopicAccessAsync(ConnectionContext context, IMittoRequest<MittoTopicMessageBody> request)
	{
		logger.LogTrace("Authorized");
		return Task.FromResult(TopicAuthorizationResult.Success);
	}

	public Task<bool> HandleAuthenticationAsync(ConnectionContext context, IMittoRequest<MittoAuthenticationMessageBody> request, CancellationToken token = default)
	{
		logger.LogTrace("Authenticated");
		return Task.FromResult(true);
	}
}