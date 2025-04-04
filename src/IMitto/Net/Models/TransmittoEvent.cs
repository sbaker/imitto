namespace IMitto.Net.Models;

public class TransmittoEvent
{
	public string RawMessage { get; set; }
}

public class TransmittoEvent<TMessage> : TransmittoEvent
{
	public TMessage? Message { get; set; }
}
