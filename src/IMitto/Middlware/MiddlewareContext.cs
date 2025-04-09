namespace IMitto.Middlware;

public class MiddlewareContext
{
	public string ConnectionId { get; } = Guid.NewGuid().ToString();

	public DateTime CreatedAt { get; } = DateTime.UtcNow;

	public TimeSpan RunningTimespan => DateTime.UtcNow - CreatedAt;
}

public class MiddlewareContext<TState>(TState state) : MiddlewareContext
{
	public TState State { get; } = state;
}
