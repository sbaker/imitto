using IMitto.Storage.Internal;
using IMitto.Storage;

namespace IMitto;

public class SubscriberDefaults
{
	/// <summary>
	/// Returns the default <see cref="ConcurrentDictionarySubscriptionRepository"/> implementation.
	/// </summary>
	public static readonly ISubscriptionRepository InMemory = new ConcurrentDictionarySubscriptionRepository();
}