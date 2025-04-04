using IMitto.Net.Models;

namespace IMitto.Converters;

internal class TransmittoAuthenticationRequestConverter : TransmittoBodyConverter<TransmittoAuthenticationMessageBody>
{
	//public override TransmittoAuthenticationMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	//{
	//	var element = JsonElement.ParseValue(ref reader);
	//	var result = element.Deserialize<TransmittoAuthenticationMessageBody>();

	//	if (result is not null)
	//	{
	//		result.RawBody = element.ToString();
	//	}
		
	//	return result;
	//}

	//public override void Write(Utf8JsonWriter writer, TransmittoAuthenticationMessageBody value, JsonSerializerOptions options)
	//{
	//	writer.WriteStartObject();

	//	writer.WritePropertyName(nameof(value.Key));
	//	writer.WriteStringValue(value.Secret);

	//	writer.WritePropertyName(nameof(value.Secret));
	//	writer.WriteStringValue(value.Secret);

	//	writer.WriteEndObject();
	//}
}
