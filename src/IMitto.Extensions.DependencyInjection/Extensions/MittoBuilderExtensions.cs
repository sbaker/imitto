using IMitto.Consumers;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net.Clients;
using IMitto.Net.Settings;
using IMitto.Producers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

public static class MittoBuilderExtensions
{
	public static IMittoBuilder AddConsumer<TConsumer, TPackage>(this IMittoBuilder builder, string topic)
		where TConsumer : class, IMittoPackageConsumer<TPackage>
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic, TopicMappingType.Consumer);
		builder.Services.AddTransient<IMittoPackageConsumer<TPackage>, TConsumer>();

		return builder;
	}

	public static IMittoBuilder AddProducer<TProducer, TPackage>(this IMittoBuilder builder, string topic)
		where TProducer : class, IMittoPackageProducer<TPackage>
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic, TopicMappingType.Producer);
		builder.Services.AddTransient<IMittoPackageProducer<TPackage>, TProducer>();

		return builder;
	}

	public static IMittoBuilder Configure(this IMittoBuilder builder, Action<MittoClientOptions> configure)
	{
		builder.Services.Configure(configure);
		return builder;
	}
}
