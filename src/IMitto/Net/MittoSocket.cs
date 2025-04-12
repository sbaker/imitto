using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using IMitto.Settings;

namespace IMitto.Net;

public class MittoSocket : Disposables
{
	private readonly Encoding _encoding;
	private readonly MittoOptions _options;
	private readonly JsonSerializerOptions _serializerOptions;
	private readonly StreamReader _reader;
	private readonly TcpClient _tcpClient;
	private readonly NetworkStream _networkStream;
	private readonly StreamWriter _writer;

	public MittoSocket(TcpClient tcpClient, MittoOptions options)
	{
		var stream = tcpClient.GetStream();

		_tcpClient = Add(tcpClient, tcpClient.Close);
		_networkStream = Add(stream);

		_reader = Add(new StreamReader(stream));
		_writer = Add(new StreamWriter(stream));

		_encoding = options.Encoding;
		_options = options;
		_serializerOptions = options.Json.Serializer;
	}

	public bool IsConnected => _tcpClient.Client.Connected;

	public bool DataAvailable => IsConnected && _networkStream.DataAvailable;

	protected MittoOptions Options => _options;

	public Task<TMessage?> ReadRequestAsync<TMessage>(CancellationToken token = default)
		=> ReadAsync<TMessage>(token);

	public Task<TMessage?> ReadResponseAsync<TMessage>(CancellationToken token = default)
		=> ReadAsync<TMessage>(token);

	public Task SendResponseAsync<TMessage>(TMessage response, CancellationToken token = default)
		=> SendAsync(response, token);

	public Task SendRequestAsync<TMessage>(TMessage request, CancellationToken token = default)
		=> SendAsync(request, token);

	public virtual async Task<TMessage?> ReadAsync<TMessage>(CancellationToken token = default)
	{
		var requestRaw = await _reader.ReadLineAsync(token);

		if (requestRaw == null)
		{
			return default;
		}

		return JsonSerializer.Deserialize<TMessage>(
			requestRaw,
			options: _serializerOptions
		);
	}

	public virtual async Task SendAsync<TMessage>(TMessage message, CancellationToken token = default)
	{
		ArgumentNullException.ThrowIfNull(message);

		var mittoMessage = JsonSerializer.Serialize(message, options: _serializerOptions);

		await _writer.WriteAsync(mittoMessage);
		await _writer.WriteAsync((char)_options.Pipeline.CharTerminator);
		await _writer.FlushAsync(token);
	}
}
