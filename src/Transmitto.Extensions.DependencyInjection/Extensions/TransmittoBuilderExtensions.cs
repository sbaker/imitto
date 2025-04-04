using Transmitto.Handlers;
using Transmitto.Net.Clients;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

public static class TransmittoBuilderExtensions
{
	public static ITransmittoBuilder AddSubscriber<THandler, TMessage>(this ITransmittoBuilder builder, string topic)
		where TMessage : class
		where THandler : class, ITransmittoMessageHandler<TMessage>
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		builder.AddTopicTypeMapping<TMessage>(topic);
		builder.Services.AddTransient<ITransmittoMessageHandler<TMessage>, THandler>();

		return builder;
	}

	public static ITransmittoBuilder Configure(this ITransmittoBuilder builder, Action<TransmittoClientOptions> configure)
	{
		builder.Services.Configure(configure);
		return builder;
	}
}
