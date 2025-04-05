namespace IMitto;

public class EventContext(EventId eventId, IMittoEvents aggregator)
{
	private readonly object? _data = null;

	public EventId EventId { get; } = eventId;

	public IMittoEvents Aggregator { get; } = aggregator;

	public virtual Type GetDataType() => _data?.GetType() ?? typeof(object);

	public virtual object? GetData()
	{
		return _data;
	}
}

public class EventContext<T>(EventId eventId, IMittoEvents aggregator, T data) : EventContext(eventId, aggregator)
{
	private static readonly Type DataType = typeof(T);

	public T Data { get; } = data;

	public override Type GetDataType() => DataType;

	public override object? GetData()
	{
		return Data;
	}
}