using IMitto.Net.Models;
using IMitto.Net.Server;
using IMitto.Net;
using IMitto.Storage;
using System.Threading.Channels;
using IMitto.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static partial class ServiceCollectionExtensions
{
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
}