namespace IMitto.Middlware;

public delegate Task MiddlewareFunc(MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareFunc<TState>(MiddlewareContext<TState> context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc(MiddlewareFunc next, MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc<TState>(MiddlewareFunc<TState> next, MiddlewareContext<TState> context, CancellationToken token);
