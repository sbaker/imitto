using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using Transmitto.Channels;
using Transmitto.Extensions;
using Transmitto.Net;
using Transmitto.Net.Responses;
using Transmitto.Net.Server;

namespace Transmitto.Server;

public sealed class ServerEventManager : EventAggregator, IServerEventManager
{
	private readonly ConcurrentBag<ClientConnectionContext> _connections = [];

	private readonly ILogger<ServerEventManager> _logger;
	private readonly ITransmittoEventListener _eventListener;
	private readonly ITransmittoChannelProvider<ConnectionContext> _channelProvider;

	public ServerEventManager(ILogger<ServerEventManager> logger, ITransmittoEventListener eventListener, ITransmittoChannelProvider<ConnectionContext> channelProvider) : base(SubscriberDefaults.InMemory)
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
						_connections.Add(new ClientConnectionContext(connection, eventLoopTask));
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
		if (!Repository.TryGet(eventId, out var subscriptions))
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
		});
	}
}
