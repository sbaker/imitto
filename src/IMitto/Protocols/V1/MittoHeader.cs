using System.Buffers;
using System.Text;

namespace IMitto.Protocols.V1;

public sealed class MittoHeader : MittoByteContent<byte>, IMittoHeader
{
	private string? _key = null;
	private string? _value = null;
	private MittoHeaderKey _keyId;
	private readonly ReadOnlySequence<byte> _keyContent;

	public MittoHeader() : base()
	{
	}

	public MittoHeader(string key, string value) : base()
	{
		Key = key;
		Value = value;
	}

	public MittoHeader(string key, ReadOnlySequence<byte> value) : base(value)
	{
		Key = key;
	}

	public MittoHeader(MittoHeaderKey keyId, ReadOnlySequence<byte> key, ReadOnlySequence<byte> value) : base(value)
	{
		_keyId = keyId;
		_keyContent = key;

		IsSerialized = true;
	}

	public string Value
	{
		get
		{
			if (_value == null && !Content.IsEmpty)
			{
				_value = GetContent();
			}

			return _value ?? string.Empty;
		}
		set
		{
			_value = value;
		}
	}

	public MittoHeaderKey KeyId
	{
		get => _keyId;
		set
		{
			if (_keyId != value)
			{
				_keyId = value;
				IsSerialized = false;
			}
		}
	}

	public string Key
	{
		get
		{
			if (KeyId == MittoHeaderKey.Custom)
			{
				return _key ??= GetContent(_keyContent) ?? string.Empty;
			}

			if (MittoHeaderKeys.HeaderIdToKey.TryGetValue(KeyId, out var key))
			{
				return key;
			}

			return _key ?? string.Empty;
		}
		set
		{
			if (MittoHeaderKeys.KeyToHeaderId.TryGetValue(value, out var keyId))
			{
				KeyId = keyId;
				return;
			}

			KeyId = MittoHeaderKey.Custom;

			_key = value;

			IsSerialized = false;
		}
	}

	public bool HasStringKey => !string.IsNullOrEmpty(Key);
}
