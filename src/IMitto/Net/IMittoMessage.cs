using IMitto.Net.Models;

namespace IMitto.Net;

public interface IMittoMessage
{
	MittoHeader Header { get; set; }

	bool HasBody();

	MittoMessageBody GetBody();

	TBody GetBody<TBody>() where TBody : notnull, MittoMessageBody;
}
public interface IMittoMessage<TBody> where TBody : MittoMessageBody
{
	TBody Body { get; set; }

	MittoHeader Header { get; set; }
}
