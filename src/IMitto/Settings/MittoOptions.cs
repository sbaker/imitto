using System.Net;
using System.Text;

namespace IMitto.Settings;

public class MittoOptions
{
	public static readonly MittoOptions Default = new();

	public MittoOptions()
	{
		Pipeline = new MittoPipeOptions(this);
	}

	public MittoPipeOptions Pipeline { get; set; }

	public MittoConnectionOptions Connection { get; } = new MittoConnectionOptions()
	{
		Host = new MittoHostOptions(IPAddress.Loopback)
	};

	public Encoding Encoding { get; set; } = Encoding.UTF8;

	public MittoEventsOptions Events { get; set; } = new MittoEventsOptions();

	public MittoJsonOptions Json { get; set; } = new MittoJsonOptions();

	public int MaxConnectionRetries { get; set; } = 5;

	public int StoppingTimeoutInSeconds { get; set; } = 5;
	
	public bool EnableSocketPipelines { get; set; } = true;
}
