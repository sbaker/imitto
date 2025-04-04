namespace Transmitto.Net.Models;

public class EventNotification
{
	public string Id { get; set; }

	public string Type { get; set; }

	public string Topic { get; set; }

	public TransmittoEvent Event { get; set; }
}
