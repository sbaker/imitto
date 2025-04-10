using IMitto.Net.Clients;
using IMitto.Net.Server;

namespace IMitto.Net;

public abstract class MittoConnection : Disposable, IMittoConnection
{
	protected MittoConnection()
	{
	}

	public abstract bool IsConnected();

	public static IMittoServerConnection CreateServer(MittoServerOptions options)
		=> new SingleMittoServerConnection(options);

	public static IMittoClientConnection CreateClient(MittoClientOptions options)
		=> new MittoClientConnection(options);
}
