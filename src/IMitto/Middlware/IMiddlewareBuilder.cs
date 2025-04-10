namespace IMitto.Middlware;

public interface IMiddlewareBuilder
{
	IMiddlewareBuilder Add(MiddlewareBuilderFunc action);

	IMiddlewareBuilder Add(IMiddlewareHandler handler);

	IMiddlewareHandler Build();
}

public interface IMiddlewareBuilder<T>
{
	IMiddlewareBuilder<T> Add(MiddlewareBuilderFunc<T> action);

	IMiddlewareBuilder<T> Add(IMiddlewareHandler<T> handler);

	IMiddlewareHandler<T> Build();
}
