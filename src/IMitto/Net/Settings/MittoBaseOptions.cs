using System.Net;
using System.Text;

namespace IMitto.Net.Settings;

public abstract class MittoBaseOptions
{
	public Encoding Encoding { get; set; } = Encoding.UTF8;

	public MittoJsonOptions Json { get; set; } = new MittoJsonOptions();

	public int MaxConnectionRetries { get; set; } = 5;

	public MittoConnectionOptions Connection { get; } = new MittoConnectionOptions()
	{
		Host = new MittoHost(IPAddress.Loopback)
	};
}
