using System.Buffers;

namespace IMitto.Protocols.V1;

public sealed class MittoContent : MittoByteContent<int>, IMittoContent
{
	private string? _package;

	public MittoContent(string? package = null) : base()
	{
		_package = package;
		IsSerialized = false;
	}

	public MittoContent(ReadOnlySequence<byte> content) : base(content)
	{
	}

	public static byte ContentLengthByteCount => 4;

	public string Package
	{
		get
		{
			if (_package == null)
			{
				return _package = GetContent();
			}

			return _package ?? string.Empty;
		}
		set
		{
			_package = value;
			IsSerialized = false;
		}
	}
}
