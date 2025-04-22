namespace IMitto.Consumers;

public interface IMittoPackageConsumer<in TPackage>
{
	Task<PackageConsumptionResult> ConsumeAsync(TPackage goods);
}
