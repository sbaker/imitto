using System.Net.Sockets;
using IMitto.Pipelines;
using IMitto.Protocols.Requests;
using IMitto.Protocols.Responses;

namespace IMitto.Net.Clients;

public class MittoClientConnection : MittoConnection, IMittoClientConnection
{
	private MittoSocket? _mittoSocket;

	public MittoClientConnection(MittoClientOptions options) : base()
	{
		Options = options;
	}

	public string? ConnectionId { get; set; }

	protected MittoClientOptions Options { get; }

	protected MittoSocket? Socket => _mittoSocket;

	public override Task CloseAsync(CancellationToken token = default)
	{
		if (_mittoSocket is null || !_mittoSocket.IsConnected || _mittoSocket.Disposed)
		{
			return Task.CompletedTask;
		}

		_mittoSocket.Dispose();

		_mittoSocket = null;

		return Task.CompletedTask;
	}

	public override async Task ConnectAsync(CancellationToken token = default)
	{
		if (_mittoSocket?.IsConnected ?? false == true)
		{
			return;
		}

		var tcpClient = new TcpClient();
		var connectionOptions = Options.Connection;
		await tcpClient.ConnectAsync(connectionOptions.Host.EndPoint, token).Await();

		tcpClient.ReceiveTimeout = connectionOptions.ConnectionTimeout;
		tcpClient.SendTimeout = connectionOptions.ConnectionTimeout;

		_mittoSocket = new MittoPipelineSocket(tcpClient, Options);
	}

	public override bool IsConnected()
	{
		return _mittoSocket?.IsConnected ?? false;
	}

	public bool IsDataAvailable()
	{
		return _mittoSocket?.DataAvailable ?? false;
	}

	public Task<TResponse?> ReadResponseAsync<TResponse>(CancellationToken token) where TResponse : IMittoResponse
	{
		if (_mittoSocket == null || !_mittoSocket.IsConnected)
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		return _mittoSocket.ReadResponseAsync<TResponse>(token);
	}

	public Task SendRequestAsync<TRequest>(TRequest request, CancellationToken token) where TRequest : IMittoRequest
	{
		if (_mittoSocket == null || !_mittoSocket.IsConnected)
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		return _mittoSocket.SendRequestAsync(request, token);
	}

	protected override void DisposeCore()
	{
		_mittoSocket?.Dispose();
	}
}