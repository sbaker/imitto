namespace Transmitto.Consumers;

public interface ITransmittoMessageConsumer<in TPackage>
{
	Task<MessageConsumptionResult> ConsumeAsync(TPackage goods);
}

public class MessageConsumptionResult
{
	public MessageConsumptionResult(bool consumed)
	{
		Consumed = consumed;
	}

	public bool Consumed { get; set; }

	public Exception? Exception { get; set; }
}
