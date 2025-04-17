using IMitto.Protocols.Models;

namespace IMitto.Protocols.Responses;

public class MittoStatusResponse : MittoResponse<MittoStatusBody>
{
	public MittoStatusResponse() : this(new(), new())
	{
	}

	public MittoStatusResponse(MittoStatusBody body, MittoHeader header)
	{
		Body = body;
		Header = header;
	}
}


