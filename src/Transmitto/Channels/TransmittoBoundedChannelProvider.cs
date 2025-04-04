using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Transmitto.Channels;

public sealed class TransmittoBoundedChannelProvider<T> : ITransmittoChannelProvider<T>
{
	private readonly Channel<T> _channel;

	public TransmittoBoundedChannelProvider(IOptions<TransmittoBoundedChannelOptions> options)
	{
		_channel = Channel.CreateBounded<T>(new BoundedChannelOptions(options.Value.Capacity)
		{
			FullMode = BoundedChannelFullMode.Wait
		});
	}

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
