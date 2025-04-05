using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using IMitto.Net.Settings;

namespace IMitto.Net;

public class MittoSocket : Disposables
{
	private readonly Encoding _encoding;
	private readonly IMittoConnection _parent;
	private readonly JsonSerializerOptions _options;
	private readonly StreamReader _reader;
	private readonly TcpClient _tcpClient;
	private readonly NetworkStream _networkStream;
	private readonly StreamWriter _writer;

	public MittoSocket(IMittoConnection parent, TcpClient tcpClient, MittoBaseOptions options)
	{
		var stream = tcpClient.GetStream();

		_parent = parent;

		_tcpClient = Add(tcpClient, tcpClient.Close);
		_networkStream = Add(stream);

		_reader = Add(new StreamReader(stream));
		_writer = Add(new StreamWriter(stream));

		_encoding = options.Encoding;
		_options = options.Json.Serializer;
	}

	public bool IsConnected => _tcpClient.Client.Connected;

	public bool DataAvailable => IsConnected && _networkStream.DataAvailable;

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
}
