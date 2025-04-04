namespace IMitto.Net.Models;

public class TransmittoAuthenticationMessageBody : TransmittoMessageBody
{
	public string? Key { get; set; }

	public string? Secret { get; set; }
}
