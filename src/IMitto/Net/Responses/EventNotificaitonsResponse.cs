using IMitto.Net.Models;

namespace IMitto.Net.Responses;

public class EventNotificationsResponse : MittoResponse<EventNotificationsBody>
{
	public EventNotificationsResponse(EventNotificationsBody? body = null, MittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
