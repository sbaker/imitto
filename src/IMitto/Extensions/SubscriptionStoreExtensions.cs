using IMitto.Storage;

namespace IMitto.Extensions;

    public static class SubscriptionStoreExtensions
    {
        public static bool TryGet(this ISubscriptionRepository store, Guid eventAggregatorId, EventId eventId, out IReadOnlyList<ISubscription> subscriptions)
        {
            subscriptions = store.Get(eventAggregatorId, eventId);

            return subscriptions.Any();
        }
    }