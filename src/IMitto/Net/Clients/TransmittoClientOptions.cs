using System.Collections.Concurrent;
using IMitto.Net.Settings;

namespace IMitto.Net.Clients;

public class TransmittoClientOptions : TransmittoBaseOptions
{
	public string? AuthenticationSecret { get; set; }
	
	public string? AuthenticationKey { get; set; }

	public ConcurrentDictionary<string, Type> TypeMappings { get; set; } = new ConcurrentDictionary<string, Type>();
}
