using IMitto.Net.Models;
namespace IMitto.Net.Requests;

public class AuthenticationRequest : TransmittoRequest<TransmittoAuthenticationMessageBody>
{
	public AuthenticationRequest(TransmittoAuthenticationMessageBody? body = null, TransmittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
