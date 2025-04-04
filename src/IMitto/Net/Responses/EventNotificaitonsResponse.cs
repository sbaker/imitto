using IMitto.Net.Models;

namespace IMitto.Net.Responses;

public class EventNotificationsResponse : TransmittoResponse<EventNotificationsBody>
{
	public EventNotificationsResponse(EventNotificationsBody? body = null, TransmittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
