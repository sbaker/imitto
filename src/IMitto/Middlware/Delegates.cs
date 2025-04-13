namespace IMitto.Middlware;

public delegate Task MiddlewareFunc(MiddlewareContext context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc(MiddlewareFunc next, MiddlewareContext context, CancellationToken token);

public delegate Task MiddlewareFunc<TContext>(TContext context, CancellationToken token);
public delegate Task MiddlewareBuilderFunc<TContext>(MiddlewareFunc<TContext> next, TContext context, CancellationToken token);
