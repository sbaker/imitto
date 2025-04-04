using Transmitto.Net.Requests;

namespace Transmitto.Net.Server;

public class RequestEventContext : ServerEventContext<ITransmittoRequest>
{
	public RequestEventContext(EventId eventId, ConnectionContext context, ITransmittoRequest request) : base(eventId, context, request)
	{
	}
}