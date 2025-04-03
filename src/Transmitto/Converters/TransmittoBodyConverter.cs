using System.Text.Json;
using System.Text.Json.Serialization;
using Transmitto.Net.Models;

namespace Transmitto.Converters;

public class TransmittoBodyConverter : JsonConverter<TransmittoMessageBody>
{
	public override TransmittoMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var result = new TransmittoMessageBody();

		if (reader.TokenType == JsonTokenType.StartObject)
		{
			var jsonDocument = JsonDocument.ParseValue(ref reader);

			result.RawBody = jsonDocument.RootElement.ToString();
		}

		return result;
	}

	public override void Write(Utf8JsonWriter writer, TransmittoMessageBody value, JsonSerializerOptions options)
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
