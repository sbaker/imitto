using IMitto.Net;
using IMitto.Net.Requests;

namespace IMitto.Net.Protocols
{
	public interface IProtocolTransport
	{
		string Name { get; }

		Task SendAsync<TMittoMessage>(TMittoMessage message, CancellationToken token) where TMittoMessage : IMittoMessage;
	}

}
