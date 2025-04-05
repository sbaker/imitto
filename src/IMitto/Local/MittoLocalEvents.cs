using IMitto.Storage;
using Microsoft.Extensions.Options;
using Opt = Microsoft.Extensions.Options.Options;

namespace IMitto.Local;

public class MittoLocalEvents(
	IOptions<MittoEventsOptions> options,
	ISubscriptionRepository? repository = null) : MittoEvents(options, repository), IMittoLocalEvents
{
	public MittoLocalEvents() : this(Opt.Create(new MittoEventsOptions()))
	{
	}
}