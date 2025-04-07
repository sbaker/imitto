namespace IMitto.Net.Settings;

public class MittoConnectionOptions
{
	public int ConnectionTimeout { get; set; } = Timeout.Infinite;

	public required MittoHostOptions Host { get; set; } = new MittoHostOptions();

	public int TaskDelayMilliseconds { get; set; } = 100;
}
