using IMitto.Net.Models;
namespace IMitto.Net.Requests;

public class AuthenticationRequest : MittoRequest<MittoAuthenticationMessageBody>
{
	public AuthenticationRequest(MittoAuthenticationMessageBody? body = null, MittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
