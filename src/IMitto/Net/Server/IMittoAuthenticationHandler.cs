using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Server.Results;

namespace IMitto.Net.Server;

public interface IMittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccessAsync(ConnectionContext context, IMittoRequest<MittoTopicMessageBody> request);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, IMittoRequest<MittoAuthenticationMessageBody> request, CancellationToken token = default);
}
