namespace Transmitto.Net.Clients;

public interface ITransmittoClient : IDisposable
{
	Task RunAsync(CancellationToken token);
}
