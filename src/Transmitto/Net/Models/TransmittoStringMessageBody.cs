namespace Transmitto.Net.Models;

public class TransmittoStringMessageBody : TransmittoMessageBody<string?>
{
	public static implicit operator TransmittoStringMessageBody(string? content)
	{
		return new TransmittoStringMessageBody { Content = content };
	}

	public static implicit operator string?(TransmittoStringMessageBody content)
	{
		return content.Content;
	}
}
