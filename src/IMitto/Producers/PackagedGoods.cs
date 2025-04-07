namespace IMitto.Producers;

public record PackagedGoods<TGoods>(TGoods Product, string Topic) : PackagedGoods(typeof(TGoods), Product!, Topic);

public record PackagedGoods
{
	internal static EmptyPackagedGoods Empty(string topic) => new(topic);

	public PackagedGoods(Type producType, object goods, string topic)
	{
		CreatedAt = DateTime.UtcNow;
		ProductType = producType;
		ProductName = producType.Name;
		Goods = goods;
		Topic = topic;
	}

	public PackagedGoods(Type producType, string topic) : this(producType, default!, topic)
	{
	}

	public DateTime CreatedAt { get; }

	public Type ProductType { get; }

	public string ProductName { get; }

	public string Topic { get; }

	public object Goods { get; }

	public bool IsEmpty => Goods == null || Goods is EmptyPackagedGoods;

	public static PackagedGoods<TGoods> From<TGoods>(string topic, PackageProductionResult<TGoods> packageProduction)
		=> packageProduction.GetPackagedGoods(topic);

	public static PackagedGoods From(string topic, PackageProductionResult packageProduction)
		=> packageProduction.GetPackagedGoods(topic);

	public static PackagedGoods FromException(string topic, Exception exception)
		=> new ExceptionPackagedGoods(topic, exception);
}
