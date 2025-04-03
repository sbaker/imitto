using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Transmitto.Channels;

public sealed class TransmittoBoundedChannelProvider<T>(IOptions<TransmittoBoundedChannelOptions> options)
	: ITransmittoChannelProvider<T>
{
	private readonly Channel<T> _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(options.Value.Capacity));

	public bool Complete()
	{
		return _channel.Writer.TryComplete();
	}

	public ChannelReader<T> GetReader()
	{
		return _channel.Reader;
	}

	public ChannelWriter<T> GetWriter()
	{
		return _channel.Writer;
	}
}
