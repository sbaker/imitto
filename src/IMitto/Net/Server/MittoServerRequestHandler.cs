using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using IMitto.Channels;
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

	public MittoServerRequestHandler(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServerRequestHandler> logger,
		IMittoAuthenticationHandler authenticationHandler,
		IServerEventManager eventManager,
		IMittoChannelWriterProvider<ConnectionContext> channelProvider)
	{
		_options = options.Value;
		_logger = logger;
		_authenticationHandler = authenticationHandler;
		_eventManager = eventManager;
		_channelProvider = channelProvider;
	}

	public Task HandleRequestAsync(ConnectionContext context, CancellationToken token = default)
	{
		return HandleAuthenticationAsync(context, token);
	}

	public async Task HandleAuthenticationAsync(ConnectionContext context, CancellationToken token)
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

		await HandleAuthorizationAsync(context, token);
	}

	public async Task HandleAuthorizationAsync(ConnectionContext context, CancellationToken token)
	{
		token.ThrowIfCancellationRequested();

		var message = await ListenForRequest<MittoTopicsRequest>(context, token);

		_logger.LogTrace("Topic Registration received: {connecitonId} {message}", context.ConnectionId, message);

		if (message is not null)
		{
			var validationResult = await _authenticationHandler.AuthorizeTopicAccess(context, message);

			if (validationResult.IsAuthorized)
			{
				// TODO: Also, side note: the client that sent the package should not receive the
				// TODO: same package and publish the event locally after sending it to the server
				// TODO: but might need to verify before publishing. Then again the server should
				// TODO: respond the client with its permission (or if it applies) to publish that event locally 

				//await SendResponse(context, new MittoStatusResponse(MittoStatusBody.WithStatus((int)HttpStatusCode.OK), message.Header), token);

				context.Topics = message.Body.Topics;

				_logger.LogTrace("Topic Registration: start {connectionId}", context.ConnectionId);
				await RegisterClientInServerManager(context, token);
				_logger.LogTrace("Topic Registration: end {connectionId}", context.ConnectionId);
			}
		}
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
				: MittoEventType.Unauthorized, context.ConnectionId);

			_logger.LogTrace("Authenticating request: end {connectionId}", context.ConnectionId);
		}

		await SendResponse(context, new MittoStatusResponse(body, header), token);
	}

	private async Task RegisterClientInServerManager(ConnectionContext context, CancellationToken token)
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

	private async Task SendResponse<TBody>(ConnectionContext context, MittoResponse<TBody> response, CancellationToken token = default) where TBody : MittoMessageBody
	{
		_logger.LogTrace("Writing response: start {connectionId}", context.ConnectionId);
		await context.Socket.SendResponseAsync(response, token);
		_logger.LogTrace("Writing response: end {connectionId}", context.ConnectionId);
	}
}
