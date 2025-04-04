using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Transmitto.Storage.Internal;

public class ConcurrentDictionarySubscriptionRepository : SubscriptionRepository
{
    protected ConcurrentDictionary<EventId, List<ISubscription>> Subscriptions { get; } = new();

    /// <inheritdoc />
    public override void Add(EventId eventId, ISubscription subscription)
    {
        Subscriptions.AddOrUpdate(eventId, [subscription], (k, list) => {
            list.Add(subscription);
            return list;
        });
    }

    /// <inheritdoc />
    public override bool Remove(ISubscription subscription)
        => Subscriptions.TryGetValue(subscription.EventId, out var subscriptions)
        && subscriptions.Remove(subscription);

    /// <inheritdoc />
    public override IEnumerable<ISubscription> GetAll() => Subscriptions.Values.SelectMany(x => x);

    /// <inheritdoc />
    public override IReadOnlyList<ISubscription> Get(EventId eventId)
    {
        var subscribers = Subscriptions.GetOrAdd(eventId, static _ => {
            return []; 
        });

        return [.. subscribers];
    }
}