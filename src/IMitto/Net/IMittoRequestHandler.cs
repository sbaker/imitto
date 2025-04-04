namespace IMitto.Net;

public interface IMittoRequestHandler
{
	IMittoEventListener GetEventListener();

	Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default);
}