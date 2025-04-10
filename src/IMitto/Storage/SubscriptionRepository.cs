namespace IMitto.Storage;

public abstract class SubscriptionRepository : ISubscriptionRepository
{
	/// <inheritdoc />
	public abstract void Add(ISubscription subscription);

	/// <inheritdoc />
	public abstract bool Remove(ISubscription subscription);

	/// <inheritdoc />
	public abstract IReadOnlyList<ISubscription> Get(Guid eventAggregatorId, EventId eventId);

	/// <inheritdoc />
	public abstract IEnumerable<ISubscription> GetAll(Guid eventAggregatorId);
}