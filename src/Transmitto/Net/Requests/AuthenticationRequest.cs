using Transmitto.Net.Models;
namespace Transmitto.Net.Requests;

public class AuthenticationRequest : TransmittoRequest<TransmittoAuthenticationMessageBody>
{
	public AuthenticationRequest(TransmittoAuthenticationMessageBody? body = null, TransmittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
