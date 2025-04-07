namespace IMitto.Hosting;

public interface IMittoHost : IMittoRunnable
{
	Task StartAsync(CancellationToken? token = null);

	Task StopAsync(CancellationToken? token = null);
}
