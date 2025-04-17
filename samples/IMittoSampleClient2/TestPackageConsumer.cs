using IMitto.Consumers;
using Microsoft.Extensions.Logging;

namespace IMittoSampleClient2;

public class TestPackageConsumer : IMittoPackageConsumer<TestPackage>
{
	private readonly ILogger<TestPackageConsumer> _logger;

	private int _counter = 0;

	public TestPackageConsumer(ILogger<TestPackageConsumer> logger)
	{
		_logger = logger;
	}

	public Task<PackageConsumptionResult> ConsumeAsync(TestPackage message)
	{
		_logger.LogInformation("TestPackageConsumer: {invocationCount}; {Message}; {MessageUpper}", ++_counter, message.Goods, message.Goods.ToUpper());

		return Task.FromResult(PackageConsumptionResult.Success(true));
	}
}
