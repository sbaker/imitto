namespace IMitto.Consumers;

public interface IMittoPackageConsumer<in TPackage>
{
	Task<PackageConsumptionResult> ConsumeAsync(TPackage goods);
}

public class PackageConsumptionResult(bool consumed)
{
	public bool Consumed { get; set; } = consumed;

	public Exception? Exception { get; set; }
}
