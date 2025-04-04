namespace IMitto.Net.Models;

public class TransmittoMessageBody
{
	internal string? RawBody { get; set; }


	public static implicit operator TransmittoMessageBody(string? content)
	{
		return new TransmittoStringMessageBody { Content = content };
	}

	public static implicit operator string?(TransmittoMessageBody content)
	{
		return content.RawBody;
	}
}

public class TransmittoMessageBody<T> : TransmittoMessageBody
{
	public T? Content { get; set; }
}
