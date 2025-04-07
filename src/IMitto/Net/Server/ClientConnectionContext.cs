namespace IMitto.Net.Server;

public class ClientConnectionContext
{
	public ClientConnectionContext(ConnectionContext connection, Task eventLoopTask, CancellationToken token)
	{
		Connection = connection;
		EventLoopTask = eventLoopTask;
	}

	public ConnectionContext Connection { get; private set; }

	public Task EventLoopTask { get; private set; }
}