using Transmitto.Net;
using Transmitto.Net.Requests;
using Transmitto.Server.Results;

namespace Transmitto.Server;

public interface ITransmittoAuthenticationHandler
{
	Task<TopicAuthorizationResult> AuthorizeTopicAccess(ConnectionContext context, TransmittoTopicsRequest message);

	Task<bool> HandleAuthenticationAsync(ConnectionContext context, CancellationToken token = default);
}
