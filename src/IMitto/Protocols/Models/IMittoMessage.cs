﻿using IMitto.Settings;

namespace IMitto.Protocols.Models;

public interface IMittoMessage
{
	MittoHeader Header { get; set; }

	bool HasBody();

	MittoMessageBody GetBody();

	TBody GetBody<TBody>(MittoJsonOptions? options = null);
}

public interface IMittoMessage<TBody> : IMittoMessage where TBody : MittoMessageBody
{
	TBody Body { get; set; }
}
