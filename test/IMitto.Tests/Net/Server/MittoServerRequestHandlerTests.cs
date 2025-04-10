using IMitto.Channels;
using IMitto.Net;
using IMitto.Net.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Overmocked;

namespace IMitto.Tests.Net.Server;

public class MittoServerRequestHandlerTests
{
	private readonly ILogger<MittoServerRequestHandler> _mockLogger;
	private readonly IOvermock<IOptions<MittoServerOptions>> _mockOptions;
	private readonly IOvermock<IMittoAuthenticationHandler> _mockAuthenticationHandler;
	private readonly IOvermock<IServerEventManager> _mockEventManager;
	private readonly IOvermock<IMittoChannelWriterProvider<ConnectionContext>> _mockChannelProvider;
	private readonly IOvermock<IMittoEventListener> _mockEventListener;
	private readonly MittoServerRequestHandler _handler;

	public MittoServerRequestHandlerTests()
	{
		_mockLogger = Overmock.AnyInvocation<ILogger<MittoServerRequestHandler>>();
		_mockOptions = Overmock.Mock<IOptions<MittoServerOptions>>();
		_mockAuthenticationHandler = Overmock.Mock<IMittoAuthenticationHandler>();
		_mockEventManager = Overmock.Mock<IServerEventManager>();
		_mockChannelProvider = Overmock.Mock<IMittoChannelWriterProvider<ConnectionContext>>();
		_mockEventListener = Overmock.Mock<IMittoEventListener>();

		_mockOptions.Mock(o => o.Value).ToReturn(new MittoServerOptions());
		_mockEventManager.Mock(o => o.Publish(Its.Any<EventId>(), Its.Any<ConnectionContext>())).ToBeCalled();
		_handler = new MittoServerRequestHandler(
			_mockOptions.Target,
			_mockLogger,
			_mockAuthenticationHandler.Target,
			_mockEventManager.Target,
			_mockChannelProvider.Target);
	}

	//[Fact]
	//public async Task HandleRequestAsync_ShouldCallWaitForAuthenticationAsync()
	//{
	//	// Arrange
	//	var context = new ConnectionContext(_mockEventManager.Target, null, CancellationToken.None);

	//	// Act
	//	await _handler.HandleRequestAsync(context);

	//	//// Assert
	//	//_mockLogger.Verify(l => l.Log(
	//	//	Its.Is<LogLevel>(logLevel => logLevel == LogLevel.Trace),
	//	//	Its.IsAny<EventId>(),
	//	//	Its.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Authentication Message received")),
	//	//	Its.Any<Exception>(),
	//	//	Its.This<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
	//}
}
