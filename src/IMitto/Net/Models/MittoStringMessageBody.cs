namespace IMitto.Net.Models;

public class MittoStringMessageBody : MittoMessageBody
{
	public string? Content { get; set; }

	public static implicit operator MittoStringMessageBody(string? content)
	{
		return new MittoStringMessageBody { Content = content };
	}

	public static implicit operator string?(MittoStringMessageBody content)
	{
		return content.Content;
	}
}
