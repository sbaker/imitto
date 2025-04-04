namespace IMitto.Net.Clients;

public interface IMittoClient : IDisposable
{
	Task RunAsync(CancellationToken token);
}
