using IMitto.Net.Settings;
using System.Buffers;
using System.IO.Pipelines;

namespace IMitto.Pipelines;

public class MittoDuplexPipe(Stream stream, MittoPipeOptions options) : IDuplexPipe
{
	private readonly char _terminator = (char)0x0A;
	private readonly PipeReader _reader = PipeReader.Create(stream, options.CreateReaderOptions());
	private readonly PipeWriter _writer = PipeWriter.Create(stream, options.CreateWriterOptions());

	public PipeReader Input => _reader;

	public PipeWriter Output => _writer;
}

public class MittoDuplexPipe<T>(Stream stream, MittoPipeOptions options) : IDuplexPipe, IMittoDuplexPipe<T>
{
	private readonly SerializingPipeReader<T> _reader = MittoPipe.CreateReader<T>(stream, options.CreateReaderOptions());
	private readonly SerializingPipeWriter<T> _writer = MittoPipe.CreateWriter<T>(stream, options.CreateWriterOptions());

	PipeReader IDuplexPipe.Input => _reader;
	public SerializingPipeReader<T> Reader => _reader;

	PipeWriter IDuplexPipe.Output => _writer;
	public SerializingPipeWriter<T> Writer => _writer;
}

public readonly struct ReadResult<T>(ReadResult result, T value)
{
	public ReadOnlySequence<byte> Buffer => result.Buffer;

	public bool IsCompleted => result.IsCompleted;
	
	public bool IsCanceled => result.IsCanceled;
	
	public bool IsEmpty => result.Buffer.IsEmpty;
	
	public ReadResult Result => result;

	public T Value => value;
}
