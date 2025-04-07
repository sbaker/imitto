using Microsoft.Extensions.Options;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;

namespace IMitto.Net.Settings;

public class MittoPipeOptions : PipeOptions
{
	private readonly MittoJsonOptions _jsonOptions;
	private readonly Encoding _encoding;

	public MittoPipeOptions()
		: this(new MittoJsonOptions(), Encoding.UTF8)
	{

	}

	public MittoPipeOptions(MittoBaseOptions parentOptions)
		: this(parentOptions.Json, parentOptions.Encoding)
	{
	}

	public MittoPipeOptions(MittoJsonOptions jsonOptions, Encoding encoding)
	{
		_jsonOptions = jsonOptions;
		_encoding = encoding;
	}

	public Encoding Encoding => _encoding;

	public int MinimumBufferSize { get; set; } = 4096;

	public int MinimumWriteSize { get; set; } = 4096;

	public byte CharTerminator { get; set; } = (byte)'\n';

	public Func<T, string> CreateDefaultWriterSerializer<T>()
	{
		return (T data) => {
			return JsonSerializer.Serialize(data, _jsonOptions.Serializer);
		};
	}

	public Func<string, T> CreateDefaultReaderSerializer<T>()
	{
		return (string data) => {
			return JsonSerializer.Deserialize<T>(data, _jsonOptions.Serializer)!;
		};
	}

	internal StreamPipeReaderOptions? CreateReaderOptions()
	{
		return new StreamPipeReaderOptions(bufferSize: MinimumBufferSize, leaveOpen: true);
	}

	internal StreamPipeWriterOptions? CreateWriterOptions()
	{
		return new StreamPipeWriterOptions(minimumBufferSize: MinimumBufferSize, leaveOpen: true);
	}
}

public interface IMittoPipeSerializer
{
	T? Deserialize<T>(string data);

	string Serialize<T>(T data);
}

public abstract class MittoPipeSerializer : IMittoPipeSerializer
{
	public abstract T Deserialize<T>(string data);

	public abstract string Serialize<T>(T data);
}

public sealed class SystemIOTextJsonPipeSerializer : MittoPipeSerializer
{
	private readonly MittoJsonOptions _options;
	private readonly Encoding _encoding;

	public SystemIOTextJsonPipeSerializer(MittoJsonOptions options, Encoding encoding)
	{
		_options = options;
		_encoding = encoding;
	}

	public override T Deserialize<T>(string data)
	{
		var bytes = _encoding.GetBytes(data).AsSpan();
		return JsonSerializer.Deserialize<T>(bytes, _options.Serializer)!;
	}

	public override string Serialize<T>(T data)
	{
		return JsonSerializer.Serialize(data, _options.Serializer);
	}
}