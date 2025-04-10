using IMitto.Net;
using IMitto.Net.Requests;
using IMitto.Server.Results;

namespace IMitto.Server;

public interface IMittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccess(ConnectionContext context, MittoTopicsRequest message);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default);
}
