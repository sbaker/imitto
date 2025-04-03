using Transmitto.Net;

namespace Transmitto.Server;

internal class ReadSocketSubscription : Subscription
{
	private ConnectionContext _context;

	public ReadSocketSubscription(EventId topic, IEventAggregator eventAggregator, ConnectionContext context) : base(topic, eventAggregator)
	{
		_context = context;
	}

	protected override void InvokeCore(EventContext context)
	{
		//_context.HandleDataReceived(context);
	}
}