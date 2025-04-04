using Transmitto.Net.Models;

namespace Transmitto.Net.Requests;

public interface ITransmittoRequest : ITransmittoMessage
{
}

public abstract class TransmittoRequest : TransmittoMessage, ITransmittoRequest
{
	protected TransmittoRequest()
	{
	}

	protected TransmittoRequest(TransmittoHeader header) : base(header)
	{
		Header = header;
	}
}

public class TransmittoRequest<TBody> : TransmittoMessage<TBody>, ITransmittoRequest where TBody : TransmittoMessageBody
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
