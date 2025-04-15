namespace IMitto.Protocols.Models;

public class MittoEvent
{
	public object? Package { get; set; }

	public string? Topic { get; set; }
}

public class MittoEvent<TMessage> : MittoEvent
{
	public MittoEvent()
	{
	}

	public MittoEvent(PackagedGoods<TMessage>? message)
	{
		Package = message;
	}
}
