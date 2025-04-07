using IMitto.Consumers;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net.Clients;
using IMitto.Producers;
using IMitto.Settings;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MittoBuilderExtensions
{
	public static IMittoClientBuilder AddConsumer<TConsumer, TPackage>(this IMittoClientBuilder builder, string topic)
		where TConsumer : class, IMittoPackageConsumer<TPackage>
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic, TopicMappingType.Consumer);
		builder.Services.AddTransient<IMittoPackageConsumer<TPackage>, TConsumer>();

		return builder;
	}

	public static IMittoClientBuilder AddProducer<TPackage>(this IMittoClientBuilder builder, string topic)
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic, TopicMappingType.Producer);
		builder.Services.AddTransient<IMittoProducerProvider<TPackage>, MittoProducerProvider<TPackage>>();

		return builder;
	}

	public static IMittoClientBuilder Configure(this IMittoClientBuilder builder, Action<MittoClientOptions> configure)
	{
		builder.Services.Configure(configure);
		return builder;
	}
}
