using Microsoft.Extensions.Logging;
using Transmitto.Net.Models;

namespace Transmitto.Net.Clients;

public abstract class TransmittoEventDispatcher : ITransmittoEventDispatcher
{
	private readonly ILogger _logger;

	protected TransmittoEventDispatcher(ILogger logger)
	{
		_logger = logger;
	}

	public abstract ValueTask DispatchAsync(EventNotificationsModel notifications);
}
