namespace IMitto.Middlware;

public sealed class MiddlewareCollection : List<MiddlewareBuilderFunc>, IMiddlewareHandler
{
	public MiddlewareCollection() : base()
	{
	}

	public MiddlewareCollection(IEnumerable<MiddlewareBuilderFunc> collection) : base(collection)
	{
	}

	public MiddlewareCollection(int capacity) : base(capacity)
	{
	}

	public Task HandleAsync(MiddlewareContext context, CancellationToken token)
	{
		return MiddlewareExecutor.ExecuteAsync(this, context, token).Await();
	}
}

public sealed class MiddlewareCollection<T> : List<MiddlewareBuilderFunc<T>>, IMiddlewareHandler<T>
{
	public MiddlewareCollection() : base()
	{
	}

	public MiddlewareCollection(IEnumerable<MiddlewareBuilderFunc<T>> collection) : base(collection)
	{
	}

	public MiddlewareCollection(int capacity) : base(capacity)
	{
	}

	public Task HandleAsync(MiddlewareContext<T> context, CancellationToken token)
	{
		return MiddlewareExecutor.ExecuteAsync(this, context, token).Await();
	}
}
