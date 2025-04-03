using System.Collections.Concurrent;
using Transmitto.Net.Settings;

namespace Transmitto.Net.Clients;

public class TransmittoClientOptions : TransmittoBaseOptions
{
	public string? AuthenticationSecret { get; set; }
	
	public string? AuthenticationKey { get; set; }

	public ConcurrentDictionary<string, Type> TypeMappings { get; set; } = new ConcurrentDictionary<string, Type>();
}
