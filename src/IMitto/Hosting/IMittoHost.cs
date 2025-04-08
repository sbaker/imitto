namespace IMitto.Hosting;

public interface IMittoHost : IMittoRunnable
{
	bool IsRunning { get; }

	Task StartAsync(CancellationToken? token = null);

	Task StopAsync(CancellationToken? token = null);
}
