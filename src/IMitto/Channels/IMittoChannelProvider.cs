using System.Threading.Channels;

namespace IMitto.Channels;

/// <summary>
/// Provides <see cref="ChannelReader{T}"/> and <see cref="ChannelWriter{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMittoChannelProvider<T> : IMittoChannelReaderProvider<T>, IMittoChannelWriterProvider<T>
{
	/// <summary>
	/// Tries to complete the <see cref="Channel{T}"/>.
	/// </summary>
	/// <returns>true if the channel was successfully completed; false otherwise.</returns>
	bool Complete();
}
