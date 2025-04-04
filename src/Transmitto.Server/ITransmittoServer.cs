using Transmitto.Net.Server;

namespace Transmitto.Server;

public interface ITransmittoServer : IDisposable
{
	TransmittoServerOptions Options { get; }

	string Name { get; }

	bool Started { get; }

	Task StartAsync(CancellationToken? token = null);

	Task RunAsync(CancellationToken token = default);

	Task StopAsync(CancellationToken token = default);
}
