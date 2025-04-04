using IMitto.Net.Requests;

namespace IMitto.Net.Server;

public class RequestEventContext : ServerEventContext<IMittoRequest>
{
	public RequestEventContext(EventId eventId, ConnectionContext context, IMittoRequest request) : base(eventId, context, request)
	{
	}
}