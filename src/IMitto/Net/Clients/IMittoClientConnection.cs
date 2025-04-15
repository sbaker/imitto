using IMitto.Protocols.Requests;
using IMitto.Protocols.Responses;

namespace IMitto.Net.Clients;

public interface IMittoClientConnection : IMittoConnection
{
	string? ConnectionId { get; set; }

	bool IsDataAvailable();

	Task SendRequestAsync<TRequest>(TRequest request, CancellationToken token) where TRequest : IMittoRequest;

	Task<TResponse?> ReadResponseAsync<TResponse>(CancellationToken token) where TResponse : IMittoResponse;
}
