namespace IMitto.Producers;

public record PackageProductionResult
{
	public PackageProductionResult(Exception? exception = null)
	{
		Exception = exception;
		Produced = exception == null;
	}

	public bool Produced { get; set; }

	public Exception? Exception { get; set; }

	public static PackageProductionResult<TPackage> Success<TPackage>(TPackage package) => new(package);

	public static PackageProductionResult<TPackage> Failure<TPackage>(Exception exception) => new(exception);

	public virtual PackagedGoods GetPackagedGoods(string topic)
	{
		if (Exception != null)
		{
			return new ExceptionPackagedGoods(topic, Exception);
		}
		
		return PackagedGoods.Empty(topic);
	}
}

public record PackageProductionResult<TPackage>(TPackage Package, Exception? Exception = null) : PackageProductionResult(Exception)
{
	public PackageProductionResult(Exception exception) : this(default!, null)
	{
		Exception = exception;
		Produced = false;
	}

	public override PackagedGoods<TPackage> GetPackagedGoods(string topic)
	{
		return new PackagedGoods<TPackage>(Package, topic);
	}
}