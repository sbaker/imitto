using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using IMitto.Channels;
using IMitto.Net;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using IMitto.Net.Server;

namespace IMitto.Server;

public class TransmittoServerRequestHandler : IMittoRequestHandler
{
	private readonly TransmittoServerOptions _options;
	private readonly ILogger<TransmittoServerRequestHandler> _logger;
	private readonly IMittoAuthenticationHandler _authenticationHandler;
	private readonly IServerEventManager _eventManager;
	private readonly IMittoChannelWriterProvider<ConnectionContext> _channelProvider;
	private readonly IMittoEventListener _eventListener;

	public TransmittoServerRequestHandler(
		IOptions<TransmittoServerOptions> options,
		ILogger<TransmittoServerRequestHandler> logger,
		IMittoAuthenticationHandler authenticationHandler,
		IServerEventManager eventManager,
		IMittoChannelWriterProvider<ConnectionContext> channelProvider,
		IMittoEventListener eventListener)
	{
		_options = options.Value;
		_logger = logger;
		_authenticationHandler = authenticationHandler;
		_eventManager = eventManager;
		_channelProvider = channelProvider;
		_eventListener = eventListener;
	}

	public Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default)
	{
		return Task.Run(async () => await WaitForAuthenticationAsync(context, token), token);
	}

	private async Task<TRequest?> ListenForRequest<TRequest>(ConnectionContext context, CancellationToken token)
		where TRequest : IMittoRequest
	{
		_logger.LogTrace("Listen For Request: start {connectionId}", context.ConnectionId);

		while (!context.Socket.DataAvailable)
		{
			await Task.Delay(200, token);

			token.ThrowIfCancellationRequested();
		}

		_logger.LogTrace("Listen For Request: end {connectionId}", context.ConnectionId);

		var message = await context.Socket.ReadAsync<TRequest>(token);

		if (message is not null)
		{
			_eventManager.Publish(ServerEventConstants.MessageReceivedEvent, message);
		}

		return message;
	}

	private async Task WaitForAuthenticationAsync(ConnectionContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		var message = await ListenForRequest<AuthenticationRequest>(context, token);

		_logger.LogTrace("Authentication Message received: {connecitonId} {message}", context.ConnectionId, message);

		if (message is not null)
		{
			_logger.LogTrace("Authentication: start {connectionId}", context.ConnectionId);
			await AuthenticationAsync(context, message, token);
			_logger.LogTrace("Authentication: end {connectionId}", context.ConnectionId);
		}

		await WaitForTopicsAsync(context, token);
	}

	private async Task WaitForTopicsAsync(ConnectionContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		var message = await ListenForRequest<TransmittoTopicsRequest>(context, token);

		_logger.LogTrace("Topic Registration received: {connecitonId} {message}", context.ConnectionId, message);

		if (message is not null)
		{
			var validationResult = await _authenticationHandler.AuthorizeTopicAccess(context, message);

			if (validationResult.IsAuthorized)
			{
				await RegisterClientForNotification(context, message, token);
			}
		}
	}

	private async Task RegisterClientForNotification(ConnectionContext context, TransmittoTopicsRequest header, CancellationToken token)
	{
		_logger.LogTrace("Registering client for events: start {connectionId}", context.ConnectionId);

		var writer = _channelProvider.GetWriter();

		if (await writer.WaitToWriteAsync(token))
		{
			await writer.WriteAsync(context, token);
		
			_logger.LogTrace("Registered client success: {connectionId}", context.ConnectionId);
		}

		_logger.LogTrace("Registering client for events: end {connectionId}", context.ConnectionId);
	}

	private async Task AuthenticationAsync(ConnectionContext context, AuthenticationRequest request, CancellationToken token)
	{
		TransmittoHeader? header = TransmittoHeader.Error;
		TransmittoStatusBody? body = TransmittoStatusBody.Error("The request could not be authenticated.");

		if (request.Header.Action == TransmittoEventType.Authentication)
		{
			_logger.LogTrace("Authenticating request: start {connectionId}", context.ConnectionId);

			var authenticated = await _authenticationHandler.HandleAuthenticationAsync(context, token);

			body = TransmittoStatusBody.WithStatus(authenticated
				? (int)HttpStatusCode.OK
				: (int)HttpStatusCode.Unauthorized);
			header = TransmittoHeader.Authorization(authenticated
				? TransmittoEventType.Completed
				: TransmittoEventType.Unauthorized);

			_logger.LogTrace("Authenticating request: end {connectionId}", context.ConnectionId);
		}

		await SendResponse(context, new TransmittoStatusResponse(body, header), token);
	}

	private async Task SendResponse<TBody>(ConnectionContext context, TransmittoResponse<TBody> response, CancellationToken token = default) where TBody : TransmittoMessageBody
	{
		_logger.LogTrace("Writing response: start {connectionId}", context.ConnectionId);
		await context.Socket.SendResponseAsync(response, token);
		_logger.LogTrace("Writing response: end {connectionId}", context.ConnectionId);
	}

	public IMittoEventListener GetEventListener()
	{
		return _eventListener;
	}
}
