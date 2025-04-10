using IMitto.Channels;
using Microsoft.Extensions.DependencyInjection.Extensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static partial class ServiceCollectionExtensions
{
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
}