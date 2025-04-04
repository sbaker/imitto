using Transmitto.Net;

namespace Transmitto.Server;

public class ClientConnectionContext
{
	public ClientConnectionContext(ConnectionContext connection)
	{
		Connection = connection;
	}

	public ConnectionContext Connection { get; }

	public CancellationTokenSource TokenSource { get; private set; }

	internal void StartEventLoopAsync(CancellationToken token)
	{
		TokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

		try
		{
			Connection.EventListener.PollForEventsAsync(Connection, TokenSource.Token);
		}
		catch (Exception)
		{

		}
	}
}