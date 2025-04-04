namespace IMitto.Net.Server;

public abstract class TransmittoServerConnection : TransmittoConnection, IMittoServerConnection
{
	protected TransmittoServerConnection(TransmittoServerOptions options)
	{
		Options = options;
	}

	protected TransmittoServerOptions Options { get; set; }

	public abstract Task<TransmittoSocket> AcceptAsync(CancellationToken token = default);

	public abstract Task ConnectAsync(CancellationToken token = default);
}
