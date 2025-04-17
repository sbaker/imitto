using IMitto.Protocols.V1;

namespace IMitto.Protocols;

public interface IMittoHeader : IMittoByteContent<byte>
{
	//byte KeyLength { get; }

	//byte ValueLength { get; }

	MittoHeaderKey KeyId { get; }

	string Key { get; }

	string Value { get; }

	bool HasStringKey { get; }
}
