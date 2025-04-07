using System.Collections.Concurrent;
using IMitto.Settings;

namespace IMitto.Net.Clients;

public class MittoClientOptions : MittoOptions
{
	public string? AuthenticationSecret { get; set; }
	
	public string? AuthenticationKey { get; set; }

	public ConcurrentDictionary<string, TopicPackageTypeMapping> TypeMappings { get; set; } = new();
}
