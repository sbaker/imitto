namespace Transmitto;

public abstract class Disposable : IDisposable
{
	private bool _disposed;

	protected bool Disposed => _disposed;

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
