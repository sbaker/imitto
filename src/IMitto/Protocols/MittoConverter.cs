namespace IMitto.Protocols;

public static class MittoConverter
{
	public static byte ToByte(ReadOnlySpan<byte> bytes)
	{
		return bytes[0];
	}

	public static MittoProtocolVersion ToProtocolVersion(ReadOnlySpan<byte> bytes)
	{
		return (MittoProtocolVersion)bytes[0];
	}

	public static MittoAction ToAction(ReadOnlySpan<byte> bytes)
	{
		return (MittoAction)BitConverter.ToInt16(bytes);
	}

	public static MittoModifier ToModifier(ReadOnlySpan<byte> bytes)
	{
		return (MittoModifier)ToByte(bytes);
	}

	public static int ToInt(ReadOnlySpan<byte> bytes)
	{
		return BitConverter.ToInt32(bytes);
	}
}
