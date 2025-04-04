using IMitto.Net.Clients;
using IMitto.Net.Server;

namespace IMitto.Net;

public abstract class TransmittoConnection : IMittoConnection
{
	private bool _disposedValue;

	protected TransmittoConnection()
	{
	}

	public abstract bool IsConnected();

	public static IMittoServerConnection CreateServer(TransmittoServerOptions options)
		=> new SingleTransmittoServerConnection(options);

	public static IMittoClientConnection CreateClient(TransmittoClientOptions options)
		=> new TransmittoClientConnection(options);

	protected abstract void Disposing();

	protected void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				Disposing();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			_disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
