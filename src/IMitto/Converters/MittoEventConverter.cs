using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Protocols.Models;

namespace IMitto.Converters;

public class MittoEventConverter : JsonConverter<MittoEvent>
{
	public override MittoEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var jsonDocument = JsonDocument.ParseValue(ref reader);
		var rootElement = jsonDocument.RootElement;
		var package = rootElement.GetProperty("Package");
		
		return new MittoEvent
		{
			Package = package
		};
	}

	public override void Write(Utf8JsonWriter writer, MittoEvent value, JsonSerializerOptions options)
	{
		var type = value.GetType();
		var properties = type.GetProperties();

		writer.WriteStartObject();

		foreach (var property in properties)
		{
			writer.WritePropertyName(property.Name);

			if (property.PropertyType == typeof(string))
			{
				writer.WriteStringValue(property.GetValue(value)?.ToString());
				continue;
			}

			if (property.PropertyType == typeof(int))
			{
				var intValue = property.GetValue(value) as int?;

				if (intValue != null)
				{
					writer.WriteNumberValue(intValue ?? 0); // Handle null values safely
					continue;
				}
			}

			if (property.PropertyType == typeof(DateTime))
			{
				var dateTimeValue = property.GetValue(value) as DateTime?;
				writer.WriteStringValue(dateTimeValue?.ToString("o") ?? string.Empty);
				continue;
			}

			if (property.PropertyType == typeof(object))
			{
				var objectValue = property.GetValue(value);

				if (objectValue != null)
				{
					JsonSerializer.Serialize(writer, objectValue, options);
					continue;
				}
			}

			writer.WriteNullValue();
		}

		writer.WriteEndObject();
	}
}
