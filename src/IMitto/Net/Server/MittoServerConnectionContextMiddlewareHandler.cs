using Microsoft.Extensions.Logging;
using IMitto.Middlware;
using IMitto.Settings;
using IMitto.Protocols.Models;
using IMitto.Protocols.Requests;

namespace IMitto.Net.Server;

internal class MittoServerConnectionContextMiddlewareHandler : IMiddlewareHandler<ConnectionContext>
{
	private readonly MittoOptions _options;
	private readonly ILogger _logger;
	private readonly IMiddlewareHandler<MittoConnectionContext> _innerHandler;

	public MittoServerConnectionContextMiddlewareHandler(MittoOptions options, ILogger logger, IMiddlewareHandler<MittoConnectionContext> innerHandler)
	{
		_options = options;
		_logger = logger;
		_innerHandler = innerHandler;
	}

	public Task HandleAsync(ConnectionContext context, CancellationToken token)
	{
		return Task.Run(async () =>
		{
			while (!token.IsCancellationRequested)
			{
				while (!context.Socket.DataAvailable)
				{
					token.ThrowIfCancellationRequested();
					await Task.Delay(_options.StoppingTimeoutInSeconds, token).Await();

					if (context.Disposed)
					{
						_logger.LogWarning("Connection Disposed: {connectionId}", context.ConnectionId);
						return;
					}
				}

				try
				{
					var request = await context.Socket.ReadAsync<MittoRequest<MittoMessageBody>>(token).Await();

					if (request == null)
					{
						break;
					}

					var mittoContext = new MittoConnectionContext(context, request);

					await _innerHandler.HandleAsync(mittoContext, token).Await();
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error Handling request: {connectionId}", context.ConnectionId);
				}
			}
		}, token);
	}
}
