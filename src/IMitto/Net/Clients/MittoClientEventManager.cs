using IMitto.Channels;
using IMitto.Consumers;
using IMitto.Local;
using IMitto.Net.Models;
using IMitto.Net.Requests;
using IMitto.Net.Responses;
using IMitto.Producers;
using IMitto.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Opt = Microsoft.Extensions.Options.Options;

namespace IMitto.Net.Clients;

public class MittoClientEventManager : MittoLocalEvents, IMittoClientEventManager
{
	private readonly MittoClientOptions _options;
	private readonly IServiceProvider _serviceProvider;
	private readonly IMittoChannelReaderProvider<EventNotificationsModel> _readerProvider;
	private readonly ConcurrentDictionary<string, TopicPackageTypeMapping> _topicTypeMappings;

	public MittoClientEventManager(
		IOptions<MittoClientOptions> options,
		IServiceProvider serviceProvider,
		IMittoChannelReaderProvider<EventNotificationsModel> readerProvider) : base(Opt.Create(options.Value.Events))
	{
		_options = options.Value;
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

		while (await reader.WaitToReadAsync(token))
		{
			if (!connection.IsConnected())
			{
				throw new InvalidOperationException("Socket is not connected or not initialized.");
			}

			var package = await reader.ReadAsync(token);

			var eventNotificationBody = new EventNotificationsBody
			{
				Content = package,
			};

			await connection.SendRequestAsync(new EventNotificationRequest(eventNotificationBody), token);
		}
	}

	public async Task<EventNotificationsModel> WaitForServerEventsAsync(IMittoClientConnection connection, CancellationToken token)
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
				Action = MittoEventType.Consume
			},
			Body = new()
			{
				Topics = new TopicRegistrationModel
				{
					ProduceTopics = producerTopics,
					ConsumeTopics = consumerTopics,
				}
			}
		}, token);

		while (!connection.IsDataAvailable())
		{
			token.ThrowIfCancellationRequested();
			await Task.Delay(_options.Connection.TaskDelayMilliseconds, token);
		}

		var response = await connection.ReadResponseAsync<EventNotificationsResponse>(token);

		if (response is not null && response.Header.Path == MittoPaths.Topics)
		{
			var eventNotifications = response.Body;

			return eventNotifications.Content!;
		}

		return null!;
	}

	private void ConfigureTopicMapping(string topic, TopicPackageTypeMapping topicMapping)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);



	}

	private sealed class PackageConsumerCallInvoker
	{
		private static readonly Type PackageConsumerType = typeof(IMittoPackageConsumer<>);
		
		//Task<PackageProductionResult> ProduceAsync(string topic, TPackage goods);
		public PackageConsumerCallInvoker(Func<string, Task<PackageProductionResult>> producer)
		{
			InvocationCount = 0;
		}

		public int InvocationCount { get; } = 0;

		public Task<PackagedGoods> InvokeConsumeAsync(string topic) => Task.FromResult(
			(PackagedGoods)PackagedGoods.From(topic, PackageProductionResult.Success("Message from client!")));
	}
}
