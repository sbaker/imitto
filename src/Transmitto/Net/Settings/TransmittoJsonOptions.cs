using System.Text.Json.Serialization;
using System.Text.Json;
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
			new TransmittoBodyConverter<TransmittoMessageBody>(),
			new TransmittoHeaderConverter(),
			new TransmittoAuthenticationRequestConverter()
		}
	};

	//private sealed class TransmittoTypeInfoResolver : IJsonTypeInfoResolver
	//{
	//	public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
	//	{
	//		var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(type, options);

	//		return typeInfo;
	//	}
	//}
}
