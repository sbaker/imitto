using System.Text.Json;

namespace IMitto.Protocols.Models;

public class MittoMessageBody
{
	internal string? RawBody { get; set; }

	internal JsonElement BodyElement { get; set; }


	public static implicit operator MittoMessageBody(string? content)
	{
		return new MittoStringMessageBody { Content = content };
	}

	public static implicit operator string?(MittoMessageBody content)
	{
		return content.RawBody;
	}
}

public class MittoMessageBody<T> : MittoMessageBody
{
	public T? Content { get; set; }
}
