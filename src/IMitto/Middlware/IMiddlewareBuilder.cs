namespace IMitto.Middlware;

public interface IMiddlewareBuilder
{
	IMiddlewareBuilder Add(MiddlewareBuilderFunc action);

	IMiddlewareBuilder Add(IMiddlewareHandler handler);

	IMiddlewareHandler Build();
}

public interface IMiddlewareBuilder<TContext>
{
	IMiddlewareBuilder<TContext> Add(MiddlewareBuilderFunc<TContext> action);

	IMiddlewareBuilder<TContext> Add(IMiddlewareHandler<TContext> handler);

	IMiddlewareHandler<TContext> Build();
}
