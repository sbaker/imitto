namespace IMitto.Middlware;

public sealed class MiddlewareExecutor
{
	private static readonly MiddlewareExecutor Instance = new();

	private MiddlewareExecutor()
	{
	}

	public static Task ExecuteAsync(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	{
		return Instance.ExecuteMiddlewareAsync(collection, state, token);
	}

	public static Task ExecuteAsync<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	{
		return Instance.ExecuteMiddlewareAsync(collection, state, token);
	}

	private Task ExecuteMiddlewareAsync(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	{
		if (collection.Count == 0)
		{
			return Task.CompletedTask;
		}

		var count = 0;

		return Next(state, token);

		Task Next(MiddlewareContext context, CancellationToken token)
		{
			if (count >= collection.Count)
			{
				return Task.CompletedTask;
			}

			token.ThrowIfCancellationRequested();
			return collection.ElementAt(count++).Invoke(Next, context, token);
		}
	}

	private Task ExecuteMiddlewareAsync<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	{
		if (collection.Count == 0)
		{
			return Task.CompletedTask;
		}

		var count = 0;

		return Next(state, token);

		Task Next(MiddlewareContext<T> context, CancellationToken token)
		{
			if (count >= collection.Count)
			{
				return Task.CompletedTask;
			}

			token.ThrowIfCancellationRequested();
			return collection.ElementAt(count++).Invoke(Next, context, token);
		}
	}

	// 20.0002047995 more memory
	//public static Task ExecuteAsyncOld(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	//{
	//	return Instance.ExecuteMiddlewareAsyncOld(collection, state, token);
	//}

	//public static Task ExecuteAsyncOld<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	//{
	//	return Instance.ExecuteMiddlewareAsyncOld(collection, state, token);
	//}

	//private Task ExecuteMiddlewareAsyncOld(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	//{
	//	var handlers = collection.ToArray();

	//	if (handlers.Length == 0)
	//	{
	//		return Task.CompletedTask;
	//	}

	//	var count = 0;

	//	return Next(state, token);

	//	Task Next(MiddlewareContext context, CancellationToken token)
	//	{
	//		if (count >= handlers.Length)
	//		{
	//			return Task.CompletedTask;
	//		}

	//		token.ThrowIfCancellationRequested();
	//		return handlers[count++].Invoke(Next, context, token);
	//	}
	//}

	//private Task ExecuteMiddlewareAsyncOld<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	//{
	//	var handlers = collection.ToArray();

	//	if (handlers.Length == 0)
	//	{
	//		return Task.CompletedTask;
	//	}

	//	var count = 0;

	//	return Next(state, token);

	//	Task Next(MiddlewareContext<T> context, CancellationToken token)
	//	{
	//		if (count >= handlers.Length)
	//		{
	//			return Task.CompletedTask;
	//		}

	//		token.ThrowIfCancellationRequested();
	//		return handlers[count++].Invoke(Next, context, token);
	//	}
	//}





	// 40.4760380952% more memory
	//public static Task ExecuteAsyncNew(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	//{
	//	return Instance.ExecuteMiddlewareAsyncNew(collection, state, token);
	//}

	//public static Task ExecuteAsyncNew<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	//{
	//	return Instance.ExecuteMiddlewareAsyncNew(collection, state, token);
	//}

	//private Task ExecuteMiddlewareAsyncNew(MiddlewareCollection collection, MiddlewareContext state, CancellationToken token)
	//{
	//	return ExecuteInternalAsync(collection, (handler, context, token, next) => handler.Invoke(next.Invoke, context, token), state, token);
	//}

	//private Task ExecuteMiddlewareAsyncNew<T>(MiddlewareCollection<T> collection, MiddlewareContext<T> state, CancellationToken token)
	//{
	//	return ExecuteInternalAsync(collection, (handler, context, token, next) => handler.Invoke(next.Invoke, context, token), state, token);
	//}

	//private Task ExecuteInternalAsync<T, TContext>(
	//	IEnumerable<T> collection,
	//	Func<T, TContext, CancellationToken, AsyncHandler<TContext>, Task> action,
	//	TContext state,
	//	CancellationToken token) where TContext : MiddlewareContext
	//{
	//	var handlers = collection.ToArray();

	//	if (handlers.Length == 0)
	//	{
	//		return Task.CompletedTask;
	//	}

	//	var count = 0;

	//	return Next(state, token);

	//	Task Next(TContext context, CancellationToken token)
	//	{
	//		if (count >= handlers.Length)
	//		{
	//			return Task.CompletedTask;
	//		}

	//		token.ThrowIfCancellationRequested();

	//		return action(handlers[count++], context, token, Next);
	//	}
	//}
}
