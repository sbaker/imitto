using IMitto.Net;
using IMitto.Net.Requests;
using IMitto.Net.Server.Results;

namespace IMitto.Net.Server;

public interface IMittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccess(ConnectionContext context, MittoTopicsRequest message);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default);
}
