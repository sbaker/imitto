using IMitto.Middlware;
using IMitto.Protocols;

namespace IMitto.Net.Server;

public class MittoConnectionContext : MiddlewareContext
{
	public MittoConnectionContext(ConnectionContext context, IMittoPackage package) : base(context.ConnectionId)
	{
		Connection = context;
		Package = package;
	}

	public ConnectionContext Connection { get; private set; }

	public IMittoPackage Package { get; private set; }
}
