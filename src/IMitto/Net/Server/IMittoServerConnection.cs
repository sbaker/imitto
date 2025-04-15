namespace IMitto.Net.Server;

public interface IMittoServerConnection : IMittoConnection
{
	Task<MittoSocket> AcceptAsync(CancellationToken token = default);
}
