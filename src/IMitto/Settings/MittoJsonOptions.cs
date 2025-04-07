using System.Text.Json.Serialization;
using System.Text.Json;
using IMitto.Converters;
using IMitto.Net.Models;

namespace IMitto.Settings;

public class MittoJsonOptions
{
	public JsonSerializerOptions Serializer { get; set; } = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		RespectNullableAnnotations = true,
		UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
		Converters = {
			new JsonStringEnumConverter(),
			new MittoBodyConverter<MittoMessageBody>(),
			new MittoHeaderConverter(),
			new MittoAuthenticationRequestConverter()
		}
	};

	//private sealed class MittoTypeInfoResolver : IJsonTypeInfoResolver
	//{
	//	public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
	//	{
	//		var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(type, options);

	//		return typeInfo;
	//	}
	//}
}
