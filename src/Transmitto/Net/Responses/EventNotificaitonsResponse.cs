using Transmitto.Net.Models;

namespace Transmitto.Net.Responses;

public class EventNotificationsResponse : TransmittoResponse<EventNotificationsBody>
{
	public EventNotificationsResponse(EventNotificationsBody? body = null, TransmittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
