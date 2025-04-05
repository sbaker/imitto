namespace IMitto.Local;

public sealed class ActionSubscription(EventId eventId, IEventAggregator aggregator, Action<EventContext> callback) : DelegateSubscription<Action<EventContext>>(eventId, aggregator, callback)
{
	protected override void InvokeCore(EventContext context)
	{
		Callback.Invoke(context);
	}
}

public class ActionSubscription<TData>(EventId eventId, IEventAggregator aggregator, Action<EventContext<TData>> callback) : DelegateSubscription<Action<EventContext<TData>>>(eventId, aggregator, callback)
{
	private static readonly Type DataType = typeof(TData);

	protected override void InvokeCore(EventContext context)
	{
		if (!context.GetDataType().IsAssignableTo(DataType))
		{
			throw new ArgumentException($"Invalid data type used to publish event expecting {DataType}. Invalid data type: {context.GetDataType()}");
		}

		if (context is EventContext<TData> newContext)
		{
			Callback.Invoke(newContext);
			return;
		}

		var data = context.GetData();

		if (data is TData newData)
		{
			Callback.Invoke(new EventContext<TData>(context.EventId, context.Aggregator, newData));
			return;
		}

		throw new InvalidOperationException("Data type mismatch.");
	}
}