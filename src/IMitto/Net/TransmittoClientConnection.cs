using System.Net.Sockets;
using IMitto.Net.Clients;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;

namespace IMitto.Net;

public class TransmittoClientConnection : TransmittoConnection, IMittoClientConnection
{
	private TransmittoSocket? _transmittoSocket;

	public TransmittoClientConnection(TransmittoClientOptions options) : base()
	{
		Options = options;
	}

	protected TransmittoClientOptions Options { get; }

	protected TransmittoSocket? Socket => _transmittoSocket;

	public async Task<TransmittoStatus> AuthenticateAsync(CancellationToken token = default)
	{
		if (!IsConnected())
		{
			await ConnectAsync(token);
		}

		var authBody = new TransmittoAuthenticationMessageBody
		{
			Key = Options.AuthenticationKey,
			Secret = Options.AuthenticationSecret
		};

		var authHeader = new TransmittoHeader
		{
			Path = TransmittoPaths.Auth,
			Action = TransmittoEventType.Authentication
		};

		await _transmittoSocket!.SendRequestAsync(new AuthenticationRequest(authBody, authHeader), token);

		var response = await _transmittoSocket.ReadResponseAsync<TransmittoStatusResponse>(token);
		return response!.Body.Status;
	}

	public async Task ConnectAsync(CancellationToken token = default)
	{
		if (!_transmittoSocket?.IsConnected ?? false == true)
		{
			return;
		}

		var tcpClient = new TcpClient();
		var connectionOptions = Options.Connection;
		await tcpClient.ConnectAsync(connectionOptions.Host.EndPoint, token);

		tcpClient.ReceiveTimeout = connectionOptions.ConnectionTimeout;
		tcpClient.SendTimeout = connectionOptions.ConnectionTimeout;

		_transmittoSocket = new TransmittoSocket(this, tcpClient, Options);
	}

	public override bool IsConnected()
	{
		return _transmittoSocket?.IsConnected ?? false;
	}

	public async Task<EventNotificationsModel> WaitForEventsAsync(CancellationToken token)
	{
		if (!IsConnected())
		{
			await ConnectAsync(token);
		}

		await _transmittoSocket!.SendRequestAsync(new TransmittoTopicsRequest
		{
			Header = new()
			{
				Path = TransmittoPaths.Topics,
				Action = TransmittoEventType.Consume
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
		
		while (!token.IsCancellationRequested && !_transmittoSocket.DataAvailable)
		{
			await Task.Delay(Options.Connection.TaskDelayMilliseconds, token);
		}

		var response = await _transmittoSocket.ReadResponseAsync<EventNotificationsResponse>(token);

		if (response is not null && response.Header.Path == TransmittoPaths.Topics)
		{
			var eventNotifications = response.ReadBodyAs<EventNotificationsBody>(Options.Json.Serializer);

			return eventNotifications.Content!;
		}

		return null!;
	}

	protected override void Disposing()
	{
		_transmittoSocket?.Dispose();
	}
}