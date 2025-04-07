using IMitto.Storage;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class SubscriptionStoreExtensions
{
	public static bool TryGet(this ISubscriptionRepository store, Guid eventAggregatorId, EventId eventId, out IReadOnlyList<ISubscription> subscriptions)
	{
		subscriptions = store.Get(eventAggregatorId, eventId);

		return subscriptions.Any();
	}
}
