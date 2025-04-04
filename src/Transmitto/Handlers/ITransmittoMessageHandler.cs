namespace Transmitto.Handlers
{
	public interface ITransmittoMessageHandler<in TMessage>
	{
		Task<MessageHandlerResult> HandleAsync(TMessage message);
	}

	public class MessageHandlerResult
	{
		public MessageHandlerResult(bool isHandled)
		{
			Handled = isHandled;
		}

		public bool Handled { get; set; }

		public Exception? Exception { get; set; }
	}
}
