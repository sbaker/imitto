namespace Transmitto.Net.Settings;

public class TransmittoConnectionOptions
{
	public int ConnectionTimeout { get; set; } = Timeout.Infinite;

	public required TransmittoHost Host { get; set; } = new TransmittoHost();
}
