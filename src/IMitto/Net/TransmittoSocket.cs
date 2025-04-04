using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using IMitto.Net.Settings;

namespace IMitto.Net;

public class TransmittoSocket : IDisposable
{
	private readonly StreamReader _reader;
	private readonly StreamWriter _writer;
	private readonly Encoding _encoding;
	private readonly IMittoConnection _parent;
	private readonly TcpClient _tcpClient;
	private readonly JsonSerializerOptions _options;

	public TransmittoSocket(IMittoConnection parent, TcpClient tcpClient, TransmittoBaseOptions options)
	{
		var stream = tcpClient.GetStream();

		_parent = parent;
		_tcpClient = tcpClient;

		_options = options.Json.Serializer;
		_encoding = options.Encoding;

		_reader = new StreamReader(stream);
		_writer = new StreamWriter(stream);
	}

	public bool IsConnected => _tcpClient.Connected;

	public bool DataAvailable => _tcpClient.Connected && _tcpClient.Available > 0;

	public Task<TMessage?> ReadRequestAsync<TMessage>(CancellationToken token = default) where TMessage : IMittoMessage
		=> ReadAsync<TMessage>(token);

	public Task<TMessage?> ReadResponseAsync<TMessage>(CancellationToken token = default) where TMessage : IMittoMessage
		=> ReadAsync<TMessage>(token);

	public Task SendResponseAsync<TMessage>(TMessage response, CancellationToken token = default) where TMessage : IMittoMessage
		=> SendAsync(response, token);

	public Task SendRequestAsync<TMessage>(TMessage request, CancellationToken token = default) where TMessage : IMittoMessage
		=> SendAsync(request, token);

	public async Task<TMessage?> ReadAsync<TMessage>(CancellationToken token = default) where TMessage : IMittoMessage
	{
		var requestRaw = await _reader.ReadLineAsync(token);

		if (requestRaw == null)
		{
			return default;
		}

		return JsonSerializer.Deserialize<TMessage>(
			requestRaw,
			options: _options
		);
	}

	public async Task SendAsync<TMessage>(TMessage message, CancellationToken token = default) where TMessage : IMittoMessage
	{
		ArgumentNullException.ThrowIfNull(message);

		var transmittoMessage = JsonSerializer.Serialize(message, options: _options);

		await _writer.WriteLineAsync(transmittoMessage);
		//_writer.Write(0x1A);
		await _writer.FlushAsync(token);
	}

	public void Dispose()
	{
		_tcpClient.Dispose();

		GC.SuppressFinalize(this);
	}
}
