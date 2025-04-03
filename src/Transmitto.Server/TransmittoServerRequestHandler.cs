using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;
using Transmitto.Net;
using Transmitto.Net.Models;
using Transmitto.Net.Requests;
using Transmitto.Net.Responses;
using Transmitto.Net.Server;

namespace Transmitto.Server;

public class TransmittoServerRequestHandler : ITransmittoRequestHandler
{
	private readonly ConcurrentDictionary<string, ConnectionContext> _connectedClients = new();
	private readonly TransmittoServerOptions _options;
	private readonly ILogger<TransmittoServerRequestHandler> _logger;
	private readonly ITransmittoAuthenticationHandler _authenticationHandler;
	private readonly IServerEventAggregator _eventAggregator;

	public TransmittoServerRequestHandler(
		IOptions<TransmittoServerOptions> options,
		ILogger<TransmittoServerRequestHandler> logger,
		ITransmittoAuthenticationHandler authenticationHandler,
		IServerEventAggregator eventAggregator)
	{
		_options = options.Value;
		_logger = logger;
		_authenticationHandler = authenticationHandler;
		_eventAggregator = eventAggregator;
	}

	public Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default)
	{
		_connectedClients.AddOrUpdate(context.ConnectionId, context, (k, v) => context);

		return Task.Run(async () => await WaitForAuthenticationAsync(context, token), token);
	}

	private async Task<TRequest?> ListenForRequest<TRequest>(ConnectionContext context, CancellationToken token)
		where TRequest : TransmittoMessage
	{
		_logger.LogTrace("Listen For Request: start");

		while (!context.Socket.DataAvailable)
		{
			await Task.Delay(200, token);

			token.ThrowIfCancellationRequested();
		}

		_logger.LogTrace("Listen For Request: end");

		return await context.Socket.ReadAsync<TRequest>(token);
	}

	private async Task WaitForAuthenticationAsync(ConnectionContext context, CancellationToken token)
	{
		var message = await ListenForRequest<AuthenticationRequest>(context, token);

		_logger.LogTrace("Authentication Message received: {message}", message);

		if (message is not null)
		{
			_logger.LogTrace("Authentication: start");
			await AuthenticationAsync(context, message, token);
			_logger.LogTrace("Authentication: end");
		}

		await WaitForTopicsAsync(context, token);
	}

	private async Task WaitForTopicsAsync(ConnectionContext context, CancellationToken token)
	{
		while (context.Socket.IsConnected)
		{
			token.ThrowIfCancellationRequested();

			var message = await ListenForRequest<TransmittoTopicsRequest>(context, token);

			_logger.LogTrace("Authentication Message received: {connecitonId} {message}", context.ConnectionId, message);

			if (message is not null)
			{
				_logger.LogTrace("GetEventNotifications: start {connectionId}", context.ConnectionId);
				await GetEventNotificationsForTopics(context, message, token);
				_logger.LogTrace("GetEventNotifications: end {connectionId}", context.ConnectionId);
			}
		}
	}

	private async Task GetEventNotificationsForTopics(ConnectionContext context, TransmittoTopicsRequest header, CancellationToken token)
	{
		await SendResponse(context, new TransmittoResponse<EventNotificationsBody>
		{
			Header = new(),
			Body = new()
			{
				Content = new()
				{
					Events = [new()
					{
						Event = new TransmittoEvent<string>
						{
							Message = "this is a test from the server"
						}
					}]
				}
			}
		}, token);
	}

	private async Task AuthenticationAsync(ConnectionContext context, AuthenticationRequest request, CancellationToken token)
	{
		TransmittoStatusBody? body;
		TransmittoHeader? header;

		switch (request.Header.Action)
		{
			case TransmittoEventType.Authentication:
				{
					_logger.LogTrace("Authenticating request: start");
					var authenticated = await _authenticationHandler.HandleAuthenticationAsync(context, token);

					body = TransmittoStatusBody.WithStatus(
						authenticated ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Unauthorized
					);
					header = new TransmittoHeader
					{
						Action = authenticated ? TransmittoEventType.Completed : TransmittoEventType.Unauthorized,
					};

					_logger.LogTrace("Authenticating request: end");
					break;
				}
			default:
				{
					body = TransmittoStatusBody.Error("");
					header = TransmittoHeader.Error;
					break;
				}
		}

		await SendResponse(context, new TransmittoStatusResponse(body, header), token);
	}

	private async Task SendResponse<TBody>(ConnectionContext context, TransmittoResponse<TBody> response, CancellationToken token = default) where TBody : TransmittoMessageBody
	{
		_logger.LogTrace("Writing response: start");
		await context.Socket.SendResponseAsync(response, token);
		_logger.LogTrace("Writing response: end");
	}
}
