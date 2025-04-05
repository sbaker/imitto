using Microsoft.Extensions.Hosting;
using IMitto.Server;

namespace IMitto.Extensions.DependencyInjection;

internal class MittoServerHostedBackgroundService(IMittoServer server) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await server.RunAsync(stoppingToken);
	}
}