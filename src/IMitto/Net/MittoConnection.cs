using IMitto.Net.Clients;
using IMitto.Net.Server;

namespace IMitto.Net;

public abstract class MittoConnection : Disposable, IMittoConnection
{
	protected MittoConnection()
	{
	}

	public static IMittoServerConnection CreateServer(MittoServerOptions options)
		=> new SingleMittoServerConnection(options);

	public static IMittoClientConnection CreateClient(MittoClientOptions options)
		=> new MittoClientConnection(options);
	
	public abstract bool IsConnected();
	
	public abstract Task ConnectAsync(CancellationToken token = default);
	
	public abstract Task CloseAsync(CancellationToken token = default);
}
