using Transmitto.Net.Clients;
using Transmitto.Net.Server;

namespace Transmitto.Net;

public abstract class TransmittoConnection : ITransmittoConnection
{
	private bool _disposedValue;

	protected TransmittoConnection()
	{
	}

	public abstract bool IsConnected();

	public static ITransmittoServerConnection CreateServer(TransmittoServerOptions options)
		=> new SingleTransmittoServerConnection(options);

	public static ITransmittoClientConnection CreateClient(TransmittoClientOptions options)
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
