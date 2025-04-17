using IMitto.Net.Server.Results;
using IMitto.Protocols.Models;
using IMitto.Protocols.Requests;

namespace IMitto.Net.Server;

public interface IMittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccessAsync(ConnectionContext context, IMittoRequest<MittoTopicMessageBody> request);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, IMittoRequest<MittoAuthenticationMessageBody> request, CancellationToken token = default);
}
