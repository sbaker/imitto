
namespace Transmitto.Net;

public class TransmittoHeader : Dictionary<string, TransmittoHeaderValue>
{
	public static readonly TransmittoHeader Error = new()
	{
		Action = TransmittoEventType.Error,
		Result = TransmittoEventType.Error,
	};

	public string? Path { get; set; }

	public TransmittoEventType? Action { get; set; }

	public TransmittoEventType? Result { get; set; }

	public int Length { get; set; }
}

public class TransmittoHeaderValue
{
	public TransmittoHeaderValue(string? value = default)
		=> Value = value;

	public string? Value { get; set; }

	public static implicit operator string?(TransmittoHeaderValue value)
		=> value.Value;

	public static implicit operator TransmittoHeaderValue(string value)
		=> new(value);
}