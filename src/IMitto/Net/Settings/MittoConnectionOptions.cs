namespace IMitto.Net.Settings;

public class MittoConnectionOptions
{
	public int ConnectionTimeout { get; set; } = Timeout.Infinite;

	public required MittoHost Host { get; set; } = new MittoHost();

	public int TaskDelayMilliseconds { get; set; } = 100;
}
