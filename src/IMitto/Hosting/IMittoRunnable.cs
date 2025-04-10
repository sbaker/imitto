namespace IMitto.Hosting;

public interface IMittoRunnable : IDisposable
{
	Task RunAsync(CancellationToken? token = null);
}
