using System.Threading.Channels;

namespace Transmitto.Channels
{
	public class TransmittoBoundedChannelOptions
	{
		public int Capacity { get; set; } = 8;

		public BoundedChannelFullMode ChannelFullMode { get; set; }
	}
}