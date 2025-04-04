using System.Threading.Channels;

namespace Transmitto.Channels;

/// <summary>
/// When implemented in a derived class; provides the underlying writer to <see cref="IBackgroundWorkerQueueWriter"/>
/// </summary>
/// <typeparam name="TWriter"></typeparam>
public interface ITransmittoChannelWriterProvider<TWriter>
{
	/// <summary>
	/// Gets the <typeparamref name="TWriter"/>.
	/// </summary>
	/// <returns>The <typeparamref name="TWriter"/></returns>
	ChannelWriter<TWriter> GetWriter();
}
