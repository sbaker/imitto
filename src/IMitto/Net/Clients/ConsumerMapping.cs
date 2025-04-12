using System.Reflection;

namespace IMitto.Net.Clients;

public class ConsumerMapping
{
	public ConsumerMapping(List<object> consumers, MethodInfo consumeMethod, Type packageType)
	{
		Consumers = consumers;
		ConsumeMethod = consumeMethod;
		PackageType = packageType;
	}

	public List<object> Consumers { get; } = [];

	public MethodInfo ConsumeMethod { get; set; }

	public Type PackageType { get; }
}
