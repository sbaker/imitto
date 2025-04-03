
namespace Transmitto.Net;

public interface ITransmittoConnection : IDisposable
{
	bool IsConnected();
}
