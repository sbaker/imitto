using IMitto.Extensions;
using IMitto.Local.Storage.Internal;
using IMitto.Storage;

namespace IMitto;

public class EventAggregator(Guid? id = null, ISubscriptionRepository? repository = null) : Disposable, IEventAggregator
{
	private Guid? _id = id;

	public Guid EventAggregatorId { get => _id ??= Guid.CreateVersion7(); set => _id = value; }

	protected ISubscriptionRepository Repository { get; } = repository ?? SubscriberDefaults.InMemoryRepository;

	public virtual void Publish<TData>(EventId eventId, TData data)
	{
		if (!Repository.TryGet(EventAggregatorId, eventId, out var subscriptions))
		{
			return;
		}
		
		var context = CreateEventContext(eventId, data);

		foreach (var subscription in subscriptions)
		{
			subscription.Invoke(context);
		}
	}

	public virtual Task PublishAsync<TData>(EventId eventId, TData data)
	{
		return Task.Run(() => Publish(eventId, data));
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore(true);
		GC.SuppressFinalize(this);
	}

	public virtual bool Subscribe(ISubscription subscription)
	{
		SubscribeCore(subscription);
		return true;
	}

	public ISubscription Subscribe(EventId eventId, Func<IEventAggregator, EventId, ISubscription> factory)
	{
		ThrowIfDisposed();

		return SubscribeCore(
			factory.Invoke(this, eventId)
		);
	}

	public virtual Task<ISubscription> SubscribeAsync(ISubscription subscription)
	{
		return Task.Run(() => SubscribeCore(subscription));
	}

	public virtual async Task<ISubscription> SubscribeAsync(EventId eventId, Func<IEventAggregator, EventId, CancellationToken, Task<ISubscription>> factory, CancellationToken token)
	{
		ThrowIfDisposed();

		var subscriberTask = factory.Invoke(this, eventId, token);
		return SubscribeCore(await subscriberTask.ConfigureAwait(false));
	}

	public virtual bool Unsubscribe(ISubscription subscription)
	{
		return Repository.Remove(subscription);
	}

	protected virtual EventContext<TData> CreateEventContext<TData>(EventId eventId, TData data)
	{
		return new EventContext<TData>(eventId, this, data);
	}

	protected virtual ISubscription SubscribeCore(ISubscription subscription)
	{
		ThrowIfDisposed();

		if (!subscription.EventAggregatorId.HasValue)
		{
			subscription.EventAggregatorId = EventAggregatorId;
		}

		if (subscription.EventAggregatorId != EventAggregatorId)
		{
			throw new InvalidOperationException($"Subscription {subscription.SubscriptionId}");
		}

		Repository.Add(subscription);

		return subscription;
	}

	protected override void DisposeCore()
	{
		DisposeSubscriptions();
	}

	protected virtual ValueTask DisposeAsyncCore(bool disposing)
	{
		if (disposing && ! Disposed)
		{
			DisposeSubscriptions();
		}

		return ValueTask.CompletedTask;
	}

	private void DisposeSubscriptions()
	{
		var subscriptions = Repository.GetAll(EventAggregatorId).ToArray();

		for (int i = 0; i < subscriptions.Length; i++)
		{
			var subscription = subscriptions[i];

			subscription.Dispose();

			var repository = Repository;

			if (repository is ConcurrentDictionarySubscriptionRepository)
			{
				repository.Remove(subscription);
			}
		}
	}
}
