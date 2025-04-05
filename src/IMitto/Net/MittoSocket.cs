using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using IMitto.Net.Settings;

namespace IMitto.Net;

public class MittoSocket : IDisposable
{
	private readonly Encoding _encoding;
	private readonly IMittoConnection _parent;
	private readonly JsonSerializerOptions _options;
	private readonly StreamReader _reader;
	private readonly TcpClient _tcpClient;
	private readonly StreamWriter _writer;

	public MittoSocket(IMittoConnection parent, TcpClient tcpClient, MittoBaseOptions options)
	{
		var stream = tcpClient.GetStream();

		_parent = parent;
		_tcpClient = tcpClient;

		_encoding = options.Encoding;
		_options = options.Json.Serializer;

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

		var mittoMessage = JsonSerializer.Serialize(message, options: _options);

		await _writer.WriteLineAsync(mittoMessage);
		//_writer.Write(0x1A);
		await _writer.FlushAsync(token);
	}

	public void Dispose()
	{
		_writer.Dispose();
		_reader.Dispose();

		_tcpClient.Close();
		_tcpClient.Dispose();

		GC.SuppressFinalize(this);
	}
}
