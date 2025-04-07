using IMitto.Net.Settings;
using System.IO.Pipelines;

namespace IMitto.Pipelines;

public class SerializingPipeWriter<T> : PipeWriter
{
	private readonly PipeWriter _innerWriter;
	private readonly MittoPipeOptions _options;
	private readonly Func<T, string> _serializer;

	public SerializingPipeWriter(Stream stream, MittoPipeOptions options)
		: this(Create(stream, options.CreateWriterOptions()), options)
	{
	}

	public SerializingPipeWriter(PipeWriter innerWriter, MittoPipeOptions options)
	{
		_innerWriter = innerWriter;
		_options = options;
		_serializer = options.CreateDefaultWriterSerializer<T>();
	}

	public override void Advance(int bytes)
	{
		_innerWriter.Advance(bytes);
	}

	public override void CancelPendingFlush()
	{
		_innerWriter.CancelPendingFlush();
	}

	public override void Complete(Exception? exception = null)
	{
		_innerWriter.Complete(exception);
	}

	public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
	{
		return _innerWriter.FlushAsync(cancellationToken);
	}

	public override Memory<byte> GetMemory(int sizeHint = 0)
	{
		return _innerWriter.GetMemory(sizeHint);
	}

	public override Span<byte> GetSpan(int sizeHint = 0)
	{
		return _innerWriter.GetSpan(sizeHint);
	}

	public async Task WriteAsync(T data, CancellationToken token = default)
	{
		var serializedData = _serializer(data);
		var bytes = _options.Encoding.GetBytes(serializedData + (char)_options.CharTerminator);
		var memory = GetMemory(bytes.Length);

		bytes.CopyTo(memory.Span);
		
		Advance(bytes.Length);

		var result = await FlushAsync(token);
		
		if (result.IsCanceled)
		{
			throw new OperationCanceledException("Flush was canceled");
		}
	}
}
