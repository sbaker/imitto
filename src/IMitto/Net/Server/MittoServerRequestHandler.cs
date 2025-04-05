using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using IMitto.Channels;
using IMitto.Net;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;

namespace IMitto.Net.Server;

public class MittoServerRequestHandler : IMittoRequestHandler
{
	private readonly MittoServerOptions _options;
	private readonly ILogger<MittoServerRequestHandler> _logger;
	private readonly IMittoAuthenticationHandler _authenticationHandler;
	private readonly IServerEventManager _eventManager;
	private readonly IMittoChannelWriterProvider<ConnectionContext> _channelProvider;
	private readonly IMittoEventListener _eventListener;

	public MittoServerRequestHandler(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServerRequestHandler> logger,
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
		return WaitForAuthenticationAsync(context, token);
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

		var message = await ListenForRequest<MittoTopicsRequest>(context, token);

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

	private async Task RegisterClientForNotification(ConnectionContext context, MittoTopicsRequest header, CancellationToken token)
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
		MittoHeader? header = MittoHeader.Error;
		MittoStatusBody? body = MittoStatusBody.Error("The request could not be authenticated.");

		if (request.Header.Action == MittoEventType.Authentication)
		{
			_logger.LogTrace("Authenticating request: start {connectionId}", context.ConnectionId);

			var authenticated = await _authenticationHandler.HandleAuthenticationAsync(context, token);

			body = MittoStatusBody.WithStatus(authenticated
				? (int)HttpStatusCode.OK
				: (int)HttpStatusCode.Unauthorized);
			header = MittoHeader.Authorization(authenticated
				? MittoEventType.Completed
				: MittoEventType.Unauthorized);

			_logger.LogTrace("Authenticating request: end {connectionId}", context.ConnectionId);
		}

		await SendResponse(context, new MittoStatusResponse(body, header), token);
	}

	private async Task SendResponse<TBody>(ConnectionContext context, MittoResponse<TBody> response, CancellationToken token = default) where TBody : MittoMessageBody
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
