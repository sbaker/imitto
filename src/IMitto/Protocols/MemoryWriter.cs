using System.Buffers;

namespace IMitto.Protocols;

public struct MemoryWriter<T>
{
	private readonly Memory<T> _memory;
	private int _length;

	public MemoryWriter(Memory<T> memory)
	{
		_memory = memory;
	}

	public int WrittenLength => _length;

	public void Write(T value)
	{
		_memory.Span[_length++] = value;
	}

	public void WriteBytes(T[] values)
	{
		values.CopyTo(_memory.Span);
		_length += values.Length;
	}
}
