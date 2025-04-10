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

	private class RootMiddlewareHandler : IMiddlewareHandler
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

public class MiddlewareBuilder<T> : IMiddlewareBuilder<T>
{
	protected readonly MiddlewareCollection<T> middleware = [];

	public IMiddlewareBuilder<T> Add(MiddlewareBuilderFunc<T> action)
	{
		middleware.Add(action);
		return this;
	}

	public IMiddlewareBuilder<T> Add(IMiddlewareHandler<T> handler)
	{
		var middlewareWrapper = new MiddlewareChainHandler<T>(handler);
		middleware.Add(middlewareWrapper.HandleAsync);
		return this;
	}

	public IMiddlewareHandler<T> Build()
	{
		return new RootMiddlewareHandler<T>(middleware);
	}

	private class RootMiddlewareHandler<TState>(MiddlewareCollection<TState> middleware) : IMiddlewareHandler<TState>
	{
		public Task HandleAsync(MiddlewareContext<TState> context, CancellationToken token)
		{
			return middleware.HandleAsync(context, token);
		}
	}

	public sealed class MiddlewareChainHandler<TState>(IMiddlewareHandler<TState> innerHandler)
	{
		public async Task HandleAsync(MiddlewareFunc<TState> next, MiddlewareContext<TState> context, CancellationToken token)
		{
			await innerHandler.HandleAsync(context, token);
			await next.Invoke(context, token);
		}
	}
}
