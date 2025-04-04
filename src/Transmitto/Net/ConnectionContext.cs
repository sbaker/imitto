using Transmitto.Server;

namespace Transmitto.Net;

public class ConnectionContext : Disposable
{
	public ConnectionContext(IEventAggregator eventAggregator, TransmittoSocket socket, ITransmittoEventListener eventListener, CancellationToken token)
	{
		EventAggregator = eventAggregator;
		Socket = socket;
		EventListener = eventListener;
		CancellationToken = token;

		token.Register(Dispose);

		eventAggregator.Publish(ServerEventConstants.ConnectionReceivedEvent, this);
	}

	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public IEventAggregator EventAggregator { get; }
	
	public TransmittoSocket Socket { get; }

	public ITransmittoEventListener EventListener { get; private set; }

	public CancellationToken CancellationToken { get; set; }

	public Task? BackgroundTask { get; set; }

	protected override void DisposeCore()
	{
		Socket.Dispose();
		BackgroundTask?.Wait();
	}
}
