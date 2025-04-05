using IMitto.Extensions;
using IMitto.Local.Storage.Internal;
using IMitto.Storage;
using Microsoft.Extensions.Options;
using Opt = Microsoft.Extensions.Options.Options;

namespace IMitto;

public class MittoEvents(IOptions<MittoEventsOptions> options, ISubscriptionRepository? repository = null) : Disposable, IMittoEvents
{
	public MittoEvents() : this(Opt.Create(new MittoEventsOptions()))
	{
	}

	private readonly MittoEventsOptions _options = options.Value;

	private Guid? _id = null;

	public Guid MittoEventsId { get => _id ??= Guid.CreateVersion7(); set => _id = value; }

	protected ISubscriptionRepository Repository { get; } = repository ?? SubscriberDefaults.InMemoryRepository;

	public MittoEventsOptions Options => _options;

	public virtual void Publish<TData>(EventId eventId, TData data)
	{
		if (!Repository.TryGet(MittoEventsId, eventId, out var subscriptions))
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

	public ISubscription Subscribe(EventId eventId, Func<IMittoEvents, EventId, ISubscription> factory)
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

	public virtual async Task<ISubscription> SubscribeAsync(EventId eventId, Func<IMittoEvents, EventId, CancellationToken, Task<ISubscription>> factory, CancellationToken token)
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
			subscription.EventAggregatorId = MittoEventsId;
		}

		if (subscription.EventAggregatorId != MittoEventsId)
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
		var subscriptions = Repository.GetAll(MittoEventsId).ToArray();

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
