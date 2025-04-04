using Transmitto.Storage.Internal;
using Transmitto.Storage;

namespace Transmitto;

public class SubscriberDefaults
{
	/// <summary>
	/// Returns the default <see cref="ConcurrentDictionarySubscriptionRepository"/> implementation.
	/// </summary>
	public static readonly ISubscriptionRepository InMemory = new ConcurrentDictionarySubscriptionRepository();
}