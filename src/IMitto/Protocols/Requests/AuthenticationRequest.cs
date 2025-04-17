using IMitto.Protocols.Models;
namespace IMitto.Protocols.Requests;

public class AuthenticationRequest : MittoRequest<MittoAuthenticationMessageBody>
{
	public AuthenticationRequest(MittoAuthenticationMessageBody? body = null, MittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
