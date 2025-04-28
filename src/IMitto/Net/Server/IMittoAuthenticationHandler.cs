using IMitto.Net.Server.Results;
using IMitto.Protocols;

namespace IMitto.Net.Server;

public interface IMittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccessAsync(ConnectionContext context, IMittoPackage package);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, IMittoPackage package, CancellationToken token = default);
}
