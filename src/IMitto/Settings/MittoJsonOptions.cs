using System.Text.Json.Serialization;
using System.Text.Json;
using IMitto.Converters;
using System.Text.Json.Serialization.Metadata;
using IMitto.Protocols.Models;

namespace IMitto.Settings;

public class MittoJsonOptions
{
	public JsonSerializerOptions Serializer { get; set; } = new JsonSerializerOptions
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
		RespectNullableAnnotations = true,
		//TypeInfoResolver = new MittoTypeInfoResolver(),
		UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,//JsonIgnoreCondition.Never,
		PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
		Converters = {
			new JsonStringEnumConverter(),
			new MittoHeaderConverter(),
			//new EventNotificationsModelConverter(),
			new MittoEventNotificationsBodyConverter(),
			new MittoEventConverter(),
			new MittoBodyConverter<MittoMessageBody>(),
		}
	};

	private sealed class MittoTypeInfoResolver : IJsonTypeInfoResolver
	{
		public JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
		{
			var typeInfo = JsonTypeInfo.CreateJsonTypeInfo(type, options);

			return typeInfo;
		}
	}
}
