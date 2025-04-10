namespace IMitto;

public delegate Task AsyncHandler<TContext>(TContext context, CancellationToken token);

