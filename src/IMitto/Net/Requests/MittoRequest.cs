using IMitto.Net.Models;

namespace IMitto.Net.Requests;

public interface IMittoRequest : IMittoMessage
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

public class MittoRequest<TBody> : MittoMessage<TBody>, IMittoRequest where TBody : MittoMessageBody
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
