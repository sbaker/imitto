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
		return MiddlewareExecutor.ExecuteAsync(this, context, token);
	}
}

public sealed class MiddlewareCollection<TContext> : List<MiddlewareBuilderFunc<TContext>>, IMiddlewareHandler<TContext>
{
	public MiddlewareCollection() : base()
	{
	}

	public MiddlewareCollection(IEnumerable<MiddlewareBuilderFunc<TContext>> collection) : base(collection)
	{
	}

	public MiddlewareCollection(int capacity) : base(capacity)
	{
	}

	public Task HandleAsync(TContext context, CancellationToken token)
	{
		return MiddlewareExecutor.ExecuteAsync(this, context, token);
	}
}
