using IMitto.Net.Models;

namespace IMitto.Net.Responses;

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


