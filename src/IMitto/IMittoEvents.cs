namespace IMitto;

public interface IMittoEvents : IDisposable, IAsyncDisposable
{
	Guid MittoEventsId { get; set; }

	MittoEventsOptions Options { get; }

	bool Subscribe(ISubscription subscription);

	ISubscription Subscribe(EventId eventId, Func<IMittoEvents, EventId, ISubscription> factory);

	Task<ISubscription> SubscribeAsync(ISubscription subscription);

	Task<ISubscription> SubscribeAsync(EventId eventId, Func<IMittoEvents, EventId, CancellationToken, Task<ISubscription>> factory, CancellationToken token);

	bool Unsubscribe(ISubscription subscription);

	void Publish<TData>(EventId eventId, TData data);

	Task PublishAsync<TData>(EventId eventId, TData data);
}