using Microsoft.Extensions.Logging;
using IMitto.Middlware;
using IMitto.Settings;
using IMitto.Protocols.Models;
using IMitto.Protocols.Requests;
using IMitto.Protocols;

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

				var transport = MittoProtocol.DefaultProtocolTransport;

				try
				{
					var package = await transport.ReadPackageAsync(context.Socket.GetReader(), token).Await();

					if (!package.Command.Action.HasFlag(MittoAction.Connect))
					{
						var mustConnectPackage = MittoProtocol.CreatePackageBuilder()
							.WithAction(MittoAction.Connect)
							.WithModifier(MittoModifier.Error)
							.AddHeader(MittoHeaderKey.StatusCode, MittoModifier.Error.ToString())
							.Build();
						await transport.WritePackageAsync(context.Socket.GetWriter(), mustConnectPackage, token);
						break;
					}

					var mittoContext = new MittoConnectionContext(context, package);

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
