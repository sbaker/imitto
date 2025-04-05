using Microsoft.Extensions.Hosting;
using IMitto.Net.Server;

namespace IMitto.Extensions.DependencyInjection;

internal class MittoServerHostedBackgroundService(IMittoServer server) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await server.RunAsync(stoppingToken);
	}
}