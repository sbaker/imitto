using IMitto.Channels;
using IMitto.Consumers;
using IMitto.Local;
using IMitto.Net.Models;
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
	private readonly IMittoChannelReaderProvider<PackagedGoods> _readerProvider;
	private readonly ConcurrentDictionary<string, TopicPackageTypeMapping> _topicTypeMappings;

	public MittoClientEventManager(
		IOptions<MittoClientOptions> options,
		IServiceProvider serviceProvider,
		IMittoChannelReaderProvider<PackagedGoods> readerProvider) : base(Opt.Create(options.Value.Events))
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

	public Task HandleClientEventReceived(EventNotificationsModel clientEvent, CancellationToken token)
	{
		


		return Task.CompletedTask;
	}

	private void ConfigureTopicMapping(string topic, TopicPackageTypeMapping topicMapping)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic);



	}

	//private sealed class 

	private sealed class PackageConsumerCallInvoker
	{
		private static readonly Type PackageConsumerType = typeof(IMittoPackageConsumer<>);
		
		//Task<PackageProductionResult> ProduceAsync(string topic, TPackage goods);
		public PackageConsumerCallInvoker(Func<string, Task<PackageProductionResult>> producer)
		{
			InvocationCount = 0;
		}

		public int InvocationCount { get; } = 0;

		public Task<PackagedGoods> InvokeProduceAsync(string topic) => Task.FromResult(
			(PackagedGoods)PackagedGoods.From(topic, PackageProductionResult.Success("Message from client!")));
	}
}
