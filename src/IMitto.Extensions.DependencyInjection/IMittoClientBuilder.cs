using IMitto.Consumers;
using IMitto.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace IMitto.Extensions.DependencyInjection;

public interface IMittoClientBuilder
{
	IServiceCollection Services { get; }

	void AddProducerMapping<TMapping>(string topic) where TMapping : class;

	void AddTopicTypeMapping<TConsumer, TPackage>(string topic, TopicMappingType mappingType)
		where TConsumer : class, IMittoPackageConsumer<TPackage>
		where TPackage : class;
	IServiceCollection Build();
}
