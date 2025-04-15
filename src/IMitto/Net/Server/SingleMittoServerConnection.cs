using IMitto.Pipelines;
using System.Net.Sockets;

namespace IMitto.Net.Server;

public class SingleMittoServerConnection : MittoServerConnection
{
	private readonly TcpListener _listener;

	public SingleMittoServerConnection(MittoServerOptions options) : base(options)
	{
		_listener = new(options.Connection.Host.EndPoint);
	}

	public override async Task<MittoSocket> AcceptAsync(CancellationToken token = default)
	{
		var connectionOptions = Options.Connection;
		var tcpClient = await _listener.AcceptTcpClientAsync(token).Await();

		tcpClient.ReceiveTimeout = connectionOptions.ConnectionTimeout;
		tcpClient.SendTimeout = connectionOptions.ConnectionTimeout;
		return new MittoPipelineSocket(tcpClient, Options);
	}

	public override Task CloseAsync(CancellationToken token = default)
	{
		return Task.CompletedTask.ContinueWith(t => _listener.Stop(), token);
	}

	public override Task ConnectAsync(CancellationToken token = default)
	{
		return Task.Run(_listener.Start, token);
	}

	public override bool IsConnected()
	{
		return _listener.Server.Connected;
	}

	protected override void DisposeCore()
	{
		_listener.Stop();
		_listener.Dispose();
	}
}