using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace IMitto.Protocols.V1;

public sealed class MittoHeaders : Collection<IMittoHeader>, IMittoHeaders
{
	public MittoHeaders()
	{
	}

	public MittoHeaders(IList<IMittoHeader> headers) : base(headers)
	{
	}

	public MittoHeaders(IEnumerable<IMittoHeader> headers) : base(headers.ToList())
	{
	}

	public static byte HeaderCountLength => 1;

	public bool TryGetValue(byte key, [NotNullWhen(true)] out IMittoHeader? value)
	{
		return TryGetValue((MittoHeaderKey)key, out value);
	}

	public bool TryGetValue(string key, [NotNullWhen(true)] out IMittoHeader? value)
	{
		var header = this.FirstOrDefault(h => h.HasStringKey && h.Key == key);

		if (header != null)
		{
			value = header;
			return true;
		}

		value = null;
		return false;
	}

	public bool TryGetValue(MittoHeaderKey key, [NotNullWhen(true)] out IMittoHeader? value)
	{
		var header = this.FirstOrDefault(h => h.HasStringKey && h.KeyId == key);

		if (header != null)
		{
			value = header;
			return true;
		}

		value = null;
		return false;
	}
}
