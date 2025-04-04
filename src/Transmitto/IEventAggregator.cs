namespace Transmitto;

    public interface IEventAggregator : IDisposable, IAsyncDisposable
    {
        ISubscription Subscribe(EventId eventId, Action<EventContext> callback);

        ISubscription Subscribe<TData>(EventId eventId, Action<EventContext<TData>> callback);

        ISubscription Subscribe(EventId eventId, Func<IEventAggregator, EventId, ISubscription> registerFactory);

        bool Unsubscribe(ISubscription subscription);

        void Publish<TData>(EventId eventId, TData data);

        Task PublishAsync<TData>(EventId eventId, TData data);
  }