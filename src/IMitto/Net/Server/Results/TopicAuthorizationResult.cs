namespace IMitto.Net.Server.Results;

public class TopicAuthorizationResult
{
	public static readonly TopicAuthorizationResult Success = new TopicAuthorizationResult
	{
		IsAuthorized = true,
	};

	public Dictionary<string, object> AccessAuthorizationDetails { get; } = [];

	public bool IsAuthorized { get; private set; }
}