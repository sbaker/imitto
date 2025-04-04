using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Net.Models;

namespace IMitto.Converters;

public class TransmittoStringBodyConverter : JsonConverter<TransmittoStringMessageBody>
{
	public override TransmittoStringMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, TransmittoStringMessageBody value, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}