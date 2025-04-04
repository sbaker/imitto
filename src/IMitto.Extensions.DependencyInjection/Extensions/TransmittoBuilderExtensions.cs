using IMitto.Consumers;
using IMitto.Producers;
using IMitto.Net.Clients;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

public static class TransmittoBuilderExtensions
{
	public static IMittoBuilder AddConsumer<TConsumer, TPackage>(this IMittoBuilder builder, string topic)
		where TConsumer : class, IMittoMessageConsumer<TPackage>
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic);
		builder.Services.AddTransient<IMittoMessageConsumer<TPackage>, TConsumer>();

		return builder;
	}

	public static IMittoBuilder Configure(this IMittoBuilder builder, Action<TransmittoClientOptions> configure)
	{
		builder.Services.Configure(configure);
		return builder;
	}
}
