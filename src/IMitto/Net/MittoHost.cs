using System.Net;

namespace IMitto.Net;

public class MittoHost
{
	internal const int DefaultPort = 20220;

	private IPEndPoint _endpoint;

	public MittoHost(IPAddress? address = null, int port = DefaultPort)
	{
		_endpoint = new IPEndPoint(address ?? IPAddress.Any, port);
	}

	internal MittoHost(IPEndPoint address)
	{
		_endpoint = address;
	}

	public IPAddress Address => _endpoint.Address;

	public int Port => _endpoint.Port;

	public IPEndPoint EndPoint => _endpoint;

	public static implicit operator MittoHost(string host)
	{
		return MittoHost.FromString(host);
	}

	public static MittoHost FromString(string host)
	{
		if (int.TryParse(host, out var port))
		{
			return new MittoHost(port: port);
		}

		if (IPAddress.TryParse(host, out var address))
		{
			return new MittoHost(address);
		}

		if (IPEndPoint.TryParse(host, out var endpoint))
		{
			return new MittoHost(endpoint);
		}

		return new MittoHost();
	}
}