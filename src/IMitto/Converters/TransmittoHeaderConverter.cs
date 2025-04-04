using System.Text.Json.Serialization;
using System.Text.Json;
using IMitto.Net;

namespace IMitto.Converters;

internal class TransmittoHeaderConverter : JsonConverter<TransmittoHeader>
{
	public override TransmittoHeader? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var element = JsonElement.ParseValue(ref reader);
		var result = element.Deserialize<IDictionary<string, object?>>();

		return new TransmittoHeader(result);
	}

	public override void Write(Utf8JsonWriter writer, TransmittoHeader value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		if (value.Path is not null)
		{
			writer.WritePropertyName(nameof(value.Path));
			writer.WriteStringValue(value.Path);
		}

		if (value.Action is not null)
		{
			writer.WritePropertyName(nameof(value.Action));
			writer.WriteStringValue(value.Action.ToString());
		}

		if (value.Length.HasValue)
		{
			writer.WritePropertyName(nameof(value.Length));
			writer.WriteNumberValue(value.Length.Value);
		}

		if (value.Result is not null)
		{
			writer.WritePropertyName(nameof(value.Result));
			writer.WriteStringValue(value.Result?.ToString());
		}

		foreach (var item in value)
		{
			if (item.Value is not null)
			{
				writer.WritePropertyName(item.Key);
				writer.WriteStringValue(item.Value.ToString());
			}
		}

		writer.WriteEndObject();
	}
}