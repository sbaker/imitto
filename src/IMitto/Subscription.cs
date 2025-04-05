namespace IMitto;

public abstract class Subscription(EventId eventId, IMittoEvents aggregator) : Disposable, ISubscription
{
	private Guid? _id = null;

	public Guid SubscriptionId { get => _id ??= Guid.CreateVersion7(); set => _id = value; }

	public EventId EventId { get; protected set; } = eventId;

	protected IMittoEvents Aggregator { get; } = aggregator;

	public bool Released { get; protected set; }

	public int Invocations { get; protected set; }

	public Guid? EventAggregatorId { get; set; }

	public bool Unsubscribe()
	{
		if (Aggregator.Unsubscribe(this))
		{
			Released = true;
		}

		return Released;
	}

	public void Publish<T>(T data)
	{
		Aggregator.Publish(EventId, data);
	}

	public virtual void Invoke<T>(EventContext<T> context)
	{
		Increment();
		InvokeCore(context);
	}

	protected override void DisposeCore()
	{
		if (Aggregator.Options.SubscribersUnsubscribeOnDispose)
		{
			Unsubscribe();
		}
	}

	protected abstract void InvokeCore(EventContext context);

	protected void Increment()
	{
		Invocations++;
	}
}
