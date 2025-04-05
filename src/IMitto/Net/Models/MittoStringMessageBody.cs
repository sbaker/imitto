namespace IMitto.Net.Models;

public class MittoStringMessageBody : MittoMessageBody<string?>
{
	public static implicit operator MittoStringMessageBody(string? content)
	{
		return new MittoStringMessageBody { Content = content };
	}

	public static implicit operator string?(MittoStringMessageBody content)
	{
		return content.Content;
	}
}
