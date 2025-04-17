namespace IMitto.Protocols.Models;

public class MittoAuthenticationMessageBody : MittoMessageBody
{
	public string? Key { get; set; }

	public string? Secret { get; set; }
}
