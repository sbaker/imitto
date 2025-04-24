using System.Buffers;

namespace IMitto.Protocols;

public class AutoAdvancingBufferWriter<T> : IWriter<T>
{
	private readonly IBufferWriter<T> _writer;
	private readonly int _defaultCapacity;
	private MemoryWriter<T> _memoryWriter;

	public int WrittenLength => _memoryWriter.WrittenLength;

	public AutoAdvancingBufferWriter(IBufferWriter<T> writer, int defaultMemoryCapacity = 4096)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(defaultMemoryCapacity, 0, nameof(defaultMemoryCapacity));

		_writer = writer;
		_defaultCapacity = defaultMemoryCapacity;
		_memoryWriter = new MemoryWriter<T>(writer.GetMemory(_defaultCapacity));
	}

	public void EnsureCapacity(int expected)
	{
		if (!_memoryWriter.HasCapacity(expected))
		{
			Advance(expected);
		}
	}

	public void EnsureCapacity(long expected)
	{
		if (!_memoryWriter.HasCapacity(expected))
		{
			Advance((int)expected);
		}
	}

	private void Advance(int? excepctedCapacity = null)
	{
		var neededCapacity = excepctedCapacity ?? _defaultCapacity;

		ArgumentOutOfRangeException.ThrowIfLessThan(neededCapacity, 0, nameof(excepctedCapacity));

		if (neededCapacity > _defaultCapacity)
		{
			// If the needed capacity is greater than the default
			// capacity, add a little to the end of the needed buffer.
			neededCapacity += _defaultCapacity;
		}

		_writer.Advance(_memoryWriter.WrittenLength);

		var memory = _writer.GetMemory(neededCapacity);

		_memoryWriter = new MemoryWriter<T>(memory);
	}

	public bool HasCapacity(int expected)
	{
		return _memoryWriter.HasCapacity(expected);
	}

	public void Write(T value)
	{
		EnsureCapacity(1);
		_memoryWriter.Write(value);
	}

	public void Write(Span<T> span)
	{
		EnsureCapacity(span.Length);
		_memoryWriter.Write(span);
	}

	public void Write(ReadOnlySpan<T> span)
	{
		EnsureCapacity(span.Length);
		_memoryWriter.Write(span);
	}

	public void Write(ReadOnlySequence<T> sequence)
	{
		EnsureCapacity(sequence.Length);
		_memoryWriter.Write(sequence);
	}
}

public class MemoryWriter<T> : IWriter<T>
{
	private readonly Memory<T> _memory;
	private int _length;

	public MemoryWriter(Memory<T> memory)
	{
		_memory = memory;
	}

	public  int WrittenLength => _length;

	public  bool HasCapacity(int length)
		=> length + _length < _memory.Length;

	public  bool HasCapacity(long length)
		=> HasCapacity((int)length);

	public void Write(T value)
	{
		_memory.Span[_length++] = value;
	}

	public void Write(Span<T> span)
	{
		span.CopyTo(SliceSpan());
		_length += span.Length;
	}

	public void Write(ReadOnlySpan<T> span)
	{
		span.CopyTo(SliceSpan());
		_length += span.Length;
	}

	public void Write(ReadOnlySequence<T> sequence)
	{
		if (sequence.IsEmpty)
		{
			return;
		}

		CheckValidCapacity(sequence.Length);

		if (sequence.IsSingleSegment)
		{
			sequence.FirstSpan.CopyTo(SliceSpan());
			_length += sequence.FirstSpan.Length;
			return;
		}

		foreach (var segment in sequence)
		{
			segment.Span.CopyTo(SliceSpan());
			_length += segment.Length;
		}
	}

	private Span<T> SliceSpan(int? capacity = null)
	{
		if (capacity.HasValue)
		{
			CheckValidCapacity(capacity.Value);
		}

		return _memory.Span[_length..];
	}

	private void CheckValidCapacity(int length)
	{
		if (length + _length > _memory.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(length), $"The length {length} exceeds the capacity of the memory.");
		}
	}

	private void CheckValidCapacity(long length)
	{
		if (length + _length > _memory.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(length), $"The length {length} exceeds the capacity of the memory.");
		}
	}
}
