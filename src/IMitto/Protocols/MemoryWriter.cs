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

	public readonly bool HasCapacity(int length)
		=> length + _length > _memory.Length;

	public readonly bool HasCapacity(long length)
		=> length + _length > _memory.Length;

	public void Write(T value)
	{
		_memory.Span[_length++] = value;
	}

	public void Write(T[] values)
	{
		values.CopyTo(SliceSpan());
		_length += values.Length;
	}

	public void Write(ReadOnlySequence<T> content)
	{
		if (content.IsEmpty)
		{
			return;
		}

		CheckValidCapacity(content.Length);

		if (content.IsSingleSegment)
		{
			content.FirstSpan.CopyTo(SliceSpan());
			_length += content.FirstSpan.Length;
			return;
		}

		foreach (var segment in content)
		{
			segment.Span.CopyTo(SliceSpan());
			_length += segment.Length;
		}
	}

	private readonly Span<T> SliceSpan(int? capacity = null)
	{
		if (capacity.HasValue)
		{
			CheckValidCapacity(capacity.Value);
		}

		return _memory.Span[_length..];
	}

	private readonly void CheckValidCapacity(int length)
	{
		if (length + _length > _memory.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(length), $"The length {length} exceeds the capacity of the memory.");
		}
	}

	private readonly void CheckValidCapacity(long length)
	{
		if (length + _length > _memory.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(length), $"The length {length} exceeds the capacity of the memory.");
		}
	}
}
