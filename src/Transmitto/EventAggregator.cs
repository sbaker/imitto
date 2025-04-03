using Transmitto.Extensions;
using Transmitto.Storage;

namespace Transmitto;

public class EventAggregator(ISubscriptionRepository? repository = null) : IEventAggregator
{
	protected ISubscriptionRepository Repository { get; } = repository ?? SubscriberDefaults.InMemory;

	public ISubscription Subscribe(EventId eventId, Action<EventContext> callback)
	{
		return SubscribeCore(new ActionSubscription(eventId, this, callback));
	}

	public virtual ISubscription Subscribe<T>(EventId eventId, Action<EventContext<T>> callback)
	{
		return SubscribeCore(new ActionSubscription<T>(eventId, this, callback));
	}

	public ISubscription Subscribe(EventId eventId, Func<IEventAggregator, EventId, ISubscription> registerFactory)
	{
		var subscription = registerFactory.Invoke(this, eventId);
		return SubscribeCore(subscription);
	}

	public virtual bool Unsubscribe(ISubscription subscription)
	{
		return Repository.Remove(subscription);
	}

	public virtual void Publish<TData>(EventId eventId, TData data)
	{
		if (!Repository.TryGet(eventId, out var subscriptions))
		{
			return;
		}

		var context = new EventContext<TData>(eventId, this, data);

		foreach (var subscription in subscriptions)
		{
			subscription.Invoke(context);
		}
	}

	public Task PublishAsync<TData>(EventId eventId, TData data)
	{
		return Task.Run(() => Publish(eventId, data));
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsyncCore();
		GC.SuppressFinalize(this);
	}

	protected virtual ISubscription SubscribeCore(ISubscription subscription)
	{
		Repository.Add(subscription.EventId, subscription);

		return subscription;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			DisposeSubscriptions();
		}
	}

	protected virtual ValueTask DisposeAsyncCore()
	{
		DisposeSubscriptions();
		return ValueTask.CompletedTask;
	}

	private void DisposeSubscriptions()
	{
		var subscriptions = Repository.GetAll().ToArray();

		for (int i = 0; i < subscriptions.Length; i++)
		{
			var subscription = subscriptions[i];
			subscription.Dispose();
			Repository.Remove(subscription);
		}
	}
}
