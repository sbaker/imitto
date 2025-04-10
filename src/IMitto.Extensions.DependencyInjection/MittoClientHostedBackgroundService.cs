using Microsoft.Extensions.Hosting;
using IMitto.Net.Clients;

namespace IMitto.Extensions.DependencyInjection;

internal class MittoClientHostedBackgroundService(IMittoClient client) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await client.RunAsync(stoppingToken);
	}
}

