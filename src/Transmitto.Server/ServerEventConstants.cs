using Transmitto.Net;
using Transmitto.Net.Requests;
using Transmitto.Net.Responses;

namespace Transmitto.Server;

internal static class ServerEventConstants
{
	public const string MessageReceivedEvent = nameof(MessageReceivedEvent);

	public const string ConnectionReceivedEvent = nameof(ConnectionReceivedEvent);
}

public class ServerEventContext(ConnectionContext context)
{
	public ConnectionContext Context { get; } = context;

	public static ServerEventContext<TMessage> For<TMessage>(ConnectionContext context, TMessage body) where TMessage : ITransmittoMessage
	{
		return new ServerEventContext<TMessage>(context, body);
	}

	public static ServerEventContext<TMessage> ForRequest<TMessage>(ConnectionContext context, TMessage request) where TMessage : TransmittoRequest
	{
		return new ServerEventContext<TMessage>(context, request);
	}

	public static ServerEventContext<TMessage> ForResponse<TMessage>(ConnectionContext context, TMessage response) where TMessage : TransmittoResponse
	{
		return new ServerEventContext<TMessage>(context, response);
	}
}

public class ServerEventContext<TMessage>(ConnectionContext context, TMessage message) : ServerEventContext(context) where TMessage : ITransmittoMessage
{
	public TMessage Message { get; } = message;
}