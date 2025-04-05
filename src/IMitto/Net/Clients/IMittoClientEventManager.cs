using IMitto.Consumers;
using IMitto.Local;
using IMitto.Net.Models;
using IMitto.Net.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Opt = Microsoft.Extensions.Options.Options;

namespace IMitto.Net.Clients;

public interface IMittoClientEventManager : IMittoLocalEvents
{
	Task HandleClientEventReceived(EventNotificationsModel clientEvent, CancellationToken token);
}

public class MittoClientEventManager : MittoLocalEvents, IMittoClientEventManager
{
	private readonly MittoClientOptions _options;
	private readonly IServiceProvider _serviceProvider;
	private readonly ConcurrentDictionary<string, TopicPackageTypeMapping> _topicTypeMappings;

	public MittoClientEventManager(IOptions<MittoClientOptions> options, IServiceProvider serviceProvider)
		: base(Opt.Create(options.Value.Events))
	{
		_options = options.Value;
		_serviceProvider = serviceProvider;

		_topicTypeMappings = _options.TypeMappings;

		foreach (var mapping in _topicTypeMappings)
		{
			ConfigureTopicMapping(mapping.Key, mapping.Value);
		}
	}

	private void ConfigureTopicMapping(string key, TopicPackageTypeMapping mapping)
	{
		throw new NotImplementedException();
	}

	public Task HandleClientEventReceived(EventNotificationsModel clientEvent, CancellationToken token)
	{
		throw new NotImplementedException();
	}
}
