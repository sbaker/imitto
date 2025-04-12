using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using IMitto.Channels;
using IMitto.Local;
using Microsoft.Extensions.Options;
using IMitto.Settings;
using IMitto.Net.Models;

namespace IMitto.Net.Server;

public sealed class ServerEventManager : MittoLocalEvents, IServerEventManager
{
	private readonly ConcurrentBag<ClientConnectionContext> _connections = [];
	private readonly ILogger<ServerEventManager> _logger;
	private readonly IMittoEventListener _eventListener;
	private readonly IMittoChannelProvider<ConnectionContext> _connectionChannelProvider;
	private readonly IMittoChannelProvider<ServerEventNotificationsContext> _eventRoutingChannelProvider;

	public ServerEventManager(
		IOptions<MittoEventsOptions> options,
		ILogger<ServerEventManager> logger,
		IMittoEventListener eventListener,
		IMittoChannelProvider<ConnectionContext> connectionChannelProvider,
		IMittoChannelProvider<ServerEventNotificationsContext> eventRoutingChannelProvider)
		: base(options)
	{
		_logger = logger;
		_eventListener = eventListener;
		_connectionChannelProvider = connectionChannelProvider;
		_eventRoutingChannelProvider = eventRoutingChannelProvider;
	}

	private CancellationTokenSource TokenSource { get; set; }

	public Task RunAsync(CancellationToken token)
	{
		TokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

		var startingTask = Task.Run(async () => {
			try
			{
				var pollForEventsTask = Task.Run(async () => {
					var channelReader = _connectionChannelProvider.GetReader();

					while (await channelReader.WaitToReadAsync(token))
					{
						token.ThrowIfCancellationRequested();

						var connection = await channelReader.ReadAsync(token);

						var eventLoopTask = _eventListener.PollForEventsAsync(connection, token);
						var context = new ClientConnectionContext(connection, eventLoopTask, token);
						context.SubscribeToEvents();
						_connections.Add(context);
					}
				}, token);
				
				var pollForRoutingTask = Task.Run(async () => {
					var channelReader = _eventRoutingChannelProvider.GetReader();

					while (await channelReader.WaitToReadAsync(token))
					{
						token.ThrowIfCancellationRequested();

						var eventNotification = await channelReader.ReadAsync(token);
						var connection = _connections.First(c => c.ConnectionId == eventNotification.ConnectionId);
						await PublishServerEventAsync(eventNotification.Event.Topic, connection.Connection, eventNotification.Event, token);
					}
				}, token);

				await Task.WhenAll(pollForEventsTask, pollForRoutingTask);
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
