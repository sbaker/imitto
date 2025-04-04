using IMitto.Net.Models;

namespace IMitto.Net.Requests;

public interface IMittoRequest : IMittoMessage
{
}

public abstract class TransmittoRequest : TransmittoMessage, IMittoRequest
{
	protected TransmittoRequest()
	{
	}

	protected TransmittoRequest(TransmittoHeader header) : base(header)
	{
		Header = header;
	}
}

public class TransmittoRequest<TBody> : TransmittoMessage<TBody>, IMittoRequest where TBody : TransmittoMessageBody
{
	public TransmittoRequest()
	{
	}

	public TransmittoRequest(TBody body, TransmittoHeader header)
	{
		Body = body;
		Header = header;
	}
}
