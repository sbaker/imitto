namespace IMitto.Net;

public interface IMittoRequestHandler
{
	Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default);
}