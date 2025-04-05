using IMitto.Producers;

namespace IMittoSampleClient1;

public class TestPackageProducer : IMittoPackageProducer<TestPackage>
{
	public Task<PackageProductionResult> ProduceAsync(string topic, TestPackage message)
	{
		throw new NotImplementedException();
	}
}