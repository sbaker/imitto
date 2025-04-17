namespace IMitto.Net;

public interface IMittoConnection : IDisposable
{
	bool IsConnected();

	Task ConnectAsync(CancellationToken token = default);

	Task CloseAsync(CancellationToken token = default);
}
