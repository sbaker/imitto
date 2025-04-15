using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using IMitto.Channels;
using IMitto.Local;
using Microsoft.Extensions.Options;
using IMitto.Settings;

namespace IMitto.Net.Server;

public sealed class ServerEventManager : MittoLocalEvents, IServerEventManager
{
	private readonly ConcurrentBag<ClientConnectionContext> _connections = [];
	private readonly ILogger<ServerEventManager> _logger;
	private readonly IMittoChannelProvider<ConnectionContext> _connectionChannelProvider;
	private readonly IMittoChannelProvider<ServerEventNotificationsContext> _eventRoutingChannelProvider;
	private CancellationTokenSource? _tokenSource;

	public ServerEventManager(
		IOptions<MittoEventsOptions> options,
		ILogger<ServerEventManager> logger,
		IMittoEventListener eventListener,
		IMittoChannelProvider<ConnectionContext> connectionChannelProvider,
		IMittoChannelProvider<ServerEventNotificationsContext> eventRoutingChannelProvider)
		: base(options)
	{
		_logger = logger;
		_connectionChannelProvider = connectionChannelProvider;
		_eventRoutingChannelProvider = eventRoutingChannelProvider;
	}

	private CancellationTokenSource TokenSource => GetOrCreateTokenSource();

	private CancellationTokenSource GetOrCreateTokenSource(CancellationToken? token = null)
	{
		if (_tokenSource != null)
		{
			return _tokenSource;
		}

		return _tokenSource = !token.HasValue
			? new CancellationTokenSource()
			: CreateLinkedSource(token.Value);
	}

	public Task RunAsync(CancellationToken token)
	{
		var startingTask = Task.Run(async () =>
		{
			try
			{
				var pollForEventsTask = Task.Run(async () =>
				{
					var channelReader = _connectionChannelProvider.GetReader();
					
					while (await channelReader.WaitToReadAsync(token).Await())
					{
						try
						{
							token.ThrowIfCancellationRequested();

							var connection = await channelReader.ReadAsync(token).Await();

							var context = new ClientConnectionContext(connection, token);
							context.SubscribeToEvents();
							_connections.Add(context);
						}
						catch (Exception e)
						{
							_logger.LogError(e, "Failed to publish server event: {message}", e.Message);
						}
					}

				}, token);
				
				var pollForRoutingTask = Task.Run(async () =>
				{
					var channelReader = _eventRoutingChannelProvider.GetReader();

					while (await channelReader.WaitToReadAsync(token).Await())
					{
						try
						{
							token.ThrowIfCancellationRequested();

							var eventNotification = await channelReader.ReadAsync(token).Await();
							var connection = _connections.First(c => c.ConnectionId == eventNotification.ConnectionId);
							await PublishServerEventAsync(eventNotification.Event.Topic, connection.Connection, eventNotification.Event, token).Await();
						}
						catch (Exception e)
						{
							_logger.LogError(e, "Failed to publish server event: {message}", e.Message);
						}
					}

				}, token);

				await Task.WhenAll(pollForEventsTask, pollForRoutingTask).Await();
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

	private CancellationTokenSource CreateLinkedSource(CancellationToken? token = null)
	{
		return CancellationTokenSource.CreateLinkedTokenSource(token ?? default);
	}

	protected override void DisposeCore()
	{
		base.DisposeCore();

		_tokenSource?.Cancel();
		_tokenSource?.Dispose();
	}
}
