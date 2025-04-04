using Transmitto.Net.Models;

namespace Transmitto.Net.Clients;

public interface ITransmittoEventDispatcher
{
	ValueTask DispatchAsync(EventNotificationsModel notifications);
}
