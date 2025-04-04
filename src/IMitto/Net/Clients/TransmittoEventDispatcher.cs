using Microsoft.Extensions.Logging;
using IMitto.Net.Models;

namespace IMitto.Net.Clients;

public abstract class TransmittoEventDispatcher : IMittoEventDispatcher
{
	private readonly ILogger _logger;

	protected TransmittoEventDispatcher(ILogger logger)
	{
		_logger = logger;
	}

	public abstract ValueTask DispatchAsync(EventNotificationsModel notifications);
}
