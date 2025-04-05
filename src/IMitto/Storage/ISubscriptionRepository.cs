namespace IMitto.Storage;

public interface ISubscriptionRepository
{
	/// <summary>
	/// Adds a subscription to the store.
	/// </summary>
	/// <param name="eventId"></param>
	/// <param name="subscription"></param>
	public void Add(ISubscription subscription);

	/// <summary>
	/// Removes a subscription from the store.
	/// </summary>
	/// <param name="subscription"></param>
	/// <returns></returns>
	public bool Remove(ISubscription subscription);

	/// <summary>
	/// Gets the subscriptions associated with the <paramref name="eventAggregatorId"/> for the specified event.
	/// </summary>
	/// <param name="eventId"></param>
	/// <returns></returns>
	public IReadOnlyList<ISubscription> Get(Guid eventAggregatorId, EventId eventId);

	/// <summary>
	/// Gets all the subscriptions in the store associated with the provided <paramref name="eventAggregatorId"/>.
	/// </summary>
	/// <returns></returns>
	public IEnumerable<ISubscription> GetAll(Guid eventAggregatorId);
}