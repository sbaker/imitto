using Transmitto.Net.Models;

namespace Transmitto.Net;

public interface ITransmittoMessage
{
	TransmittoHeader Header { get; set; }

	bool HasBody();

	TransmittoMessageBody GetBody();

	TBody GetBody<TBody>() where TBody : notnull, TransmittoMessageBody;
}
public interface ITransmittoMessage<TBody> where TBody : TransmittoMessageBody
{
	TBody Body { get; set; }

	TransmittoHeader Header { get; set; }
}
