using IMitto.Settings;

namespace IMitto.Net.Server;

public class MittoServerOptions : MittoOptions
{
	private static readonly string DefaultConnectionName = "imitto-server";

	public string Name { get; set; } = DefaultConnectionName;
}