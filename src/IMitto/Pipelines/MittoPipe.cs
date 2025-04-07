using IMitto.Settings;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace IMitto.Pipelines;

public static class MittoPipe
{
	public static IDuplexPipe CreatePipe<T>(TcpClient client, MittoOptions options)
	{
		return CreatePipe<T>(client.GetStream(), options);
	}

	public static IDuplexPipe CreatePipe<T>(NetworkStream stream, MittoOptions options)
	{
		return new MittoDuplexPipe(stream, options.Pipeline);
	}

	public static SerializingPipeReader<T> CreateReader<T>(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var reader = PipeReader.Create(stream, pipeOptions.CreateReaderOptions());
		return new SerializingPipeReader<T>(reader, pipeOptions);
	}

	public static SerializingPipeReader<T> CreateReader<T>(Stream stream, StreamPipeReaderOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var reader = PipeReader.Create(stream, options);
		return new SerializingPipeReader<T>(reader, new MittoPipeOptions());
	}

	public static SerializingPipeWriter<T> CreateWriter<T>(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var writer = PipeWriter.Create(stream, pipeOptions.CreateWriterOptions());
		return new SerializingPipeWriter<T>(writer, pipeOptions);
	}

	public static SerializingPipeWriter<T> CreateWriter<T>(Stream stream, StreamPipeWriterOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var writer = PipeWriter.Create(stream, options);
		return new SerializingPipeWriter<T>(writer, new MittoPipeOptions());
	}
}
