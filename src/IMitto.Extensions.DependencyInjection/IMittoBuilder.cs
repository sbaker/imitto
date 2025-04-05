using IMitto.Net.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
	public interface IMittoBuilder
	{
		IServiceCollection Services { get; }

		void AddTopicTypeMapping<TMapping>(string topic, TopicMappingType mappingType) where TMapping : class;

		IServiceCollection Build();
	}
}