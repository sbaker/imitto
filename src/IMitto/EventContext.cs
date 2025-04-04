namespace IMitto;

    public class EventContext(EventId eventId, IEventAggregator aggregator)
    {
        public EventId EventId { get; } = eventId;

        public IEventAggregator Aggregator { get; } = aggregator;

        public virtual Type GetDataType() => typeof(object);

        public virtual object? GetData()
        {
            return null;
        }
    }

    public class EventContext<T>(EventId eventId, IEventAggregator aggregator, T data) : EventContext(eventId, aggregator)
    {
        private static readonly Type DataType = typeof(T);

        public T Data { get; } = data;

        public override Type GetDataType() => DataType;

        public override object? GetData()
        {
            return Data;
        }
    }