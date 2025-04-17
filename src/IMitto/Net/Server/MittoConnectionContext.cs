using IMitto.Middlware;
using IMitto.Protocols.Models;
using IMitto.Protocols.Requests;
using IMitto.Protocols.Responses;

namespace IMitto.Net.Server;

public class MittoConnectionContext : MiddlewareContext
{
	public MittoConnectionContext(ConnectionContext context, IMittoRequest<MittoMessageBody> request) : base(context.ConnectionId)
	{
		Connection = context;
		Request = request;
	}

	public ConnectionContext Connection { get; private set; }

	public IMittoRequest<MittoMessageBody> Request { get; private set; }

	public Task WriteResponse<TMittoResponse>(TMittoResponse response) where TMittoResponse : IMittoResponse
	{
		return Connection.Socket.SendResponseAsync(response);
	}
}
