﻿using IMitto.Settings;
using System.Buffers;
using System.IO.Pipelines;

namespace IMitto.Pipelines;

public class MittoDuplexPipe(Stream readerStream, Stream writerStream, MittoPipeOptions options) : IDuplexPipe, IMittoDuplexPipe
{
	private readonly SerializingPipeReader _reader = MittoPipe.CreateReader(readerStream, options.CreateReaderOptions());
	private readonly SerializingPipeWriter _writer = MittoPipe.CreateWriter(writerStream, options.CreateWriterOptions());

	public MittoDuplexPipe(Stream stream, MittoPipeOptions options) : this(stream, stream, options)
	{
	}

	PipeReader IDuplexPipe.Input => _reader;

	PipeWriter IDuplexPipe.Output => _writer;

	public SerializingPipeReader Reader => _reader;

	public SerializingPipeWriter Writer => _writer;
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
