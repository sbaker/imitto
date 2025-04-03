using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Transmitto.Converters;
using Transmitto.Net.Models;

namespace Transmitto.Net.Settings;

public class TransmittoJsonOptions
{
	public JsonSerializerOptions Serializer { get; set; } = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		RespectNullableAnnotations = true,
		//TypeInfoResolver = new TransmittoTypeInfoResolver(),
		UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
		Converters = {
			new JsonStringEnumConverter(),
			new TransmittoBodyConverter(),
			new TransmittoHeaderConverter(),
			new TransmittoAuthenticationRequestConverter()
		}
	};

	private sealed class TransmittoTypeInfoResolver : IJsonTypeInfoResolver
	{
		public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
		{
			var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(type, options);

			return typeInfo;
		}
	}
}

internal class TransmittoHeaderConverter : JsonConverter<TransmittoHeader>
{
	public override TransmittoHeader? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var element = JsonElement.ParseValue(ref reader);
		var result = element.Deserialize<IDictionary<string, object?>>();

		return new TransmittoHeader(result);
	}

	public override void Write(Utf8JsonWriter writer, TransmittoHeader value, JsonSerializerOptions options)
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

		if (value.Length.HasValue)
		{
			writer.WritePropertyName(nameof(value.Length));
			writer.WriteNumberValue(value.Length.Value);
		}

		if (value.Result is not null)
		{
			writer.WritePropertyName(nameof(value.Result));
			writer.WriteStringValue(value.Result?.ToString());
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

internal class TransmittoAuthenticationRequestConverter : JsonConverter<TransmittoAuthenticationMessageBody>
{
	public override TransmittoAuthenticationMessageBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var element = JsonElement.ParseValue(ref reader);
		var result = element.Deserialize<TransmittoAuthenticationMessageBody>();

		if (result is not null)
		{
			result.RawBody = element.ToString();
		}
		
		return result;
	}

	public override void Write(Utf8JsonWriter writer, TransmittoAuthenticationMessageBody value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();

		writer.WritePropertyName(nameof(value.Key));
		writer.WriteStringValue(value.Secret);

		writer.WritePropertyName(nameof(value.Secret));
		writer.WriteStringValue(value.Secret);

		writer.WriteEndObject();
	}
}