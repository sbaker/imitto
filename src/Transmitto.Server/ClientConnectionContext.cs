using Transmitto.Net;

namespace Transmitto.Server;

public class ClientConnectionContext
{
	public ClientConnectionContext(ConnectionContext connection, Task eventLoopTask)
	{
		Connection = connection;
		EventLoopTask = eventLoopTask;
	}

	public ConnectionContext Connection { get; private set; }

	public Task EventLoopTask { get; private set; }
}