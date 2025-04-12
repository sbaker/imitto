namespace IMitto.Consumers;

public interface IMittoPackageConsumer<in TPackage>
{
	Task<PackageConsumptionResult> ConsumeAsync(TPackage goods);
}

public class PackageConsumptionResult(bool consumed)
{
	public bool Consumed { get; set; } = consumed;

	public Exception? Exception { get; set; }

	public static PackageConsumptionResult Error(Exception? exception)
	{
		return new PackageConsumptionResult(false)
		{
			Exception = exception
		};
	}

	public static PackageConsumptionResult Success(bool consumed)
	{
		return new PackageConsumptionResult(consumed);
	}
}
