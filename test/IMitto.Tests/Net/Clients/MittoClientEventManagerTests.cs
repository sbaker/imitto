using IMitto.Channels;
using IMitto.Net.Clients;
using IMitto.Net.Models;
using IMitto.Producers;
using IMitto.Settings;
using Microsoft.Extensions.Options;
using Overmocked;
using System.Collections.Concurrent;

namespace IMitto.Tests.Net.Clients;

public class MittoClientEventManagerTests
{
	private readonly IServiceProvider _serviceProviderMock;
	private readonly IMittoChannelReaderProvider<PackagedGoods> _readerProviderMock;
	private readonly MittoClientOptions _options;
	private readonly MittoClientEventManager _eventManager;

	public MittoClientEventManagerTests()
	{
		_serviceProviderMock = Overmock.Interface<IServiceProvider>();
		_readerProviderMock = Overmock.Interface<IMittoChannelReaderProvider<PackagedGoods>>();
		_options = new MittoClientOptions
		{
			TypeMappings = new ConcurrentDictionary<string, TopicPackageTypeMapping>()
		};
		var optionsMock = Options.Create(_options);
		_eventManager = new MittoClientEventManager(optionsMock, _serviceProviderMock, _readerProviderMock);
	}

	[Fact]
	public void HandleClientEventReceived_ShouldCompleteTask()
	{
		// Arrange
		var cancellationToken = CancellationToken.None;

		// Act
		var task = _eventManager.HandleClientEventReceived(new EventNotificationsModel
		{
			Topic = "test-topic",
			Events =
			[
				new()
				{
					Id = "1",
					Type = "test-type",
					Topic = "test-topic",
					Event = new()
				}
			]
		}, cancellationToken);

		// Assert
		Assert.Equal(Task.CompletedTask, task);
	}
}