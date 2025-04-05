namespace IMitto.Net.Models;

public class MittoEvent
{
	public string RawMessage { get; set; }
}

public class MittoEvent<TMessage> : MittoEvent
{
	public TMessage? Message { get; set; }
}
