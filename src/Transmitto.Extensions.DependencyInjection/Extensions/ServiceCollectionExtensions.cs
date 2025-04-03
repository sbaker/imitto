using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Channels;
using Transmitto;
using Transmitto.Channels;
using Transmitto.Extensions.DependencyInjection;
using Transmitto.Net;
using Transmitto.Net.Clients;
using Transmitto.Net.Server;
using Transmitto.Server;
using Transmitto.Storage;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLocalEvents(this IServiceCollection services)
	{
		return services.AddEvents<EventAggregator>();
	}

	public static IServiceCollection AddEvents<TEventAggregator>(this IServiceCollection services)
		where TEventAggregator : class, IEventAggregator
	{
		return services.AddSingleton<IEventAggregator, TEventAggregator>();
	}

	public static IServiceCollection AddChannels(this IServiceCollection services, Action<TransmittoBoundedChannelOptions> configure)
	{
		var boundedChannelType = typeof(TransmittoBoundedChannelProvider<>);
		services.Configure(configure);
		services.TryAddSingleton(typeof(ITransmittoChannelReaderProvider<>), boundedChannelType);
		services.TryAddSingleton(typeof(ITransmittoChannelWriterProvider<>), boundedChannelType);
		services.TryAddSingleton(typeof(ITransmittoChannelProvider<>), boundedChannelType);
		return services;
	}

	public static IServiceCollection AddTransmitto(this IServiceCollection services, Action<ITransmittoBuilder> configure)
	{
		var builder = new TransmittoBuilder(services);

		configure(builder);

		return builder.Build()
			.AddChannels(_ => {})
			.AddSingleton<ITransmittoEventDispatcher, ChannelTransmittoEventDispatcher>();
	}

	public static IServiceCollection AddTransmittoServer(this IServiceCollection services, Action<TransmittoServerOptions>? configure = null)
	{
		return services.Configure(configure ??= options => {})
			.AddHostedService<TransmittoServerHostedBackgroundService>()
			.AddSingleton(p => SubscriberDefaults.InMemory)
			.AddSingleton<IServerEventAggregator, ServerEventAggregator>()
			.AddSingleton<ITransmittoRequestHandler, TransmittoServerRequestHandler>()
			.AddSingleton<ITransmittoAuthenticationHandler, NullAuthenticationHandler>()
			.AddSingleton<ITransmittoServer, TransmittoServer>();
	}

	internal class TransmittoBuilder(IServiceCollection services) : ITransmittoBuilder
	{
		private Dictionary<string, Type> _mappings = new();

		public IServiceCollection Services { get; } = services;

		public void AddTopicTypeMapping<TMapping>(string topic) where TMapping : class
		{
			_mappings.Add(topic, typeof(TMapping));
		}

		public IServiceCollection Build()
		{
			return Services.Configure<TransmittoClientOptions>(options => {
				foreach (var mapping in _mappings)
				{
					options.TypeMappings.AddOrUpdate(mapping.Key, mapping.Value, (s, t) => mapping.Value);
				}
			})
			.AddHostedService<TransmittoClientHostedBackgroundService>()
			.AddSingleton<ITransmittoClient, TransmittoClient>();
		}
	}
}