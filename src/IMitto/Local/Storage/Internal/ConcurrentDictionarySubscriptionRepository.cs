using IMitto.Storage;
using System.Collections.Concurrent;

namespace IMitto.Local.Storage.Internal;

internal sealed class ConcurrentDictionarySubscriptionRepository : SubscriptionRepository
{
    private ConcurrentDictionary<EventId, List<ISubscription>> Subscriptions { get; } = new();

	/// <inheritdoc />
	public sealed override void Add(ISubscription subscription)
    {
        Subscriptions.AddOrUpdate(subscription.EventId, [subscription], (k, list) => {
            list.Add(subscription);
            return list;
        });
    }

	/// <inheritdoc />
	public sealed override bool Remove(ISubscription subscription)
        => Subscriptions.TryGetValue(subscription.EventId, out var subscriptions)
        && subscriptions.Remove(subscription);

	/// <inheritdoc />
	public sealed override IEnumerable<ISubscription> GetAll(Guid eventAggregatorId)
        => Subscriptions.Values.SelectMany(x => x.Where(s => s.EventAggregatorId == eventAggregatorId));

	/// <inheritdoc />
	public sealed override IReadOnlyList<ISubscription> Get(Guid eventAggregatorId, EventId eventId)
    {
        var subscribers = Subscriptions.GetOrAdd(eventId, static _ => {
            return []; 
        });

        return [.. subscribers];
    }
}