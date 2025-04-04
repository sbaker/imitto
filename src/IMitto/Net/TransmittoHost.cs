using System.Net;

namespace IMitto.Net;

public class TransmittoHost
{
	internal const int DefaultPort = 20220;

	private IPEndPoint _endpoint;

	public TransmittoHost(IPAddress? address = null, int port = DefaultPort)
	{
		_endpoint = new IPEndPoint(address ?? IPAddress.Any, port);
	}

	internal TransmittoHost(IPEndPoint address)
	{
		_endpoint = address;
	}

	public IPAddress Address => _endpoint.Address;

	public int Port => _endpoint.Port;

	public IPEndPoint EndPoint => _endpoint;

	public static implicit operator TransmittoHost(string host)
	{
		return TransmittoHost.FromString(host);
	}

	public static TransmittoHost FromString(string host)
	{
		if (int.TryParse(host, out var port))
		{
			return new TransmittoHost(port: port);
		}

		if (IPAddress.TryParse(host, out var address))
		{
			return new TransmittoHost(address);
		}

		if (IPEndPoint.TryParse(host, out var endpoint))
		{
			return new TransmittoHost(endpoint);
		}

		return new TransmittoHost();
	}
}