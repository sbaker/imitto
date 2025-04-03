namespace Transmitto.Net;

public interface ITransmittoRequestHandler
{
	Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default);
}