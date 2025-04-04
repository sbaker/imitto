using IMitto.Net.Models;
namespace IMitto.Net.Requests;

public class TransmittoTopicsRequest : TransmittoRequest<TransmittoTopicMessageBody>
{
	public TransmittoTopicsRequest(TransmittoTopicMessageBody? body = null, TransmittoHeader? header = null)
		: base(body ?? new(), header ?? [])
	{
	}
}
