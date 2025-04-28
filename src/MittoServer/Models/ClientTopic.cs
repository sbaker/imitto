namespace MittoServer.Models;


public class ClientTopic
{
	public string ClientId { get; set; }

	public Client Client { get; set; }

	public string TopicId { get; set; }

	public Topic Topic { get; set; }
}
