namespace IMitto;

public interface IEventAggregator : IDisposable, IAsyncDisposable
{
	Guid EventAggregatorId { get; set; }

	bool Subscribe(ISubscription subscription);

	ISubscription Subscribe(EventId eventId, Func<IEventAggregator, EventId, ISubscription> factory);

	Task<ISubscription> SubscribeAsync(ISubscription subscription);

	Task<ISubscription> SubscribeAsync(EventId eventId, Func<IEventAggregator, EventId, CancellationToken, Task<ISubscription>> factory, CancellationToken token);

	bool Unsubscribe(ISubscription subscription);

	void Publish<TData>(EventId eventId, TData data);

	Task PublishAsync<TData>(EventId eventId, TData data);
}