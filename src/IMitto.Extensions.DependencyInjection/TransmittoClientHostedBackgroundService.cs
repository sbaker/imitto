using Microsoft.Extensions.Hosting;
using IMitto.Net.Clients;

namespace IMitto.Extensions.DependencyInjection;

internal class TransmittoClientHostedBackgroundService(IMittoClient client) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await client.RunAsync(stoppingToken);
	}
}

