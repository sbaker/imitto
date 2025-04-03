using Transmitto.Net.Models;

namespace Transmitto.Net.Responses;

public abstract class TransmittoResponse : TransmittoMessage
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(string? body, TransmittoHeader header)
	{
		Header = header;
	}
}

public class TransmittoResponse<TBody> : TransmittoMessage<TBody> where TBody : TransmittoMessageBody
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(TBody body, TransmittoHeader header) : base(body, header)
	{
	}
}
