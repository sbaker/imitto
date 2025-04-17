using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Protocols.Models;

namespace IMitto.Converters;

public class MittoBodyConverter<TBody> : JsonConverter<TBody> where TBody : MittoMessageBody, new()
{
	public override TBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var jsonDocument = JsonDocument.ParseValue(ref reader);

		var result = jsonDocument.Deserialize<TBody>() ?? new();

		result.BodyElement = jsonDocument.RootElement;

		if (result.BodyElement.TryGetProperty(options.PropertyNamingPolicy!.ConvertName(nameof(MittoMessageBody<int>.Content)), out var value))
		{
			result.BodyElement = value;
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, TBody value, JsonSerializerOptions options)
	{
		var type = value.GetType();
		var properties = type.GetProperties();

		writer.WriteStartObject();

		foreach (var property in properties)
		{
			var propertyValue = property.GetValue(value)?.ToString();

			if (value is not null)
			{
				writer.WritePropertyName(property.Name);

				writer.WriteStringValue(propertyValue);
			}
		}

		writer.WriteEndObject();
	}
}
