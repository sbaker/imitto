namespace IMitto.Middlware;

public class MiddlewareContext
{
	public MiddlewareContext(string? connectionId = null)
	{
		ConnectionId = connectionId ?? Guid.NewGuid().ToString();
	}

	public string ConnectionId { get; }

	public DateTime CreatedAt { get; } = DateTime.UtcNow;

	public TimeSpan RunningTimespan => DateTime.UtcNow - CreatedAt;
}
