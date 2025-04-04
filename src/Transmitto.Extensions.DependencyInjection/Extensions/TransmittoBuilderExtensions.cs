using Transmitto.Consumers;
using Transmitto.Producers;
using Transmitto.Net.Clients;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

public static class TransmittoBuilderExtensions
{
	public static ITransmittoBuilder AddConsumer<TConsumer, TPackage>(this ITransmittoBuilder builder, string topic)
		where TConsumer : class, ITransmittoMessageConsumer<TPackage>
		where TPackage : class
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TPackage>(topic);
		builder.Services.AddTransient<ITransmittoMessageConsumer<TPackage>, TConsumer>();

		return builder;
	}

	public static ITransmittoBuilder Configure(this ITransmittoBuilder builder, Action<TransmittoClientOptions> configure)
	{
		builder.Services.Configure(configure);
		return builder;
	}
}
