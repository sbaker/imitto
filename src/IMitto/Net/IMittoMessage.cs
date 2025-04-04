using IMitto.Net.Models;

namespace IMitto.Net;

public interface IMittoMessage
{
	TransmittoHeader Header { get; set; }

	bool HasBody();

	TransmittoMessageBody GetBody();

	TBody GetBody<TBody>() where TBody : notnull, TransmittoMessageBody;
}
public interface IMittoMessage<TBody> where TBody : TransmittoMessageBody
{
	TBody Body { get; set; }

	TransmittoHeader Header { get; set; }
}
