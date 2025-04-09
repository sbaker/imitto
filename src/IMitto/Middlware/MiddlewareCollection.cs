namespace IMitto.Middlware;

public sealed class MiddlewareCollection : List<MiddlewareBuilderAction>
{
	public MiddlewareCollection() : base()
	{
	}

	public MiddlewareCollection(IEnumerable<MiddlewareBuilderAction> collection) : base(collection)
	{
	}

	public MiddlewareCollection(int capacity) : base(capacity)
	{
	}

	public Task WhenAllAsync(MiddlewareContext context, CancellationToken token)
	{
		var handlers = ToArray();

		if (handlers.Length == 0)
		{
			return Task.CompletedTask;
		}

		var count = 0;

		return Next(context, token);

		Task Next(MiddlewareContext context, CancellationToken token)
		{
			if (count >= handlers.Length)
			{
				return Task.CompletedTask;
			}

			return handlers[count++].Invoke(Next, context, token);
		}
	}
}

public sealed class MiddlewareCollection<T> : List<MiddlewareBuilderAction<T>>
{
	public MiddlewareCollection() : base()
	{
	}

	public MiddlewareCollection(IEnumerable<MiddlewareBuilderAction<T>> collection) : base(collection)
	{
	}

	public MiddlewareCollection(int capacity) : base(capacity)
	{
	}

	public Task WhenAllAsync(MiddlewareContext<T> context, CancellationToken token)
	{
		var handlers = ToArray();

		if (handlers.Length == 0)
		{
			return Task.CompletedTask;
		}

		var count = 0;

		return Next(context, token);

		Task Next(MiddlewareContext<T> context, CancellationToken token)
		{
			if (count >= handlers.Length)
			{
				return Task.CompletedTask;
			}

			token.ThrowIfCancellationRequested();
			return handlers[count++].Invoke(Next, context, token);
		}
	}
}
