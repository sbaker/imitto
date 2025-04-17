using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace IMitto.Protocols.V1;

public class MittoHeaders : Collection<IMittoHeader>, IMittoHeaders
{
	public MittoHeaders()
	{
	}

	public IMittoHeader this[byte key] => this.First(h => h.HasStringKey && h.KeyId == (MittoHeaderKey)key);

	public IMittoHeader this[string key] => this.First(h => h.HasStringKey && h.Key == key);

	public IMittoHeader this[MittoHeaderKey key] => this.First(h => h.KeyId == key);

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

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
