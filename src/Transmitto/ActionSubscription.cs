namespace Transmitto;

    public sealed class ActionSubscription(EventId eventId, IEventAggregator aggregator, Action<EventContext> callback) : Subscription<Action<EventContext>>(eventId, aggregator, callback)
    {
        protected override void InvokeCore(EventContext context)
        {
            Callback.Invoke(context);
        }
    }

    public class ActionSubscription<TData>(EventId eventId, IEventAggregator aggregator, Action<EventContext<TData>> callback) : Subscription<Action<EventContext<TData>>>(eventId, aggregator, callback)
    {
        private static readonly Type DataType = typeof(TData);

        protected override void InvokeCore(EventContext context)
        {
            if (!context.GetDataType().IsAssignableTo(DataType))
            {
                throw new ArgumentException($"Invalid data type used to publish event expecting {DataType}. Invalid data type: {context.GetDataType()}");
            }

            var newContext = context as EventContext<TData>
                ?? new EventContext<TData>(context.EventId, context.Aggregator, (TData)context.GetData()!);

            Callback.Invoke(newContext);
        }
    }