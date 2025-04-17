using IMitto.Net;

namespace IMitto.Protocols.Models;

public class MittoHeader : Dictionary<string, object?>
{
	public static readonly MittoHeader Error = CreateHeader(MittoEventType.Error);

	public MittoHeader()
	{
	}

	public MittoHeader(IDictionary<string, object?> headers)
	{
		if (headers.TryGetValue(nameof(ConnectionId), out var connectionId)) ConnectionId = connectionId?.ToString();
		if (headers.TryGetValue(nameof(Version), out var version)) Version = version?.ToString()!;
		if (headers.TryGetValue(nameof(Path), out object? value)) Path = value?.ToString();
		if (headers.ContainsKey(nameof(Action))) Action = ParseEventType(headers[nameof(Action)]);
		if (headers.ContainsKey(nameof(Result))) Result = ParseEventType(headers[nameof(Result)]);

		static MittoEventType? ParseEventType(object? value)
		{
			var stringValue = value?.ToString();

			if (string.IsNullOrEmpty(stringValue)) return null;

			return Enum.Parse<MittoEventType>(stringValue);
		}
	}

	public static MittoHeader Authorization(MittoEventType? result = null, string? connectionId = null) => CreateHeader(result, MittoEventType.Authentication, connectionId);

	public static MittoHeader CreateHeader(MittoEventType? result, MittoEventType? action = null, string? connectionId = null, string? path = null)
	{
		return new()
		{
			Path = path ?? MittoPaths.Auth,
			Action = MittoEventType.Authentication,
			Result = result,
			ConnectionId = connectionId,
			Version = MittoConstants.Version,
		};
	}

	public string? ConnectionId { get; set; }

	public string? Path { get; set; }

	public MittoEventType? Action { get; set; }
	
	public MittoEventType? Result { get; set; }

	public string? Version { get; internal set; }
}
