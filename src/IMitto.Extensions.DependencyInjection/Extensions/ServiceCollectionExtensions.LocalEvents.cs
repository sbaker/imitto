using IMitto;
using IMitto.Local;
using IMitto.Settings;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static partial class ServiceCollectionExtensions
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
}
