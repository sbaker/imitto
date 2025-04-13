﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IMitto.Net.Models;
using IMitto.Hosting;
using System.Net.Sockets;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using IMitto.Middlware;

namespace IMitto.Net.Clients;

public class MittoClient : MittoHost<MittoClientOptions>, IMittoClient
{
	private readonly MittoClientOptions _options;
	private readonly IMittoEventDispatcher _eventDispatcher;
	private readonly ILogger<MittoClient> _logger;
	private readonly IMittoClientEventManager _eventManager;
	private IMittoClientConnection? _connection;
	private Task? _clientEventLoopTask;

	public MittoClient(
		IOptions<MittoClientOptions> options,
		ILogger<MittoClient> logger,
		IMittoEventDispatcher eventDispatcher,
		IMittoClientEventManager eventManager) : base(logger, options)
	{
		_options = options.Value;
		_eventDispatcher = eventDispatcher;
		_eventManager = eventManager;
		_logger = logger;
	}

	private IMittoClientConnection Connection => GetOrCreateConnection();

	protected override Task RunInternalAsync(CancellationToken token)
	{
		var middleware = GetMiddleware();

		return Task.Run(async () =>
		{
			var retries = 0;

			while (_options.MaxConnectionRetries > retries++)
			{
				try
				{
					var context = new MiddlewareContext();
					await middleware.HandleAsync(context, token).Await();
				}
				catch (TaskCanceledException tce)
				{
					_logger.LogWarning(tce, "Task canceled");
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Unknown error.");
				}

				Connection.Dispose();
			}
		}, token);
	}

	public async Task<MittoStatus> AuthenticateAsync(CancellationToken token = default)
	{
		if (!Connection.IsConnected())
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		var authBody = new MittoAuthenticationMessageBody
		{
			Key = Options.AuthenticationKey,
			Secret = Options.AuthenticationSecret
		};

		var authHeader = new MittoHeader
		{
			Path = MittoPaths.Auth,
			Action = MittoEventType.Authentication,
			Version = MittoConstants.Version,
			ConnectionId = Connection.ConnectionId,
		};

		await Connection!.SendRequestAsync(new AuthenticationRequest(authBody, authHeader), token).Await();
		var response = await Connection.ReadResponseAsync<MittoStatusResponse>(token).Await();

		Connection.ConnectionId = response!.Header.ConnectionId;
		return response.Body.Status;
	}

	private Task StartEventLoopsAsync(CancellationToken token)
	{
		StartBackgroundTask(token => _eventManager.WaitForClientEventsAsync(Connection, token), token);

		return Task.Run(async () =>
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					if (!Connection.IsConnected())
					{
						throw new InvalidOperationException("Socket is not connected or not initialized.");
					}

					await _eventManager.WaitForServerEventsAsync(Connection, token).Await();
				}
			}
			catch (Exception e)
			{
				switch (e)
				{
					case TaskCanceledException tce:
						_logger.LogWarning(tce, "Task canceled");
						break;
					case SocketException se:
						_logger.LogError(se, "Socket error: {error}", se.SocketErrorCode);
						break;
					case IOException ioe:
						_logger.LogError(ioe, "IO error: {error}", ioe.Message);
						break;
					default:
						_logger.LogError(e, "An unexpected error.");
						break;
				}

				throw;
			}
		}, token);

		//var delayedTask = Task.Delay(20000, token).ContinueWith(task => {
		//	// TODO: TESTCODE: Add code to publish an event after 20s.
		//}, token);
	}

	private IMiddlewareHandler GetMiddleware()
	{
		return new MiddlewareBuilder()
			//.Add(async (next, context, ct) => {
			//	_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);

			//	if (!Connection.IsConnected())
			//	{
			//		await Connection.ConnectAsync(ct).Await();
			//	}

			//	await next(context, ct).Await();

			//	_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
			//})
			.Add(async (next, context, ct) => {
				_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);

				var response = await AuthenticateAsync(ct).Await();

				if (response.Success)
				{
					await next(context, ct).Await();
				}

				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
			})
			.Add(async (next, context, ct) => {
				_logger.LogTrace("Middleware: start {connectionId}", context.ConnectionId);

				await StartEventLoopsAsync(ct).Await();

				_logger.LogTrace("Middleware: end {connectionId}", context.ConnectionId);
			})
			.Build();
	}

	private IMittoClientConnection GetOrCreateConnection(bool disposeAndCreateConnection = false)
	{
		if (_connection == null || disposeAndCreateConnection)
		{
			_connection?.Dispose();
			_connection = MittoConnection.CreateClient(_options);
		}

		return _connection;
	}
}
