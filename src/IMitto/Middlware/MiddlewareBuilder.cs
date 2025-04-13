namespace IMitto.Middlware;

public class MiddlewareBuilder : IMiddlewareBuilder
{
	protected readonly MiddlewareCollection middleware = [];

	public IMiddlewareBuilder Add(MiddlewareBuilderFunc action)
	{
		middleware.Add(action);
		return this;
	}

	public IMiddlewareBuilder Add(IMiddlewareHandler handler)
	{
		middleware.Add((next, context, token) => handler.HandleAsync(context, token));
		return this;
	}

	public IMiddlewareHandler Build()
	{
		return new RootMiddlewareHandler(middleware);
	}

	protected class RootMiddlewareHandler : IMiddlewareHandler
	{
		private readonly MiddlewareCollection _middleware;
		
		public RootMiddlewareHandler(MiddlewareCollection middleware)
		{
			_middleware = middleware;
		}
		
		public Task HandleAsync(MiddlewareContext context, CancellationToken token)
		{
			return _middleware.HandleAsync(context, token);
		}
	}
}

public class MiddlewareBuilder<TContext> : IMiddlewareBuilder<TContext>
{
	protected readonly MiddlewareCollection<TContext> middleware = [];

	public IMiddlewareBuilder<TContext> Add(MiddlewareBuilderFunc<TContext> action)
	{
		middleware.Add(action);
		return this;
	}

	public IMiddlewareBuilder<TContext> Add(IMiddlewareHandler<TContext> handler)
	{
		var middlewareWrapper = new MiddlewareChainHandler(handler);
		middleware.Add(middlewareWrapper.HandleAsync);
		return this;
	}

	public IMiddlewareHandler<TContext> Build()
	{
		return new RootMiddlewareHandler(middleware);
	}

	private class RootMiddlewareHandler(MiddlewareCollection<TContext> middleware) : IMiddlewareHandler<TContext>
	{
		public Task HandleAsync(TContext context, CancellationToken token)
		{
			return middleware.HandleAsync(context, token);
		}
	}

	public sealed class MiddlewareChainHandler(IMiddlewareHandler<TContext> innerHandler)
	{
		public async Task HandleAsync(MiddlewareFunc<TContext> next, TContext context, CancellationToken token)
		{
			await innerHandler.HandleAsync(context, token).Await();
			await next.Invoke(context, token).Await();
		}
	}
}
