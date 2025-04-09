namespace IMitto.Middlware;

public class MiddlewareBuilder : IMiddlewareBuilder
{
	protected readonly MiddlewareCollection middleware = [];

	public IMiddlewareBuilder Add(MiddlewareBuilderAction action)
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
			return _middleware.WhenAllAsync(context, token);
		}
	}
}

public class MiddlewareBuilder<T> : IMiddlewareBuilder<T>
{
	protected readonly MiddlewareCollection<T> middleware = [];

	public IMiddlewareBuilder<T> Add(MiddlewareBuilderAction<T> action)
	{
		middleware.Add(action);
		return this;
	}

	public IMiddlewareBuilder<T> Add(IMiddlewareHandler<T> handler)
	{
		middleware.Add((next, context, token) => handler.HandleAsync(context, token));
		return this;
	}

	public IMiddlewareHandler<T> Build()
	{
		return new RootMiddlewareHandler<T>(middleware);
	}

	private class RootMiddlewareHandler<TState> : IMiddlewareHandler<TState>
	{
		private readonly MiddlewareCollection<TState> _middleware;

		public RootMiddlewareHandler(MiddlewareCollection<TState> middleware)
		{
			_middleware = middleware;
		}

		public Task HandleAsync(MiddlewareContext<TState> context, CancellationToken token)
		{
			return _middleware.WhenAllAsync(context, token);
		}
	}

	private class MiddlewareHandler<TState> : IMiddlewareHandler<TState>
	{
		private readonly MiddlewareBuilderAction<TState> _action;
		private readonly IMiddlewareHandler<TState> _next;

		public MiddlewareHandler(MiddlewareBuilderAction<TState> action, IMiddlewareHandler<TState> next)
		{
			_action = action;
			_next = next;
		}
		public Task HandleAsync(MiddlewareContext<TState> context, CancellationToken token)
		{
			return _action(_next.HandleAsync, context, token);
		}
	}
}