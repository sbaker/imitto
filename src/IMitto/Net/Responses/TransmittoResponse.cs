using IMitto.Net.Models;

namespace IMitto.Net.Responses;

public interface IMittoResponse : IMittoMessage
{
}

public abstract class TransmittoResponse : TransmittoMessage, IMittoResponse
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(string? body, TransmittoHeader header)
	{
		Header = header;
	}
}

public class TransmittoResponse<TBody> : TransmittoMessage<TBody>, IMittoResponse where TBody : TransmittoMessageBody
{
	public TransmittoResponse()
	{
	}

	public TransmittoResponse(TBody body, TransmittoHeader header) : base(body, header)
	{
	}
}
