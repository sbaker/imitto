using System.Text.Json.Serialization;
using System.Text.Json;
using IMitto.Protocols;
using IMitto.Protocols.Models;

namespace IMitto.Converters;

internal class MittoHeaderConverter : JsonConverter<MittoHeader>
{
	public override MittoHeader? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var element = JsonElement.ParseValue(ref reader);
		var result = element.Deserialize<IDictionary<string, object?>>();

		return new MittoHeader(result);
	}

	public override void Write(Utf8JsonWriter writer, MittoHeader value, JsonSerializerOptions options)
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

		if (value.Result is not null)
		{
			writer.WritePropertyName(nameof(value.Result));
			writer.WriteStringValue(value.Result?.ToString());
		}

		if (value.ConnectionId is not null)
		{
			writer.WritePropertyName(nameof(value.ConnectionId));
			writer.WriteStringValue(value.ConnectionId?.ToString());
		}

		if (value.Version is not null)
		{
			writer.WritePropertyName(nameof(value.Version));
			writer.WriteStringValue(value.Version?.ToString());
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