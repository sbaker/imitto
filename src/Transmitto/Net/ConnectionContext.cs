
namespace Transmitto.Net;

public class ConnectionContext(IEventAggregator eventAggregator, TransmittoSocket socket)
{
	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public IEventAggregator EventAggregator { get; } = eventAggregator;
	
	public TransmittoSocket Socket { get; } = socket;
	public Task Task { get; set; }
}
