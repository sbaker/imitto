using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using IMitto.Channels;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using System.Threading.Channels;

namespace IMitto.Net.Server;

public class MittoServerRequestHandler : IMittoRequestHandler
{
	private readonly MittoServerOptions _options;
	private readonly ILogger<MittoServerRequestHandler> _logger;
	private readonly IMittoAuthenticationHandler _authenticationHandler;
	private readonly IMittoChannelWriterProvider<ConnectionContext> _channelProvider;
	private readonly IMittoChannelWriterProvider<ServerEventNotificationsContext> _eventWriterProvider;

	public MittoServerRequestHandler(
		IOptions<MittoServerOptions> options,
		ILogger<MittoServerRequestHandler> logger,
		IMittoAuthenticationHandler authenticationHandler,
		IMittoChannelWriterProvider<ConnectionContext> channelProvider,
		IMittoChannelWriterProvider<ServerEventNotificationsContext> eventWriterProvider)
	{
		_options = options.Value;
		_logger = logger;
		_authenticationHandler = authenticationHandler;
		_channelProvider = channelProvider;
		_eventWriterProvider = eventWriterProvider;
	}

	public Task HandleAuthenticationAsync(MittoConnectionContext context, CancellationToken token)
	{
		return AuthenticationAsync(context.Connection, context.Request, token);
	}

	public Task HandleAuthorizationAsync(MittoConnectionContext context, CancellationToken token)
	{
		var message = context.Request.GetBody<MittoTopicMessageBody>();
		return AuthorizeAsync(context.Connection, new MittoRequest<MittoTopicMessageBody>(message, context.Request.Header), token);
	}

	public async Task HandleEventNotificationAsync(MittoConnectionContext context, CancellationToken token)
	{
		_logger.LogTrace("Listening for events: received {connectionId}", context.ConnectionId);

		var writer = _eventWriterProvider.GetWriter();

		if (await writer.WaitToWriteAsync(token).Await())
		{
			var eventNotification = context.Request.GetBody<EventNotificationsModel>(_options.Json);

			//TODO: Event is null..

			var eventContext = new ServerEventNotificationsContext(context.ConnectionId, eventNotification);

			await writer.WriteAsync(eventContext, token).Await();
		}
	}

	private async Task AuthorizeAsync(ConnectionContext context, IMittoRequest<MittoTopicMessageBody> message, CancellationToken token)
	{
		var validationResult = await _authenticationHandler.AuthorizeTopicAccessAsync(context, message).Await();

		if (validationResult.IsAuthorized)
		{
			// TODO: Also, side note: the client that sent the package should not receive the
			// TODO: same package and publish the event locally after sending it to the server
			// TODO: but might need to verify before publishing. Then again the server should
			// TODO: respond the client with its permission (or if it applies) to publish that event locally 

			context.Topics = message.Body.Topics;

			_logger.LogTrace("Topic Registration: start {connectionId}", context.ConnectionId);
			await RegisterClientInServerManager(context, token).Await();
			_logger.LogTrace("Topic Registration: end {connectionId}", context.ConnectionId);

			await SendResponse(context, new MittoStatusResponse(MittoStatusBody.WithStatus((int)HttpStatusCode.OK), message.Header), token).Await();
			return;
		}

		_logger.LogWarning("Topic Registration: failed {connectionId}", context.ConnectionId);

		var body = MittoStatusBody.WithStatus((int)HttpStatusCode.Unauthorized);

		if (validationResult.AccessAuthorizationDetails != null)
		{
			body.Details = validationResult.AccessAuthorizationDetails;
		}
		
		await SendResponse(context, new MittoStatusResponse(body, message.Header), token);

		context.Socket.Dispose();
	}

	private async Task AuthenticationAsync(ConnectionContext context, IMittoRequest<MittoMessageBody> request, CancellationToken token)
	{
		MittoHeader? header = MittoHeader.Error;
		MittoStatusBody? body = MittoStatusBody.Error("The request could not be authenticated.");

		if (request.Header.Action == MittoEventType.Authentication)
		{
			_logger.LogTrace("Authenticating request: start {connectionId}", context.ConnectionId);

			var authenticationRequest = new MittoRequest<MittoAuthenticationMessageBody>(request.GetBody<MittoAuthenticationMessageBody>(), request.Header);

			var authenticated = await _authenticationHandler.HandleAuthenticationAsync(context, authenticationRequest, token).Await();

			body = MittoStatusBody.WithStatus(authenticated
				? (int)HttpStatusCode.OK
				: (int)HttpStatusCode.Unauthorized);
			header = MittoHeader.Authorization(authenticated
				? MittoEventType.Completed
				: MittoEventType.Unauthorized, context.ConnectionId);

			_logger.LogTrace("Authenticating request: end {connectionId}", context.ConnectionId);
		}

		await SendResponse(context, new MittoStatusResponse(body, header), token).Await();
	}

	private async Task RegisterClientInServerManager(ConnectionContext context, CancellationToken token)
	{
		_logger.LogTrace("Registering client for events: start {connectionId}", context.ConnectionId);

		var writer = _channelProvider.GetWriter();

		if (await writer.WaitToWriteAsync(token).Await())
		{
			await writer.WriteAsync(context, token).Await();
		
			_logger.LogTrace("Registered client success: {connectionId}", context.ConnectionId);
		}

		_logger.LogTrace("Registering client for events: end {connectionId}", context.ConnectionId);
	}

	private async Task SendResponse<TBody>(ConnectionContext context, MittoResponse<TBody> response, CancellationToken token = default) where TBody : MittoMessageBody
	{
		_logger.LogTrace("Writing response: start {connectionId}", context.ConnectionId);
		await context.Socket.SendResponseAsync(response, token).Await();
		_logger.LogTrace("Writing response: end {connectionId}", context.ConnectionId);
	}
}
