
namespace IMitto.Protocols;

public interface IProtocolMessage
{
	IMittoDescriptor Descriptor { get; }

	IMittoHeader Header { get; }

	IMittoPackage Package { get; }
}

public interface IMittoDescriptor
{
	MittoAction Action { get; }

	MittoActionModifier Modifier { get; }
}


public class MittoHeaders : System.Collections.Specialized.NameValueCollection
{
	public MittoHeaders() : base(StringComparer.OrdinalIgnoreCase)
	{
	}
	public MittoHeaders(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase)
	{
	}
}

public interface IMittoHeader
{
	int HeaderLength { get; }

	MittoHeaders Headers { get; set; }
}

public interface IMittoPackage
{
	int BodyLength { get; }

	ReadOnlySpan<byte> Content { get; }
}

public enum MittoAction : short
{
	None = 0,
	Auth = 1,
	Produce = 2,
	Connect = 4,
	Disconnect = 8,
	Consume = 16,
	Stream = 32,
	Session = 64,
}

public enum MittoActionModifier : byte
{
	None = 0,
	End = 1,
	Start = 2,
	Ack = 4,
	Nack = 8,
	Error = 16,
}


