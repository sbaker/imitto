using IMitto.Protocols.Models;
namespace IMitto.Protocols.Requests;

public class MittoTopicsRequest : MittoRequest<MittoTopicMessageBody>
{
	public MittoTopicsRequest(MittoTopicMessageBody? body = null, MittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
