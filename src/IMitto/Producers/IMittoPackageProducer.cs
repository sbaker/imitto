namespace IMitto.Producers;

public interface IMittoPackageProducer<in TPackage>
{
	Task<PackageProductionResult> ProduceAsync(string topic, TPackage goods);
}

public class PackageProductionResult
{
	public static readonly PackageProductionResult Success = new();

	public PackageProductionResult()
	{
	}

	public bool Produced { get; set; }

	public Exception? Exception { get; set; }
}