using IMitto.Net.Models;
namespace IMitto.Net.Requests;

public class MittoTopicsRequest : MittoRequest<MittoTopicMessageBody>
{
	public MittoTopicsRequest(MittoTopicMessageBody? body = null, MittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
