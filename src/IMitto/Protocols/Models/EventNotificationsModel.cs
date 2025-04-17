namespace IMitto.Protocols.Models;

public class EventNotificationsModel
{
	public string Topic { get; set; }

	public List<EventNotification> Events { get; set; }
}
