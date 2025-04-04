namespace Transmitto.Net;

public interface ITransmittoRequestHandler
{
	ITransmittoEventListener GetEventListener();

	Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default);
}