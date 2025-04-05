namespace IMitto.Net.Models;

public class MittoTopicMessageBody : MittoMessageBody
{
	public TopicRegistrationModel? Topics { get; set; }
}