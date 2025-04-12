using IMitto.Net.Models;
using System.Text.Json;

namespace IMitto.Converters;

public class MittoEventNotificationsBodyConverter : MittoBodyConverter<EventNotificationsBody>
{
	public override EventNotificationsBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var contentPropertyName = options.PropertyNamingPolicy.ConvertName(nameof(EventNotificationsBody.Content));
		var jsonDocument = JsonDocument.ParseValue(ref reader);
		var contentPropertyElement = jsonDocument.RootElement.GetProperty(contentPropertyName);

		var eventNotificationBody = new EventNotificationsBody
		{
			Content = contentPropertyElement.Deserialize<EventNotificationsModel>(options),
		};

		return eventNotificationBody;
	}

	public override void Write(Utf8JsonWriter writer, EventNotificationsBody value, JsonSerializerOptions options)
	{
		var nameConverter = options.PropertyNamingPolicy.ConvertName(nameof(EventNotificationsBody.Content));
		writer.WriteStartObject();
		writer.WritePropertyName(nameConverter);

		var objectValue = value.Content;

		if (objectValue != null)
		{
			JsonSerializer.Serialize(writer, objectValue, options);
		}

		writer.WriteEndObject();
	}
}