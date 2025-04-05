using IMitto.Net.Models;

namespace IMitto.Converters;

internal class MittoAuthenticationRequestConverter : MittoBodyConverter<MittoAuthenticationMessageBody>
{
	//public override MittoAuthenticationMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	//{
	//	var element = JsonElement.ParseValue(ref reader);
	//	var result = element.Deserialize<MittoAuthenticationMessageBody>();

	//	if (result is not null)
	//	{
	//		result.RawBody = element.ToString();
	//	}
		
	//	return result;
	//}

	//public override void Write(Utf8JsonWriter writer, MittoAuthenticationMessageBody value, JsonSerializerOptions options)
	//{
	//	writer.WriteStartObject();

	//	writer.WritePropertyName(nameof(value.Key));
	//	writer.WriteStringValue(value.Secret);

	//	writer.WritePropertyName(nameof(value.Secret));
	//	writer.WriteStringValue(value.Secret);

	//	writer.WriteEndObject();
	//}
}
