using IMitto.Settings;
using System.Buffers;
using System.IO.Pipelines;

namespace IMitto.Pipelines;

public class MittoDuplexPipe<T>(Stream readerStream, Stream writerStream, MittoPipeOptions options) : IDuplexPipe, IMittoDuplexPipe<T>
{
	private readonly SerializingPipeReader<T> _reader = MittoPipe.CreateReader<T>(readerStream, options.CreateReaderOptions());
	private readonly SerializingPipeWriter<T> _writer = MittoPipe.CreateWriter<T>(writerStream, options.CreateWriterOptions());

	public MittoDuplexPipe(Stream stream, MittoPipeOptions options) : this(stream, stream, options)
	{
	}

	public PipeReader Input => _reader;

	public PipeWriter Output => _writer;

	public SerializingPipeReader<T> Reader => _reader;

	public SerializingPipeWriter<T> Writer => _writer;
}

public class MittoDuplexPipe(Stream readerStream, Stream writerStream, MittoPipeOptions options) : IDuplexPipe
{
	private readonly PipeReader _reader = PipeReader.Create(readerStream, options.CreateReaderOptions());
	private readonly PipeWriter _writer = PipeWriter.Create(writerStream, options.CreateWriterOptions());

	public MittoDuplexPipe(Stream stream, MittoPipeOptions options) : this(stream, stream, options)
	{
	}

	public PipeReader Input => _reader;

	public PipeWriter Output => _writer;
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
