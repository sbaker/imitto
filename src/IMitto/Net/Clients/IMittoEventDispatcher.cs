﻿using IMitto.Protocols.Models;

namespace IMitto.Net.Clients;

public interface IMittoEventDispatcher
{
	ValueTask DispatchAsync(EventNotificationsModel package);
}
