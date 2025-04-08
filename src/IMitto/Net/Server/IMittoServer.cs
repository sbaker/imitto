using IMitto.Hosting;

namespace IMitto.Net.Server;

public interface IMittoServer : IMittoHost
{
	string Name { get; }
}
