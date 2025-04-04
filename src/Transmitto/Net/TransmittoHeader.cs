namespace Transmitto.Net;

public class TransmittoHeader : Dictionary<string, object?>
{
	public static readonly TransmittoHeader Error = new()
	{
		Action = TransmittoEventType.Error,
		Result = TransmittoEventType.Error,
	};

	public TransmittoHeader()
	{
	}

	public TransmittoHeader(IDictionary<string, object?> headers)
	{
		if (headers.TryGetValue(nameof(Path), out object? value)) Path = value?.ToString();
		if (headers.ContainsKey(nameof(Action))) Action = ParseEventType(headers[nameof(Action)]);
		if (headers.ContainsKey(nameof(Result))) Result = ParseEventType(headers[nameof(Result)]);
		if (headers.TryGetValue(nameof(Length), out object? length)) Length = length as int?;

		static TransmittoEventType? ParseEventType(object? value)
		{
			var stringValue = value?.ToString();

			if (string.IsNullOrEmpty(stringValue) ) return null;

			return Enum.Parse<TransmittoEventType>(stringValue);
		}
	}

	public static TransmittoHeader Authorization(TransmittoEventType? result = null, string? correlationId = null) => new()
	{
		Path = TransmittoPaths.Auth,
		Action = TransmittoEventType.Authentication,
		Result = result,
		CorrelationId = correlationId
	};

	public string? CorrelationId { get; set; }

	public string? Path { get; set; }

	//public TansmittoEnumHeaderValue<TransmittoEventType>? Action { get; set; }
	public TransmittoEventType? Action { get; set; }

	//public TansmittoEnumHeaderValue<TransmittoEventType>? Result { get; set; }
	public TransmittoEventType? Result { get; set; }

	public int? Length { get; set; }
}

//public class TransmittoHeaderValue
//{
//	public TransmittoHeaderValue(string? value = default)
//		=> Value = value;

//	public virtual string? Value { get; set; }

//	public static implicit operator string?(TransmittoHeaderValue value)
//		=> value.Value;

//	public static implicit operator TransmittoHeaderValue(string value)
//		=> new(value);
//}

//public class TansmittoEnumHeaderValue<TEnum> : TransmittoHeaderValue where TEnum : struct
//{
//	private TEnum _value = default;

//	public TansmittoEnumHeaderValue()
//	{
//	}

//	public TansmittoEnumHeaderValue(TEnum value)
//	{
//		_value = value;
//	}

//	public override string? Value
//	{
//		get => _value.ToString();
//		set => _value = value is not null
//			? Enum.Parse<TEnum>(value)
//			: _value = default;
//	}
//}