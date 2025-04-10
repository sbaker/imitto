using IMitto.Net.Requests;
using IMitto.Net.Responses;

namespace IMitto.Net.Clients;

public interface IMittoClientConnection : IMittoConnection
{
	bool IsDataAvailable();

	Task ConnectAsync(CancellationToken token = default);

	Task SendRequestAsync<TRequest>(TRequest request, CancellationToken token) where TRequest : IMittoRequest;

	Task<TResponse?> ReadResponseAsync<TResponse>(CancellationToken token) where TResponse : IMittoResponse;
}
