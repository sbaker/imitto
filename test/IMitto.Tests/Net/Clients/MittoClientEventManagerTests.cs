//using IMitto.Channels;
//using IMitto.Net.Clients;
//using IMitto.Net.Models;
//using IMitto.Settings;
//using Microsoft.Extensions.Options;
//using Overmocked;
//using System.Collections.Concurrent;

//namespace IMitto.Tests.Net.Clients;

//public class MittoClientEventManagerTests
//{
//	private readonly IServiceProvider _serviceProviderMock;
//	private readonly IMittoChannelReaderProvider<EventNotificationsModel> _readerProviderMock;
//	private readonly MittoClientOptions _options;
//	private readonly MittoClientEventManager _eventManager;

//	public MittoClientEventManagerTests()
//	{
//		_serviceProviderMock = Overmock.Interface<IServiceProvider>();
//		_readerProviderMock = Overmock.Interface<IMittoChannelReaderProvider<EventNotificationsModel>>();
//		_options = new MittoClientOptions
//		{
//			TypeMappings = new ConcurrentDictionary<string, TopicPackageTypeMapping>()
//		};
//		var optionsMock = Options.Create(_options);
//		_eventManager = new MittoClientEventManager(optionsMock, _serviceProviderMock, _readerProviderMock);
//	}
//}