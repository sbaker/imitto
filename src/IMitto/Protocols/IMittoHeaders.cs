using System.Diagnostics.CodeAnalysis;

namespace IMitto.Protocols;

public interface IMittoHeaders : IReadOnlyCollection<IMittoHeader>
{
	static abstract byte HeaderCountLength { get; }

	IMittoHeader this[byte key] { get; set; }

	IMittoHeader this[MittoHeaderKey key] { get; set; }

	void Add(IMittoHeader header);

	bool TryGetValue(byte key, [NotNullWhen(true)] out IMittoHeader? value);

	bool TryGetValue(MittoHeaderKey key, [NotNullWhen(true)] out IMittoHeader? value);

	bool TryGetValue(string key, [NotNullWhen(true)] out IMittoHeader? value);
	
	bool Exists(string key);

	bool Exists(MittoHeaderKey key);
}
