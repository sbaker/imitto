namespace IMitto.Net.Settings;

public class TransmittoConnectionOptions
{
	public int ConnectionTimeout { get; set; } = Timeout.Infinite;

	public required TransmittoHost Host { get; set; } = new TransmittoHost();

	public int TaskDelayMilliseconds { get; set; } = 100;
}
