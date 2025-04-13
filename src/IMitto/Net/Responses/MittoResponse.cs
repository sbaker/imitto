using IMitto.Net.Models;

namespace IMitto.Net.Responses;

public interface IMittoResponse : IMittoMessage
{
}

public interface IMittoResponse<TBody> : IMittoResponse, IMittoMessage<TBody> where TBody : MittoMessageBody
{

}

public abstract class MittoResponse : MittoMessage, IMittoResponse
{
	public MittoResponse()
	{
	}

	public MittoResponse(string? body, MittoHeader header)
	{
		Header = header;
	}
}

public class MittoResponse<TBody> : MittoMessage<TBody>, IMittoResponse<TBody>, IMittoResponse where TBody : MittoMessageBody
{
	public MittoResponse()
	{
	}

	public MittoResponse(TBody body, MittoHeader header) : base(body, header)
	{
	}
}
