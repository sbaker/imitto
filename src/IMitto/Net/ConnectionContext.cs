using IMitto.Protocols;
using IMitto.Protocols.Models;

namespace IMitto.Net;

public class ConnectionContext : Disposable
{
	private Task? _backgroudTask;

	public ConnectionContext(IMittoEvents eventAggregator, MittoPipelineSocket socket)
	{
		EventAggregator = eventAggregator;
		Socket = socket;
	}

	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public IMittoEvents EventAggregator { get; }
	
	public MittoPipelineSocket Socket { get; }

	public TopicRegistrationModel? Topics { get; set; }

	protected override void DisposeCore()
	{
		Socket.Dispose();
	}
}
