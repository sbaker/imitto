namespace IMitto.Net;

public interface IMittoRequestHandler
{
	Task HandleAuthenticationAsync(ConnectionContext context, CancellationToken token);

	Task HandleAuthorizationAsync(ConnectionContext context, CancellationToken token);
}