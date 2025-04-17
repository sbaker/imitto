using IMitto.Producers;

namespace IMitto.Protocols;

public record PackagedGoods<TGoods> : PackagedGoods
{
	public PackagedGoods(TGoods product, string topic) : base(typeof(TGoods), product!, topic)
	{
	}
}

public record PackagedGoods
{
	internal static EmptyPackagedGoods Empty(string topic) => new(topic);

	public PackagedGoods()
	{
	}

	public PackagedGoods(Type producType, string topic) : this(producType, default!, topic)
	{
	}

	public PackagedGoods(Type producType, object goods, string topic)
	{
		CreatedAt = DateTime.UtcNow;
		ProductType = producType.FullName!;
		ProductName = producType.Name;
		Goods = goods;
		Topic = topic;
	}

	public DateTime CreatedAt { get; set; }

	public string ProductType { get; set; }

	public string ProductName { get; set; }

	public string Topic { get; set; }

	public object Goods { get; set; }

	public bool IsEmpty() => Goods == null || Goods is EmptyPackagedGoods;

	public static PackagedGoods<TGoods> From<TGoods>(string topic, PackageProductionResult<TGoods> packageProduction)
		=> packageProduction.GetPackagedGoods(topic);

	public static PackagedGoods From(string topic, PackageProductionResult packageProduction)
		=> packageProduction.GetPackagedGoods(topic);

	public static PackagedGoods FromException(string topic, Exception exception)
		=> new ExceptionPackagedGoods(topic, exception);
}
