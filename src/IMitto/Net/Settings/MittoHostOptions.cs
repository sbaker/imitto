using System.Net;

namespace IMitto.Net.Settings;

public class MittoHostOptions
{
	private const int _defaultPort = 20220;

	private IPEndPoint _endpoint;

	public MittoHostOptions(IPAddress? address = null, int port = _defaultPort)
	{
		_endpoint = new IPEndPoint(address ?? IPAddress.Any, port);
	}

	internal MittoHostOptions(IPEndPoint address)
	{
		_endpoint = address;
	}

	public IPAddress Address => _endpoint.Address;

	public int Port => _endpoint.Port;

	public IPEndPoint EndPoint => _endpoint;

	public static implicit operator MittoHostOptions(string host)
	{
		return FromString(host);
	}

	public static MittoHostOptions FromString(string host)
	{
		if (int.TryParse(host, out var port))
		{
			return new MittoHostOptions(port: port);
		}

		if (IPAddress.TryParse(host, out var address))
		{
			return new MittoHostOptions(address);
		}

		if (IPEndPoint.TryParse(host, out var endpoint))
		{
			return new MittoHostOptions(endpoint);
		}

		return new MittoHostOptions();
	}
}