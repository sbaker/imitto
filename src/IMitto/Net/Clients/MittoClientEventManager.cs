using IMitto;
using IMitto.Channels;
using IMitto.Consumers;
using IMitto.Local;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using IMitto.Producers;
using IMitto.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Opt = Microsoft.Extensions.Options.Options;

namespace IMitto.Net.Clients;

public class MittoClientEventManager : MittoLocalEvents, IMittoClientEventManager
{
	private static readonly Type PackageConsumerType = typeof(IMittoPackageConsumer<>);
	private static readonly MethodInfo PackageConsumeMethod = PackageConsumerType.GetMethod(nameof(IMittoPackageConsumer<int>.ConsumeAsync))!;

	private readonly MittoClientOptions _options;
	private readonly ILogger<MittoClientEventManager> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly IMittoChannelReaderProvider<EventNotificationsModel> _readerProvider;
	private readonly ConcurrentDictionary<string, TopicPackageTypeMapping> _topicTypeMappings;

	public MittoClientEventManager(
		ILogger<MittoClientEventManager> logger,
		IOptions<MittoClientOptions> options,
		IServiceProvider serviceProvider,
		IMittoChannelReaderProvider<EventNotificationsModel> readerProvider) : base(Opt.Create(options.Value.Events))
	{
		_options = options.Value;
		_logger = logger;
		_serviceProvider = serviceProvider;
		_readerProvider = readerProvider;
		_topicTypeMappings = _options.TypeMappings;

		foreach (var mapping in _topicTypeMappings)
		{
			ConfigureTopicMapping(mapping.Key, mapping.Value);
		}
	}

	public async Task WaitForClientEventsAsync(IMittoClientConnection connection, CancellationToken token)
	{
		var reader = _readerProvider.GetReader();

		while (await reader.WaitToReadAsync(token).Await())
		{
			if (!connection.IsConnected())
			{
				throw new InvalidOperationException("Socket is not connected or not initialized.");
			}

			var package = await reader.ReadAsync(token).Await();

			var eventNotificationBody = new EventNotificationsBody
			{
				Content = package,
			};

			await connection.SendRequestAsync(new EventNotificationRequest(eventNotificationBody), token).Await();
		}
	}

	public async Task WaitForServerEventsAsync(IMittoClientConnection connection, CancellationToken token)
	{
		if (!connection.IsConnected())
		{
			throw new InvalidOperationException("Socket is not connected or not initialized.");
		}

		var consumerTopics = _options.TypeMappings.Where(x => x.Value.IsConsumer).Select(x => x.Key).ToArray();
		var producerTopics = _options.TypeMappings.Where(x => x.Value.IsProducer).Select(x => x.Key).ToArray();

		await connection!.SendRequestAsync(new MittoTopicsRequest
		{
			Header = new()
			{
				Path = MittoPaths.Topics,
				Action = MittoEventType.Consume,
				Version = MittoConstants.Version,
			},
			Body = new()
			{
				Topics = new TopicRegistrationModel
				{
					ProduceTopics = producerTopics,
					ConsumeTopics = consumerTopics,
				}
			}
		}, token).Await();

		while (!connection.IsDataAvailable())
		{
			token.ThrowIfCancellationRequested();
			await Task.Delay(_options.Connection.TaskDelayMilliseconds, token).Await();
		}

		var statusResponse = (await connection.ReadResponseAsync<MittoStatusResponse>(token).Await())!;

		if (statusResponse.Body.Status.Success)
		{
			await PollForEventNotifications(connection, token).Await();
		}
	}

	private async Task PollForEventNotifications(IMittoClientConnection connection, CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			while (!connection.IsDataAvailable())
			{
				token.ThrowIfCancellationRequested();
				await Task.Delay(_options.Connection.TaskDelayMilliseconds, token).Await();
			}

			var response = await connection.ReadResponseAsync<MittoResponse<EventNotificationsBody>>(token).Await();

			if (response is not null && response.Header.Path == MittoPaths.Topics)
			{
				var eventNotifications = response.Body;

				if (eventNotifications?.Content == null)
				{
					throw new InvalidOperationException("Event notification is null.");
				}

				await PublishAsync(eventNotifications.Content.Topic, eventNotifications.Content).Await();
			}
		}
	}

	private void ConfigureTopicMapping(string topic, TopicPackageTypeMapping topicMapping)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);

		//Task<PackageConsumptionResult> ConsumeAsync(TPackage goods);
		if (topicMapping.IsConsumer)
		{
			var consumerType = PackageConsumerType.MakeGenericType(topicMapping.PackageType);
			var consumers = _serviceProvider.GetServices(consumerType).ToList();
			var consumeMethod = consumerType.GetMethod(nameof(IMittoPackageConsumer<int>.ConsumeAsync))!;
			var consumerMapping = new ConsumerMapping(consumers!, consumeMethod, topicMapping.PackageType);
			Subscribe(new ConsumerMethodInvokerSubscription(topic, _logger, _options, consumerMapping, this));

			// TODO: Implement a better way of invoking the consumer method using Expressions or IL.
		}
	}

	private sealed class ConsumerMethodInvokerSubscription : Subscription
	{
		private readonly ConsumerMapping _consumerMapping;
		private readonly MittoClientOptions _options;
		private readonly ILogger _logger;

		public ConsumerMethodInvokerSubscription(string topic, ILogger logger, MittoClientOptions options, ConsumerMapping consumerMapping, IMittoEvents events) : base(topic, events)
		{
			_logger = logger;
			_options = options;
			_consumerMapping = consumerMapping;
		}

		protected override void InvokeCore(EventContext context)
		{
			var data = context.GetData();

			if (data is not EventNotificationsModel eventNotification)
			{
				throw new InvalidOperationException($"Invalid data type: {data?.GetType()}");
			}

			// TODO: Not looping through all consumers...Just the lastt
			eventNotification.Events.ForEach(async serverEvent =>
			{
				if (serverEvent.Event.Package is JsonElement jsonElement)
				{
					var package = jsonElement.Deserialize<PackagedGoods>(_options.Json.Serializer);

					if (package?.Goods is not JsonElement elementGoods)
					{
						return;
					}

					var consumers = _consumerMapping.Consumers;
					var goods = elementGoods.Deserialize(_consumerMapping.PackageType, _options.Json.Serializer);

					for (var i = 0; i < consumers.Count; i++)
					{
						var consumer = _consumerMapping.Consumers[i];

						try
						{
							var consumerTask = _consumerMapping.ConsumeMethod.Invoke(consumer, [goods]);

							if (consumerTask is Task<PackageConsumptionResult> task)
							{
								var result = await task.Await();

								if (!result.Consumed)
								{
									throw result.Exception ?? new InvalidOperationException($"Unknown error consuming topic: {serverEvent.Topic}");
								}
							}
						}
						catch (Exception e)
						{
							_logger.LogError(e, "Error while consuming package: {consumerType}; {error}", consumer.GetType().FullName, e.Message);
						}
					}
				}
			});
		}
	}
}
