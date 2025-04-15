using IMitto.Protocols.Models;

namespace IMitto.Protocols.Requests;

public interface IMittoRequest : IMittoMessage
{
}

public interface IMittoRequest<TBody> : IMittoRequest, IMittoMessage<TBody> where TBody : MittoMessageBody
{
}

public abstract class MittoRequest : MittoMessage, IMittoRequest
{
	protected MittoRequest()
	{
	}

	protected MittoRequest(MittoHeader header) : base(header)
	{
		Header = header;
	}
}

public class MittoRequest<TBody> : MittoMessage<TBody>, IMittoRequest<TBody> where TBody : MittoMessageBody
{
	public MittoRequest()
	{
	}

	public MittoRequest(TBody body, MittoHeader header)
	{
		Body = body;
		Header = header;
	}
}
