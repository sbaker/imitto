using IMitto.Net.Server;

namespace IMitto.Server;

public interface IMittoServer : IDisposable
{
	TransmittoServerOptions Options { get; }

	string Name { get; }

	bool Started { get; }

	Task StartAsync(CancellationToken? token = null);

	Task RunAsync(CancellationToken token = default);

	Task StopAsync(CancellationToken token = default);
}
