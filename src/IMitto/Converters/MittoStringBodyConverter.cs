using System.Text.Json;
using System.Text.Json.Serialization;
using IMitto.Net.Models;

namespace IMitto.Converters;

public class MittoStringBodyConverter : JsonConverter<MittoStringMessageBody>
{
	public override MittoStringMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, MittoStringMessageBody value, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}