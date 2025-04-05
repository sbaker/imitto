using IMitto.Local;
using IMitto.Storage;
using IMitto.Local.Storage.Internal;
using Microsoft.Extensions.Options;

namespace IMitto;

public class SubscriberDefaults
{
	/// <summary>
	/// Returns the default <see cref="ConcurrentDictionarySubscriptionRepository"/> implementation.
	/// </summary>
	public static readonly ISubscriptionRepository InMemoryRepository = new ConcurrentDictionarySubscriptionRepository();

	/// <summary>
	/// Returns the default <see cref="ConcurrentDictionarySubscriptionRepository"/> implementation.
	/// </summary>
	public static readonly IMittoLocalEvents LocalEventAggregator = new MittoLocalEvents(Options.Create(new MittoEventsOptions()));
}