using Transmitto.Net.Clients;

namespace Transmitto.Net.Server;

public abstract class TransmittoServerConnection : TransmittoConnection, ITransmittoServerConnection
{
	protected TransmittoServerConnection(TransmittoServerOptions options)
	{
		Options = options;
	}

	protected TransmittoServerOptions Options { get; set; }

	public abstract Task<TransmittoSocket> AcceptAsync(CancellationToken token = default);

	public abstract Task ConnectAsync(CancellationToken token = default);
}
