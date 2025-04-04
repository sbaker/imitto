namespace IMitto.Net;

public interface IMittoConnection : IDisposable
{
	bool IsConnected();
}
