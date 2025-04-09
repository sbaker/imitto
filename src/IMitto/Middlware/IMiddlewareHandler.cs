namespace IMitto.Middlware;

public interface IMiddlewareHandler
{
	Task HandleAsync(MiddlewareContext context, CancellationToken token);
}

public interface IMiddlewareHandler<T>
{
	Task HandleAsync(MiddlewareContext<T> context, CancellationToken token);
}
