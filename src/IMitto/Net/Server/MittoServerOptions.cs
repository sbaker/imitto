using IMitto.Net.Settings;

namespace IMitto.Net.Server;

public class MittoServerOptions : MittoBaseOptions
{
	private static readonly string DefaultConnectionName = "imitto-server";

	public string Name { get; set; } = DefaultConnectionName;
}