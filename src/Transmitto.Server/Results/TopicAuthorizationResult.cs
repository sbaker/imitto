namespace Transmitto.Server.Results;

public class TopicAuthorizationResult
{
	public static readonly TopicAuthorizationResult Success = new TopicAuthorizationResult
	{
		IsAuthorized = true,
	};

	public Dictionary<string, string> AccessAuthorizationDetails { get; private set; } = [];

	public bool IsAuthorized { get; private set; }
}