using Transmitto.Net.Models;

namespace Transmitto.Net.Responses;

public interface ITransmittoResponse : ITransmittoMessage
{
}

public abstract class TransmittoResponse : TransmittoMessage, ITransmittoResponse
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(string? body, TransmittoHeader header)
	{
		Header = header;
	}
}

public class TransmittoResponse<TBody> : TransmittoMessage<TBody>, ITransmittoResponse where TBody : TransmittoMessageBody
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(TBody body, TransmittoHeader header) : base(body, header)
	{
	}
}
