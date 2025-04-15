using IMitto.Net.Clients;
using Microsoft.Extensions.Options;
using IMitto.Protocols;
using IMitto.Protocols.Models;

namespace IMitto.Producers;

public sealed class MittoProducerProvider<TPackage> : IMittoProducerProvider<TPackage>
{
	private readonly MittoClientOptions _options;
	private readonly IMittoEventDispatcher _eventDispatcher;

	public MittoProducerProvider(
		IOptions<MittoClientOptions> options,
		IMittoEventDispatcher eventDispatcher)
	{
		_options = options.Value;
		_eventDispatcher = eventDispatcher;
	}

	public IMittoProducer<TPackage> GetProducerForTopic(string topic)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(topic, nameof(topic));

		return new MittoProducer(topic, result => HandleProviderCallback(topic, result));
	}

	private Task HandleProviderCallback(string topic, PackageProductionResult<TPackage> providerResult)
	{
		return _eventDispatcher.DispatchAsync(new EventNotificationsModel
		{
			Topic = topic,
			Events = 
			[
				new()
				{
					Id = Guid.NewGuid().ToString(),
					Topic = topic,
					Type = MittoEventType.Produce,
					Event = new MittoEvent<TPackage>(providerResult.GetPackagedGoods(topic))
					{
						Topic = topic,
					}
				}
			]
		}).AsTask();
	}

	private sealed class MittoProducer(string topic, Func<PackageProductionResult<TPackage>, Task> callback) : IMittoProducer<TPackage>
	{
		private readonly string _topic = topic;

		public async Task<PackageProductionResult<TPackage>> ProduceAsync(TPackage package)
		{
			ArgumentNullException.ThrowIfNull(package, nameof(package));

			var result = new PackageProductionResult<TPackage>(package);

			await callback(result).Await();

			return result;
		}
	}
}
