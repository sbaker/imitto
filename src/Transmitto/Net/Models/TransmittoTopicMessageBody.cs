namespace Transmitto.Net.Models;

public class TransmittoTopicMessageBody : TransmittoMessageBody
{
	public string[]? Topics { get; set; }

	public static implicit operator TransmittoTopicMessageBody(string[]? content)
	{
		return new TransmittoTopicMessageBody { Topics = content };
	}

	public static implicit operator string[]?(TransmittoTopicMessageBody content)
	{
		return content.Topics;
	}
}

public class EventNotificationsBody : TransmittoMessageBody<EventNotificationsModel>
{
	
}

public class EventNotificationsModel
{
	public string MessageId { get; set; }

	public List<EventNotification> Events { get; set; }
}

public class EventNotification
{
	public string Id { get; set; }

	public string Type { get; set; }

	public string Topic { get; set; }

	public TransmittoEvent Event { get; set; }
}

public class TransmittoEvent
{
	public string RawMessage { get; set; }
}

public class TransmittoEvent<TMessage> : TransmittoEvent
{
	public TMessage? Message { get; set; }
}