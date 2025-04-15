namespace IMitto.Protocols.Models;

public class MittoTopicMessageBody : MittoMessageBody
{
	public TopicRegistrationModel? Topics { get; set; }
}