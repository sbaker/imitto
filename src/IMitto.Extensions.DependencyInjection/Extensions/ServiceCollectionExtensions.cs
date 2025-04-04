using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Channels;
using IMitto;
using IMitto.Channels;
using IMitto.Extensions.DependencyInjection;
using IMitto.Net;
using IMitto.Net.Clients;
using IMitto.Net.Models;
using IMitto.Net.Server;
using IMitto.Server;

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

	public static IServiceCollection AddChannels<TChannelModel>(this IServiceCollection services, Action<TransmittoBoundedChannelOptions> configure)
	{
		services.Configure(configure);
		services.TryAddSingleton<IMittoChannelReaderProvider<TChannelModel>>(
			s => s.GetRequiredService<IMittoChannelProvider<TChannelModel>>());
		services.TryAddSingleton<IMittoChannelWriterProvider<TChannelModel>>(
			s => s.GetRequiredService<IMittoChannelProvider<TChannelModel>>());
		services.TryAddSingleton<IMittoChannelProvider<TChannelModel>, TransmittoBoundedChannelProvider<TChannelModel>>();
		return services;
	}

	public static IServiceCollection AddTransmitto(this IServiceCollection services, Action<IMittoBuilder> configure)
	{
		var builder = new TransmittoBuilder(services);

		configure(builder);

		return builder.Build()
			.AddChannels<EventNotificationsModel>(_ => { })
			.AddSingleton<IMittoEventDispatcher, ChannelTransmittoEventDispatcher>();
	}

	public static IServiceCollection AddTransmittoServer(this IServiceCollection services, Action<TransmittoServerOptions>? configure = null)
	{
		return services.Configure(configure ??= options => { })
			.AddChannels<ConnectionContext>(options => {
				options.ChannelFullMode = BoundedChannelFullMode.Wait;
			})
			.AddChannels<ClientNotificationModel>(options => {
				options.ChannelFullMode = BoundedChannelFullMode.Wait;
			})
			.AddHostedService<TransmittoServerHostedBackgroundService>()
			.AddSingleton(p => SubscriberDefaults.InMemory)
			.AddSingleton<IServerEventManager, ServerEventManager>()
			.AddSingleton<IMittoRequestHandler, TransmittoServerRequestHandler>()
			.AddSingleton<IMittoEventListener, TransmittoEventListener>()
			.AddSingleton<IMittoAuthenticationHandler, NullAuthenticationHandler>()
			.AddSingleton<IMittoServer, TransmittoServer>();
	}

	internal class TransmittoBuilder(IServiceCollection services) : IMittoBuilder
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
			.AddSingleton<IMittoClient, TransmittoClient>();
		}
	}
}