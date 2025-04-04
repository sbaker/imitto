using System.Net;
using System.Text;

namespace Transmitto.Net.Settings;

public abstract class TransmittoBaseOptions
{
	public Encoding Encoding { get; set; } = Encoding.UTF8;

	public TransmittoJsonOptions Json { get; set; } = new TransmittoJsonOptions();

	public int MaxConnectionRetries { get; set; } = 5;

	public TransmittoConnectionOptions ConnectionOptions { get; } = new TransmittoConnectionOptions()
	{
		Host = new TransmittoHost(IPAddress.Loopback)
	};
}
