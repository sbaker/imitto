using IMitto.Settings;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace IMitto.Pipelines;

public static class MittoPipe
{
	public static IMittoDuplexPipe CreatePipe(TcpClient client, MittoOptions options)
	{
		return CreatePipe(client.GetStream(), options);
	}

	public static IMittoDuplexPipe CreatePipe(NetworkStream stream, MittoOptions options)
	{
		return new MittoDuplexPipe(stream, options.Pipeline);
	}

	public static SerializingPipeReader CreateReader(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var reader = PipeReader.Create(stream, pipeOptions.CreateReaderOptions());
		return new SerializingPipeReader(reader, pipeOptions);
	}

	public static SerializingPipeReader CreateReader(Stream stream, StreamPipeReaderOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var reader = PipeReader.Create(stream, options);
		return new SerializingPipeReader(reader, new MittoPipeOptions());
	}

	public static SerializingPipeWriter CreateWriter(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var writer = PipeWriter.Create(stream, pipeOptions.CreateWriterOptions());
		return new SerializingPipeWriter(writer, pipeOptions);
	}

	public static SerializingPipeWriter CreateWriter(Stream stream, StreamPipeWriterOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var writer = PipeWriter.Create(stream, options);
		return new SerializingPipeWriter(writer, new MittoPipeOptions());
	}
}
