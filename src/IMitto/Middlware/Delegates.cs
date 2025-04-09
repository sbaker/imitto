namespace IMitto.Middlware;

public delegate Task MiddlewareAction(MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareAction<TState>(MiddlewareContext<TState> context, CancellationToken token);
public delegate Task MiddlewareBuilderAction(MiddlewareAction next, MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareBuilderAction<TState>(MiddlewareAction<TState> next, MiddlewareContext<TState> context, CancellationToken token);
