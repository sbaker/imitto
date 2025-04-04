using Transmitto.Net.Settings;

namespace Transmitto.Net.Server;

public class TransmittoServerOptions : TransmittoBaseOptions
{
	private static readonly string DefaultConnectionName = "transmitto";

	public string Name { get; set; } = DefaultConnectionName;
}