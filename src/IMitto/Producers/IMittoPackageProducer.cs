namespace IMitto.Producers;

public class MittoProducerProvider<TPackage> : IMittoProducerProvider<TPackage>
{
	public IMittoProducer<TPackage> GetProducerForTopic(string topic)
	{
		throw new NotImplementedException();
	}
}
