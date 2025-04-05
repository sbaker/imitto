using Microsoft.Extensions.Options;

namespace IMitto.Local;

public class DelegateSubscription<TCallback>(EventId eventId, IMittoEvents aggregator, TCallback callback) : Subscription(eventId, aggregator) where TCallback : Delegate
{
	protected TCallback Callback { get; } = callback;

	protected override void InvokeCore(EventContext context)
	{
		Callback.DynamicInvoke(context);
	}
}
