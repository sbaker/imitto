using IMitto.Server;

namespace IMitto.Net;

public class ConnectionContext : Disposable
{
	public ConnectionContext(IEventAggregator eventAggregator, MittoSocket socket, CancellationToken token)
	{
		EventAggregator = eventAggregator;
		Socket = socket;
		CancellationToken = token;

		token.Register(Dispose);

		eventAggregator.Publish(ServerEventConstants.ConnectionReceivedEvent, this);
	}

	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public IEventAggregator EventAggregator { get; }
	
	public MittoSocket Socket { get; }

	public CancellationToken CancellationToken { get; set; }

	public Task? BackgroundTask { get; set; }

	protected override void DisposeCore()
	{
		Socket.Dispose();
		BackgroundTask?.Wait();
	}
}
