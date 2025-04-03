using Transmitto.Net.Models;

namespace Transmitto.Net.Requests;

public abstract class TransmittoRequest : TransmittoMessage
{
	public TransmittoRequest()
	{
	}

	public TransmittoRequest(TransmittoHeader header) : base(header)
	{
		Header = header;
	}
}

public class TransmittoRequest<TBody> : TransmittoMessage<TBody> where TBody : TransmittoMessageBody
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
