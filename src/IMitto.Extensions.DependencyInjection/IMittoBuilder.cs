using IMitto.Net.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace IMitto.Extensions.DependencyInjection
{
	public interface IMittoBuilder
	{
		IServiceCollection Services { get; }

		void AddTopicTypeMapping<TMapping>(string topic, TopicMappingType mappingType) where TMapping : class;

		IServiceCollection Build();
	}
}