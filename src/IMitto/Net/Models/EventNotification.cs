namespace IMitto.Net.Models;

public class EventNotification
{
	public string Id { get; set; }

	public MittoEventType Type { get; set; }

	public string Topic { get; set; }

	public MittoEvent Event { get; set; }
}
