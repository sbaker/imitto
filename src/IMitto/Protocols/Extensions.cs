using System.Buffers;

public static class Extensions
{
	private static bool TryReadExact(this ref SequenceReader<byte> reader, int length, out ReadOnlySpan<byte> span)
	{
		if (reader.Remaining < length)
		{
			span = default;
			return false;
		}

		span = reader.Sequence.Slice(reader.Position, length).ToArray();
		reader.Advance(length);
		return true;
	}
}
