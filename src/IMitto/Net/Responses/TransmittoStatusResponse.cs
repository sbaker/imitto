using IMitto.Net.Models;

namespace IMitto.Net.Responses;

public class TransmittoStatusResponse : TransmittoResponse<TransmittoStatusBody>
{
	public TransmittoStatusResponse() : this(new(), new())
	{
	}

	public TransmittoStatusResponse(TransmittoStatusBody body, TransmittoHeader header)
	{
		Body = body;
		Header = header;
	}
}


