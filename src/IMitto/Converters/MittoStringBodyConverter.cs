using IMitto.Net.Models;
using System.Text.Json;

namespace IMitto.Converters;

public class MittoStringBodyConverter : MittoBodyConverter<MittoStringMessageBody>;

public class MittoEventNotificationsBodyConverter : MittoBodyConverter<EventNotificationsBody>
{
	public override EventNotificationsBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var jsonDocument = JsonDocument.ParseValue(ref reader);
		var result = jsonDocument.Deserialize<EventNotificationsBody>() ?? new();
		result.RawBody = jsonDocument.RootElement.ToString();
		return result;
	}
	public override void Write(Utf8JsonWriter writer, EventNotificationsBody value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("content");

		var objectValue = value.Content;

		if (objectValue != null)
		{
			JsonSerializer.Serialize(writer, objectValue, options);
		}

		writer.WriteEndObject();
	}
}