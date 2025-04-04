using IMitto.Net.Settings;

namespace IMitto.Net.Server;

public class TransmittoServerOptions : TransmittoBaseOptions
{
	private static readonly string DefaultConnectionName = "transmitto";

	public string Name { get; set; } = DefaultConnectionName;
}