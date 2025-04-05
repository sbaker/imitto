using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using IMitto.Channels;
using IMitto.Extensions;
using IMitto.Net;
using IMitto.Net.Responses;
using IMitto.Local;
using Microsoft.Extensions.Options;

namespace IMitto.Net.Server;

public sealed class ServerEventManager : MittoLocalEvents, IServerEventManager
{
	private readonly ConcurrentBag<ClientConnectionContext> _connections = [];
	private readonly ILogger<ServerEventManager> _logger;
	private readonly IMittoEventListener _eventListener;
	private readonly IMittoChannelProvider<ConnectionContext> _channelProvider;

	public ServerEventManager(
		IOptions<MittoEventsOptions> options,
		ILogger<ServerEventManager> logger,
		IMittoEventListener eventListener,
		IMittoChannelProvider<ConnectionContext> channelProvider)
		: base(options)
	{
		_logger = logger;
		_eventListener = eventListener;
		_channelProvider = channelProvider;
	}

	private CancellationTokenSource TokenSource { get; set; }

	public Task RunAsync(CancellationToken token)
	{
		TokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

		var startingTask = Task.Run(async () =>
		{
			try
			{
				await Task.Run(async () =>
				{
					var channelReader = _channelProvider.GetReader();

					while (await channelReader.WaitToReadAsync(token))
					{
						var connection = await channelReader.ReadAsync(token);
						var eventLoopTask = _eventListener.PollForEventsAsync(connection, token);
						_connections.Add(new ClientConnectionContext(connection, eventLoopTask, token));
					}
				});
			}
			catch (TaskCanceledException tce)
			{
				_logger.LogWarning(tce, "Task canceled");
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Server failed to accept new connection: unknown error.");
			}

		}, TokenSource.Token);

		return startingTask;
	}

	public Task PublishServerEventAsync<TData>(EventId eventId, ConnectionContext context, TData data, CancellationToken token)
	{
		if (!Repository.TryGet(MittoEventsId, eventId, out var subscriptions))
		{
			return Task.CompletedTask;
		}

		return Task.Run(() =>
		{
			var eventContext = ServerEventContext.For(eventId, context, data);
			
			foreach (var subscription in subscriptions)
			{
				subscription.Invoke(eventContext);
			}
		}, token);
	}
}
