using System.Threading.Channels;

namespace IMitto.Channels;

/// <summary>
/// When implemented in a derived class; provides the underlying writer to <see cref="IBackgroundWorkerQueueWriter"/>
/// </summary>
/// <typeparam name="TWriter"></typeparam>
public interface IMittoChannelWriterProvider<TWriter>
{
	/// <summary>
	/// Gets the <typeparamref name="TWriter"/>.
	/// </summary>
	/// <returns>The <typeparamref name="TWriter"/></returns>
	ChannelWriter<TWriter> GetWriter();
}
