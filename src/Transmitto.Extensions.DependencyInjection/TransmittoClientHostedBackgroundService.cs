using Microsoft.Extensions.Hosting;
using Transmitto.Net.Clients;

namespace Transmitto.Extensions.DependencyInjection;

internal class TransmittoClientHostedBackgroundService(ITransmittoClient client) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await client.RunAsync(stoppingToken);
	}
}

