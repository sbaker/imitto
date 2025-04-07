using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Channels;
using IMitto;
using IMitto.Channels;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net;
using IMitto.Net.Clients;
using IMitto.Net.Models;
using IMitto.Net.Server;
using IMitto.Local;
using IMitto.Storage;
using IMitto.Settings;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLocalEvents(this IServiceCollection services, Action<MittoEventsOptions>? configure = null)
	{
		return services.AddLocalEvents<MittoLocalEvents>(configure)
			.AddSingleton<IMittoEvents>(s => s.GetRequiredService<IMittoLocalEvents>());
	}

	public static IServiceCollection AddLocalEvents<TEventAggregator>(this IServiceCollection services, Action<MittoEventsOptions>? configure = null)
		where TEventAggregator : class, IMittoLocalEvents
	{
		configure ??= options => { };
		return services.Configure(configure)
			.AddSingleton<IMittoLocalEvents, TEventAggregator>();
	}

	public static IServiceCollection AddIMittoChannels<TChannelModel>(this IServiceCollection services, Action<MittoBoundedChannelOptions> configure)
	{
		services.Configure(configure);
		services.TryAddSingleton<IMittoChannelReaderProvider<TChannelModel>>(
			s => s.GetRequiredService<IMittoChannelProvider<TChannelModel>>());
		services.TryAddSingleton<IMittoChannelWriterProvider<TChannelModel>>(
			s => s.GetRequiredService<IMittoChannelProvider<TChannelModel>>());
		services.TryAddSingleton<IMittoChannelProvider<TChannelModel>, MittoBoundedChannelProvider<TChannelModel>>();
		return services;
	}

	public static IServiceCollection AddIMitto(this IServiceCollection services, Action<IMittoClientBuilder> configure)
	{
		var builder = new MittoBuilder(services);

		configure(builder);

		return builder.Build()
			.AddIMittoChannels<EventNotificationsModel>(_ => { })
			.AddSingleton<IMittoClientEventManager, MittoClientEventManager>()
			.AddSingleton<IMittoEventDispatcher, ChannelMittoEventDispatcher>();
	}

	public static IServiceCollection AddIMittoServer(this IServiceCollection services, Action<MittoServerOptions>? configure = null)
	{
		return services.Configure(configure ??= options => { })
			.AddIMittoChannels<ConnectionContext>(options => {
				options.ChannelFullMode = BoundedChannelFullMode.Wait;
			})
			.AddIMittoChannels<ClientNotificationModel>(options => {
				options.ChannelFullMode = BoundedChannelFullMode.Wait;
			})
			.AddHostedService<MittoServerHostedBackgroundService>()
			.AddSingleton<IServerEventManager, ServerEventManager>()
			.AddSingleton<IMittoRequestHandler, MittoServerRequestHandler>()
			.AddSingleton<IMittoEventListener, MittoEventListener>()
			.AddSingleton<IMittoAuthenticationHandler, NullAuthenticationHandler>()
			.AddSingleton<IMittoServer, MittoServer>();
	}

	public static IServiceCollection AddIMittoServer<TRepository>(this IServiceCollection services, Action<MittoServerOptions>? configure = null)
		where TRepository : class, ISubscriptionRepository
	{
		return services.AddIMittoServer(configure)
			.AddSingleton<ISubscriptionRepository, TRepository>();
	}

	internal class MittoBuilder(IServiceCollection services) : IMittoClientBuilder
	{
		private readonly Dictionary<string, TopicPackageTypeMapping> _mappings = [];

		public IServiceCollection Services { get; } = services;

		public void AddTopicTypeMapping<TMapping>(string topic, TopicMappingType mappingType) where TMapping : class
		{
			TopicPackageTypeMapping? typeMapping = null;

			if (_mappings.TryGetValue(topic, out var value))
			{
				typeMapping = value;
				typeMapping.MappingType |= mappingType;
			}

			_mappings[topic] = typeMapping ?? new TopicPackageTypeMapping(typeof(TMapping), topic, mappingType);
		}

		public IServiceCollection Build()
		{
			return Services.Configure<MittoClientOptions>(options => {
				foreach (var mapping in _mappings)
				{
					options.TypeMappings.AddOrUpdate(mapping.Key, mapping.Value, (s, t) => mapping.Value);
				}
			})
			.AddHostedService<MittoClientHostedBackgroundService>()
			.AddSingleton<IMittoClient, MittoClient>();
		}
	}
}