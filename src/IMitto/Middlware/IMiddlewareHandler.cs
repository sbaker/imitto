namespace IMitto.Middlware;

public interface IMiddlewareHandler
{
	Task HandleAsync(MiddlewareContext context, CancellationToken token);
}

public interface IMiddlewareHandler<T>
{
	Task HandleAsync(MiddlewareContext<T> context, CancellationToken token);
}

//public interface IMiddlewareChainHandler<T>
//{
//	Task HandleAsync(MiddlewareAction<T> next, MiddlewareContext<T> context, CancellationToken token);
//}
