namespace IMitto;

public class Disposables : Disposable
{
	private readonly Queue<IDisposable> _disposables = [];

	protected TDisposable Add<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
	{
		_disposables.Enqueue(disposable);

		return disposable;
	}

	protected TDisposable Add<TDisposable>(TDisposable disposable, Action disposing) where TDisposable : IDisposable
	{
		_disposables.Enqueue(new Disposer(disposable, disposing));

		return disposable;
	}

	protected TDisposable Add<TDisposable>(TDisposable disposable, Action<TDisposable> disposing) where TDisposable : IDisposable
	{
		_disposables.Enqueue(new Disposer<TDisposable>(disposable, disposing));

		return disposable;
	}

	protected void Add(IDisposable disposable, IDisposable disposable1)
	{
		_disposables.Enqueue(disposable);
		_disposables.Enqueue(disposable1);
	}

	protected void Add(params IReadOnlyList<IDisposable> disposables)
	{
		foreach (var disposable in disposables)
		{
			_disposables.Enqueue(disposable);
		}
	}

	protected override void DisposeCore()
	{
		while (_disposables.Count > 0)
		{
			_disposables.Dequeue()?.Dispose();
		}
	}

	private record Disposer(IDisposable Disposable, Action Disposing) : IDisposable
	{
		public void Dispose()
		{
			Disposing.Invoke();
			Disposable.Dispose();
		}
	}

	private sealed record Disposer<TDisposable>(TDisposable Disposable, Action<TDisposable> Disposing) : IDisposable where TDisposable : IDisposable
	{
		public void Dispose()
		{
			Disposing.Invoke(Disposable);
			Disposable.Dispose();
		}
	}
}
