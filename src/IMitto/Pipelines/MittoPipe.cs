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

	public static MittoPipeReader CreateReader(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var reader = PipeReader.Create(stream, pipeOptions.CreateReaderOptions());
		return new MittoPipeReader(reader, pipeOptions);
	}

	public static MittoPipeReader CreateReader(Stream stream, StreamPipeReaderOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var reader = PipeReader.Create(stream, options);
		return new MittoPipeReader(reader, new MittoPipeOptions());
	}

	public static MittoPipeWriter CreateWriter(Stream stream, MittoPipeOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var pipeOptions = options ?? new MittoPipeOptions();
		var writer = PipeWriter.Create(stream, pipeOptions.CreateWriterOptions());
		return new MittoPipeWriter(writer, pipeOptions);
	}

	public static MittoPipeWriter CreateWriter(Stream stream, StreamPipeWriterOptions? options = null)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var writer = PipeWriter.Create(stream, options);
		return new MittoPipeWriter(writer, new MittoPipeOptions());
	}
}
