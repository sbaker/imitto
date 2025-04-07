using IMitto.Net.Clients;

namespace IMitto.Producers;

public class MittoProducerProvider<TPackage> : IMittoProducerProvider<TPackage>
{
	public IMittoProducer<TPackage> GetProducerForTopic(string topic)
	{
		throw new NotImplementedException();
	}
}

public interface IMittoProducerProvider<TPackage>
{
	IMittoProducer<TPackage> GetProducerForTopic(string topic);
}

public interface IMittoProducer<TPackage>
{
	Task<PackageProductionResult<TPackage>> ProduceAsync();
}

public abstract record PackageProductionResult
{
	public PackageProductionResult(bool produced = true)
	{
		Produced = produced;
	}

	public bool Produced { get; set; }

	public Exception? Exception { get; set; }

	public static PackageProductionResult<TPackage> Success<TPackage>(TPackage package) => new(package);

	public static PackageProductionResult<TPackage> Failure<TPackage>(TPackage package) => new(package);

	public abstract PackagedGoods GetPackagedGoods(string topic);
}

//public static readonly PackageProductionResult Success<TPackage>() ;

public record PackageProductionResult<TPackage>(TPackage package, bool produced = true) : PackageProductionResult(produced)
{
	public TPackage Package { get; } = package;

	public override PackagedGoods<TPackage> GetPackagedGoods(string topic)
	{
		return new PackagedGoods<TPackage>(Package, topic);
	}
}