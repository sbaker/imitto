using System.Net.Sockets;
using Transmitto.Net.Clients;
using Transmitto.Net.Server;

namespace Transmitto.Net;

public class SingleTransmittoServerConnection : TransmittoServerConnection
{
	private readonly TcpListener _listener;

	public SingleTransmittoServerConnection(TransmittoServerOptions options) : base(options)
	{
		_listener = new(options.ConnectionOptions.Host.EndPoint);
	}

	public override async Task<TransmittoSocket> AcceptAsync(CancellationToken token = default)
	{
		var connectionOptions = Options.ConnectionOptions;
		var tcpClient = await _listener.AcceptTcpClientAsync(token);

		tcpClient.ReceiveTimeout = connectionOptions.ConnectionTimeout;
		tcpClient.SendTimeout = connectionOptions.ConnectionTimeout;
		return new TransmittoSocket(this, tcpClient, Options);
	}

	public override Task ConnectAsync(CancellationToken token = default)
	{
		return Task.Run(_listener.Start, token);
	}

	public override bool IsConnected()
	{
		return _listener.Server.Connected;
	}

	protected override void Disposing()
	{
		_listener.Stop();
		_listener.Dispose();
	}
}