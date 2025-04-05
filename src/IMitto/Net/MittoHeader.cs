namespace IMitto.Net;

public class MittoHeader : Dictionary<string, object?>
{
	public static readonly MittoHeader Error = new()
	{
		Action = MittoEventType.Error,
		Result = MittoEventType.Error,
	};

	public MittoHeader()
	{
	}

	public MittoHeader(IDictionary<string, object?> headers)
	{
		if (headers.TryGetValue(nameof(Path), out object? value)) Path = value?.ToString();
		if (headers.ContainsKey(nameof(Action))) Action = ParseEventType(headers[nameof(Action)]);
		if (headers.ContainsKey(nameof(Result))) Result = ParseEventType(headers[nameof(Result)]);
		if (headers.TryGetValue(nameof(Length), out object? length)) Length = length as int?;

		static MittoEventType? ParseEventType(object? value)
		{
			var stringValue = value?.ToString();

			if (string.IsNullOrEmpty(stringValue) ) return null;

			return Enum.Parse<MittoEventType>(stringValue);
		}
	}

	public static MittoHeader Authorization(MittoEventType? result = null, string? correlationId = null) => new()
	{
		Path = MittoPaths.Auth,
		Action = MittoEventType.Authentication,
		Result = result,
		CorrelationId = correlationId
	};

	public string? CorrelationId { get; set; }

	public string? Path { get; set; }

	//public TansmittoEnumHeaderValue<MittoEventType>? Action { get; set; }
	public MittoEventType? Action { get; set; }

	//public TansmittoEnumHeaderValue<MittoEventType>? Result { get; set; }
	public MittoEventType? Result { get; set; }

	public int? Length { get; set; }
}

//public class MittoHeaderValue
//{
//	public MittoHeaderValue(string? value = default)
//		=> Value = value;

//	public virtual string? Value { get; set; }

//	public static implicit operator string?(MittoHeaderValue value)
//		=> value.Value;

//	public static implicit operator MittoHeaderValue(string value)
//		=> new(value);
//}

//public class TansmittoEnumHeaderValue<TEnum> : MittoHeaderValue where TEnum : struct
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