using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Server;

namespace IMitto.Net;

public interface IMittoRequestHandler
{
	Task HandleAuthenticationAsync(MittoConnectionContext context, CancellationToken token);

	//Task HandleAuthenticationAsync(ConnectionContext context, CancellationToken token);

	Task HandleAuthorizationAsync(MittoConnectionContext context, CancellationToken token);

	//Task HandleAuthorizationAsync(ConnectionContext context, CancellationToken token);

	Task HandleEventNotificationAsync(MittoConnectionContext context, CancellationToken token);
}