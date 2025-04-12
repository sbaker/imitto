using IMitto.Consumers;
using Microsoft.Extensions.Logging;

namespace IMittoSampleClient2;

public class NewSubscriberConsumer : IMittoPackageConsumer<SubscriberInfo>
{
	private readonly ILogger<NewSubscriberConsumer> _logger;

	private int _counter = 0;

	public NewSubscriberConsumer(ILogger<NewSubscriberConsumer> logger)
	{
		_logger = logger;
	}

	public Task<PackageConsumptionResult> ConsumeAsync(SubscriberInfo message)
	{
		_logger.LogInformation("TestPackageConsumer2: {invocationCount}; {Username}; {Age}; {SubscribedAt}", ++_counter, message.Username, message.Age, message.SubscribedAt);

		return Task.FromResult(PackageConsumptionResult.Success(true));
	}
}