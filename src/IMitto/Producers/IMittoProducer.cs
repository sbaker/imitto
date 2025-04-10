namespace IMitto.Producers;

public interface IMittoProducer<TPackage>
{
	Task<PackageProductionResult<TPackage>> ProduceAsync(TPackage package);
}