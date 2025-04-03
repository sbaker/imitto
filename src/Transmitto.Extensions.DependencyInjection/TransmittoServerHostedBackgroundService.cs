using Microsoft.Extensions.Hosting;
using Transmitto.Server;

namespace Transmitto.Extensions.DependencyInjection;

internal class TransmittoServerHostedBackgroundService(ITransmittoServer server) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await server.RunAsync(stoppingToken);
	}
}