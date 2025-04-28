namespace IMitto.Protocols;

public static class MittoHeaderKeys
{
	public static readonly Dictionary<MittoHeaderKey, string> HeaderIdToKey = new()
	{
		{ MittoHeaderKey.ContentType, "content-type" },
		{ MittoHeaderKey.Authorization, "authorization" },
		{ MittoHeaderKey.MessageId, "message-id" },
		{ MittoHeaderKey.Timestamp, "timestamp" },
		{ MittoHeaderKey.Topic, "topics" },
		{ MittoHeaderKey.CorrelationId, "correlation-id" },
		{ MittoHeaderKey.PackageEncoding, "package-encoding" },
		{ MittoHeaderKey.Ttl, "ttL" },
		{ MittoHeaderKey.ClientVersion, "client-version" },
		{ MittoHeaderKey.ServerVersion, "server-version" },
		{ MittoHeaderKey.SessionId, "session-id" },
		{ MittoHeaderKey.StatusCode, "status-code" },
	};

	public static readonly Dictionary<string, MittoHeaderKey> KeyToHeaderId = new(
		HeaderIdToKey.ToDictionary(kvp => kvp.Value, kvp => kvp.Key), StringComparer.OrdinalIgnoreCase
	);
}