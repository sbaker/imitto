namespace IMitto.Settings;

public class TopicPackageTypeMapping(Type type, string topic, TopicMappingType mappingType)
{
	public TopicMappingType MappingType { get; set; } = mappingType;

	public Type PackageType { get; } = type;

	public string Topic { get; } = topic;
}
