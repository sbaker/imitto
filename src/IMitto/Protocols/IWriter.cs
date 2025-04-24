using System.Buffers;

namespace IMitto.Protocols;

public interface IWriter<T>
{
	void Write(T value);

	void Write(Span<T> span);

	void Write(ReadOnlySpan<T> span);

	void Write(ReadOnlySequence<T> sequence);

	bool HasCapacity(int expected);
}