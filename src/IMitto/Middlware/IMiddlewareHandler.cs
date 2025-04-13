namespace IMitto.Middlware;

public interface IMiddlewareHandler
{
	Task HandleAsync(MiddlewareContext context, CancellationToken token);
}

public interface IMiddlewareHandler<TContext>
{
	Task HandleAsync(TContext context, CancellationToken token);
}
