using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Net.Models;

namespace IMitto.Converters;

public class MittoEventConverter : JsonConverter<MittoEvent>
{
	public override MittoEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new MittoEvent();

		if (reader.TokenType == JsonTokenType.StartObject)
		{
			var jsonDocument = JsonDocument.ParseValue(ref reader);

			result.RawMessage = jsonDocument.RootElement.ToString();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, MittoEvent value, JsonSerializerOptions options)
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
