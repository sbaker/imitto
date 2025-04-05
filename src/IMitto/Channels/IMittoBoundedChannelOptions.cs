using System.Threading.Channels;

namespace IMitto.Channels
{
	public class MittoBoundedChannelOptions
	{
		public int Capacity { get; set; } = 8;

		public BoundedChannelFullMode ChannelFullMode { get; set; }
	}
}