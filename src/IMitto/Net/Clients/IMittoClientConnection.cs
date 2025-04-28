using IMitto.Protocols;
using IMitto.Protocols.Requests;
using IMitto.Protocols.Responses;

namespace IMitto.Net.Clients;

public interface IMittoClientConnection : IMittoConnection
{
	string? ConnectionId { get; set; }

	bool IsDataAvailable();

	Task<IMittoPackage> ReadResponseAsync(CancellationToken token);

	Task SendPackageAsync(IMittoPackage package, CancellationToken token);

	Task SendRequestAsync<TRequest>(TRequest request, CancellationToken token) where TRequest : IMittoRequest;

	Task<TResponse?> ReadResponseAsync<TResponse>(CancellationToken token) where TResponse : IMittoResponse;
}
