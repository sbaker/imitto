namespace IMitto.Protocols;

public interface IMittoContent : IMittoByteContent<int>
{
	static abstract byte ContentLengthByteCount { get; }

	string Package { get; set; }
}
