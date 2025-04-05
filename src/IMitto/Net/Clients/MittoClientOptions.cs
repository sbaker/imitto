using System.Collections.Concurrent;
using IMitto.Net.Settings;

namespace IMitto.Net.Clients;

public class MittoClientOptions : MittoBaseOptions
{
	public string? AuthenticationSecret { get; set; }
	
	public string? AuthenticationKey { get; set; }

	public ConcurrentDictionary<string, TopicPackageTypeMapping> TypeMappings { get; set; } = new();
}
