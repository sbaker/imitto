namespace Transmitto.Producers;

public interface ITransmittoMessageProducer<in TPackage>
{
	Task<MessageProductionResult> Produce(string topic, TPackage goods);
}

public class MessageProductionResult
{
	public static readonly MessageProductionResult Success = new();

	public MessageProductionResult()
	{
	}

	public bool Produced { get; set; }

	public Exception? Exception { get; set; }
}