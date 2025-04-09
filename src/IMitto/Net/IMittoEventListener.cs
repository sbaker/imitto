namespace IMitto.Net;

public interface IMittoEventListener
{
	Task PollForEventsAsync(ConnectionContext context, CancellationToken token);
}
