namespace Transmitto.Publishers;

public interface ITransmittoMessagePublisher<in TMessage>
{
	Task<MessagePublisherResult> Pulblish(string topic, TMessage message);
}

public class MessagePublisherResult
{
	public static readonly MessagePublisherResult Success = new();

	public MessagePublisherResult()
	{
	}

	public bool Published { get; set; }

	public Exception? Exception { get; set; }
}