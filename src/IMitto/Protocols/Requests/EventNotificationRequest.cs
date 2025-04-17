using IMitto.Net;
using IMitto.Protocols.Models;

namespace IMitto.Protocols.Requests;

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