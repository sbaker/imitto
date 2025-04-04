namespace Transmitto.Storage;

public abstract class SubscriptionRepository : ISubscriptionRepository
{
	/// <inheritdoc />
	public abstract void Add(EventId eventId, ISubscription subscription);

	/// <inheritdoc />
	public abstract bool Remove(ISubscription subscription);

	/// <inheritdoc />
	public abstract IReadOnlyList<ISubscription> Get(EventId eventId);

	/// <inheritdoc />
	public abstract IEnumerable<ISubscription> GetAll();
}