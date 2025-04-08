using IMitto.Consumers;

namespace IMittoSampleClient2;

public class TestPackageConsumer : IMittoPackageConsumer<TestPackage>
{
	public Task<PackageConsumptionResult> ConsumeAsync(TestPackage message)
	{
		throw new NotImplementedException();
	}
}