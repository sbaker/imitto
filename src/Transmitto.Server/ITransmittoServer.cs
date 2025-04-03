using Transmitto.Net.Server;

namespace Transmitto.Server;

public interface ITransmittoServer
{
	TransmittoServerOptions Options { get; }

	string Name { get; }

	Task StartAsync(CancellationToken? token = null);

	Task RunAsync(CancellationToken token = default);

	Task StopAsync(CancellationToken token = default);
}
