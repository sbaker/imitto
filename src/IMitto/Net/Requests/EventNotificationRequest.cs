using IMitto.Net.Models;

namespace IMitto.Net.Requests;

public class EventNotificationRequest : MittoRequest<EventNotificationsBody>
{
	public EventNotificationRequest()
	{
	}

	public EventNotificationRequest(EventNotificationsBody body)
	{
		Body = body;
		Header = new MittoHeader
		{
			Path = MittoPaths.Topics,
			Action = MittoEventType.Produce
		};
	}
}