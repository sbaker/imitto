using IMitto.Protocols.V1;
using System.Diagnostics.CodeAnalysis;

namespace IMitto.Protocols;

public interface IMittoHeaders : IEnumerable<IMittoHeader>
{
	IMittoHeader this[MittoHeaderKey key] { get; }

	IMittoHeader this[byte key] { get; }

	IMittoHeader this[string key] { get; }

	int Count { get; }

	bool TryGetValue(byte key, [NotNullWhen(true)] out IMittoHeader? value);

	bool TryGetValue(MittoHeaderKey key, [NotNullWhen(true)] out IMittoHeader? value);

	bool TryGetValue(string key, [NotNullWhen(true)] out IMittoHeader? value);
}
