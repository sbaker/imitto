using IMitto.Consumers;

namespace IMittoSampleClient1;

public class TestPackageConsumer : IMittoPackageConsumer<TestPackage>
{
	public Task<PackageConsumptionResult> ConsumeAsync(TestPackage message)
	{
		throw new NotImplementedException();
	}
}