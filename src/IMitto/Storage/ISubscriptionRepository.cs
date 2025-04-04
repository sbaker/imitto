namespace IMitto.Storage;

    public interface ISubscriptionRepository
    {
        /// <summary>
        /// Adds a subscription to the store.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="subscription"></param>
        public void Add(EventId eventId, ISubscription subscription);

        /// <summary>
        /// Removes a subscription from the store.
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public bool Remove(ISubscription subscription);

        /// <summary>
        /// Gets the subscriptions for the specified event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public IReadOnlyList<ISubscription> Get(EventId eventId);

        /// <summary>
        /// Gets all the subscriptions in the store.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ISubscription> GetAll();
    }