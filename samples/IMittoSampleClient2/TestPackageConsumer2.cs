using IMitto.Consumers;
using Microsoft.Extensions.Logging;

namespace IMittoSampleClient2;

public class TestPackageConsumer2 : IMittoPackageConsumer<TestPackage>
{
	private readonly ILogger<TestPackageConsumer2> _logger;

	private int _counter = 0;

	public TestPackageConsumer2(ILogger<TestPackageConsumer2> logger)
	{
		_logger = logger;
	}

	public Task<PackageConsumptionResult> ConsumeAsync(TestPackage message)
	{
		_logger.LogInformation("TestPackageConsumer2: {invocationCount}; {Message}; {MessageUpper}", ++_counter, message.Goods, message.Goods.ToUpper());

		return Task.FromResult(PackageConsumptionResult.Success(true));
	}
}