using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Net.Models;

namespace IMitto.Converters;

public class TransmittoEventConverter : JsonConverter<TransmittoEvent>
{
	public override TransmittoEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new TransmittoEvent();

		if (reader.TokenType == JsonTokenType.StartObject)
		{
			var jsonDocument = JsonDocument.ParseValue(ref reader);

			result.RawMessage = jsonDocument.RootElement.ToString();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, TransmittoEvent value, JsonSerializerOptions options)
	{
		var type = value.GetType();
		var properties = type.GetProperties();

		writer.WriteStartObject();

		foreach (var property in properties)
		{
			writer.WritePropertyName(property.Name);
			
			writer.WriteStringValue(property.GetValue(value)?.ToString());
		}

		writer.WriteEndObject();
	}
}
