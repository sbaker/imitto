using System;

namespace IMitto;

public abstract class Disposable : IDisposable
{
	private bool _disposed;

	public bool Disposed => _disposed;

	protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(Disposed, GetType());
	}

	protected abstract void DisposeCore();

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				DisposeCore();
			}

			_disposed = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

public class Disposables : Disposable
{
	private readonly Queue<IDisposable> _disposables = [];

	public TDisposable Add<TDisposable>(TDisposable disposable) where TDisposable : IDisposable
	{
		_disposables.Enqueue(disposable);

		return disposable;
	}

	public TDisposable Add<TDisposable>(TDisposable disposable, Action disposing) where TDisposable : IDisposable
	{
		_disposables.Enqueue(new Disposer(disposable, disposing));

		return disposable;
	}

	public TDisposable Add<TDisposable>(TDisposable disposable, Action<TDisposable> disposing) where TDisposable : IDisposable
	{
		_disposables.Enqueue(new Disposer<TDisposable>(disposable, disposing));

		return disposable;
	}

	public void Add(IDisposable disposable, IDisposable disposable1)
	{
		_disposables.Enqueue(disposable);
		_disposables.Enqueue(disposable1);
	}

	public void Add(params IReadOnlyList<IDisposable> disposables)
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
