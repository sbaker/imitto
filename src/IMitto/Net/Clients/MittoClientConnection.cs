using System.Net.Sockets;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using IMitto.Pipelines;

namespace IMitto.Net.Clients;

public class MittoClientConnection : MittoConnection, IMittoClientConnection
{
	private MittoSocket? _mittoSocket;

	public MittoClientConnection(MittoClientOptions options) : base()
	{
		Options = options;
	}

	protected MittoClientOptions Options { get; }

	protected MittoSocket? Socket => _mittoSocket;

	public async Task ConnectAsync(CancellationToken token = default)
	{
		if (_mittoSocket?.IsConnected ?? false == true)
		{
			return;
		}

		var tcpClient = new TcpClient();
		var connectionOptions = Options.Connection;
		await tcpClient.ConnectAsync(connectionOptions.Host.EndPoint, token);

		tcpClient.ReceiveTimeout = connectionOptions.ConnectionTimeout;
		tcpClient.SendTimeout = connectionOptions.ConnectionTimeout;

		_mittoSocket = new MittoPipelineSocket(tcpClient, Options);
	}

	public override bool IsConnected()
	{
		return _mittoSocket?.IsConnected ?? false;
	}

	public Task<T?> ReadResponseAsync<T>(CancellationToken token)
	{
		if (_mittoSocket == null || !_mittoSocket.IsConnected)
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		return _mittoSocket.ReadResponseAsync<T>(token);
	}

	public Task SendRequestAsync(AuthenticationRequest authenticationRequest, CancellationToken token)
	{
		if (_mittoSocket == null || !_mittoSocket.IsConnected)
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		return _mittoSocket.SendRequestAsync(authenticationRequest, token);
	}

	public async Task<EventNotificationsModel> WaitForEventsAsync(CancellationToken token)
	{
		if (_mittoSocket == null || !_mittoSocket.IsConnected)
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		await _mittoSocket!.SendRequestAsync(new MittoTopicsRequest
		{
			Header = new()
			{
				Path = MittoPaths.Topics,
				Action = MittoEventType.Consume
			},
			Body = new()
			{
				//var topics =Options.TypeMappings.Keys.Distinct()
				Topics = new TopicRegistrationModel
				{
					PublishTopics = ["test-topic-1"],
					SubscriptionTopics = ["test-topic-2"],
				}
			}
		}, token);
		
		while (!_mittoSocket.DataAvailable)
		{
			token.ThrowIfCancellationRequested();
			await Task.Delay(Options.Connection.TaskDelayMilliseconds, token);
		}

		var response = await _mittoSocket.ReadResponseAsync<EventNotificationsResponse>(token);

		if (response is not null && response.Header.Path == MittoPaths.Topics)
		{
			var eventNotifications = response.ReadBodyAs<EventNotificationsBody>(Options.Json.Serializer);

			return eventNotifications.Content!;
		}

		return null!;
	}

	protected override void DisposeCore()
	{
		_mittoSocket?.Dispose();
	}
}