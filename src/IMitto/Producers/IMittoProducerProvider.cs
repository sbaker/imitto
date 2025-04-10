namespace IMitto.Producers;

public interface IMittoProducerProvider<TPackage>
{
	IMittoProducer<TPackage> GetProducerForTopic(string topic);
}
