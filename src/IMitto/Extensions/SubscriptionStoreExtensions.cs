using IMitto.Storage;

namespace IMitto.Extensions;

    public static class SubscriptionStoreExtensions
    {
        public static bool TryGet(this ISubscriptionRepository store, EventId eventId, out IReadOnlyList<ISubscription> subscriptions)
        {
            subscriptions = store.Get(eventId);

            return subscriptions.Any();
        }
    }