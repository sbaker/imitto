using System.Text.Json.Nodes;

namespace IMitto.Settings;

public class TopicPackageTypeMapping(Type type, string topic, TopicMappingType mappingType)
{
	public bool IsProducer => (MappingType & TopicMappingType.Producer) == TopicMappingType.Producer;

	public bool IsConsumer => (MappingType & TopicMappingType.Consumer) == TopicMappingType.Consumer;

	public TopicMappingType MappingType { get; set; } = mappingType;

	public Type PackageType { get; } = type;

	public string Topic { get; } = topic;
	public JsonNode TopicJsonSchema { get; set; }
}
