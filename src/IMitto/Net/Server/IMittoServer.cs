using IMitto.Hosting;

namespace IMitto.Net.Server;

public interface IMittoServer : IMittoHost
{
	MittoServerOptions Options { get; }

	string Name { get; }

	bool Running { get; }
}
