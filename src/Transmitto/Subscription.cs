namespace Transmitto;

    public abstract class Subscription(EventId eventId, IEventAggregator aggregator) : ISubscription
    {
        public EventId EventId { get; protected set; } = eventId;

        protected IEventAggregator Aggregator { get; } = aggregator;

        public bool Released { get; protected set; }

        public int Invocations { get; protected set; }

        public bool Unsubscribe()
        {
            if (Aggregator.Unsubscribe(this))
            {
                Released = true;
            }

            return Released;
        }

        public void Publish<T>(T data)
        {
            Aggregator.Publish(EventId, data);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Invoke<T>(EventContext<T> context)
        {
            Increment();
            InvokeCore(context);
        }

        protected abstract void InvokeCore(EventContext context);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Released)
            {
                Unsubscribe();
            }
        }

        protected void Increment()
        {
            Invocations++;
        }
    }

    public class Subscription<TCallback>(EventId eventId, IEventAggregator aggregator, TCallback callback) : Subscription(eventId, aggregator) where TCallback : Delegate
    {
        protected TCallback Callback { get; } = callback;

        protected override void InvokeCore(EventContext context)
        {
            Callback.DynamicInvoke(context);
        }
    }
