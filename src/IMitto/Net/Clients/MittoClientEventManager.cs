using IMitto.Channels;
using IMitto.Consumers;
using IMitto.Local;
using IMitto.Net.Models;
using IMitto.Net.Settings;
using IMitto.Producers;
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

		public int InvocationCount { get; }

		public Task<PackagedGoods> InvokeProduceAsync(string topic) => Task.FromResult(
			(PackagedGoods)PackagedGoods.From(topic, PackageProductionResult.Success("Message from client!")));
	}
}

public record PackagedGoods<TGoods>(TGoods Product, string Topic) : PackagedGoods(typeof(TGoods), Product!, Topic);

public record PackagedGoods
{
	public PackagedGoods(Type producType, object goods, string topic)
	{
		ProductType = producType;
		ProductName = producType.Name;
		Goods = goods;
		Topic = topic;
	}

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public Type ProductType { get; }

	public string ProductName { get; }

	public string Topic { get; set; }

	public object Goods { get; }

	public static PackagedGoods<TGoods> From<TGoods>(string topic, PackageProductionResult<TGoods> packageProduction)
		=> packageProduction.GetPackagedGoods(topic);

	public static PackagedGoods From(string topic, PackageProductionResult packageProduction)
		=> packageProduction.GetPackagedGoods(topic);
}
