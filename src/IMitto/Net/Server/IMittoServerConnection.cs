namespace IMitto.Net.Server;

public interface IMittoServerConnection : IMittoConnection
{
	Task<MittoPipelineSocket> AcceptAsync(CancellationToken token = default);
}
