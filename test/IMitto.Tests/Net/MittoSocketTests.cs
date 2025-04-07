using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using IMitto.Net;
using IMitto.Settings;
using Overmocked;

namespace IMitto.Tests.Net;

public class MittoSocketTests
{
	private readonly IOvermock<TcpClient> _tcpClientMock;
	private readonly IOvermock<NetworkStream> _networkStreamMock;
	private readonly IOvermock<StreamReader> _readerMock;
	private readonly IOvermock<StreamWriter> _writerMock;
	private readonly MittoSocket _mittoSocket;
	private readonly MittoOptions _options;

	public MittoSocketTests()
	{
		_tcpClientMock = Overmock.Mock<TcpClient>();
		_networkStreamMock = Overmock.Mock<NetworkStream>();
		_readerMock = Overmock.Mock<StreamReader>();
		_writerMock = Overmock.Mock<StreamWriter>();

		_tcpClientMock.Mock(c => c.GetStream()).ToReturn(_networkStreamMock.Target);

		_options = new MittoOptions
		{
			Encoding = Encoding.UTF8,
			Json = new MittoJsonOptions
			{
				Serializer = new JsonSerializerOptions()
			}
		};

		_mittoSocket = new MittoSocket(_tcpClientMock.Target, _options);
	}

	//[Fact]
	//public void IsConnected_ShouldReturnTrue_WhenClientIsConnected()
	//{
	//	_tcpClientMock.Mock(c => c.Client.Connected).ToReturn(true);

	//	var result = _mittoSocket.IsConnected;

	//	Assert.True(result);
	//}

	//[Fact]
	//public void IsConnected_ShouldReturnFalse_WhenClientIsNotConnected()
	//{
	//	_tcpClientMock.Mock(c => c.Client.Connected).ToReturn(false);

	//	var result = _mittoSocket.IsConnected;

	//	Assert.False(result);
	//}

	//[Fact]
	//public void DataAvailable_ShouldReturnTrue_WhenDataIsAvailable()
	//{
	//	_tcpClientMock.Mock(c => c.Client.Connected).ToReturn(true);
	//	_networkStreamMock.Mock(s => s.DataAvailable).ToReturn(true);

	//	var result = _mittoSocket.DataAvailable;

	//	Assert.True(result);
	//}

	//[Fact]
	//public void DataAvailable_ShouldReturnFalse_WhenDataIsNotAvailable()
	//{
	//	_tcpClientMock.Mock(c => c.Client.Connected).ToReturn(true);
	//	_networkStreamMock.Mock(s => s.DataAvailable).ToReturn(false);

	//	var result = _mittoSocket.DataAvailable;

	//	Assert.False(result);
	//}

	//[Fact]
	//public async Task ReadAsync_ShouldReturnDeserializedMessage_WhenDataIsAvailable()
	//{
	//	var message = Overmock.Mock<IMittoMessage>();
	//	var messageJson = JsonSerializer.Serialize(message.Target, _options.Json.Serializer);
	//	_readerMock.Mock(r => r.ReadLineAsync(Its.Any<CancellationToken>())).ToReturn(ValueTask.FromResult<string?>(messageJson));

	//	var result = await _mittoSocket.ReadAsync<IMittoMessage>();

	//	Assert.NotNull(result);
	//}

	//[Fact]
	//public async Task ReadAsync_ShouldReturnNull_WhenNoDataIsAvailable()
	//{
	//	_readerMock.Mock(r => r.ReadLineAsync(Its.Any<CancellationToken>())).ToReturn(ValueTask.FromResult((string?)null));

	//	var result = await _mittoSocket.ReadAsync<IMittoMessage>();

	//	Assert.Null(result);
	//}

	//[Fact]
	//public async Task SendAsync_ShouldWriteSerializedMessage()
	//{
	//	var message = Overmock.Mock<IMittoMessage>();
	//	var messageJson = JsonSerializer.Serialize(message.Target, _options.Json.Serializer);

	//	await _mittoSocket.SendAsync(message.Target);

	//	_writerMock.Verify();
	//	_writerMock.Verify();
	//}
}