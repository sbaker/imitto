using Microsoft.Extensions.Primitives;

namespace IMitto.Topics;

public class TopicParser
{
	public static readonly char Wildcard = '*';

	public static readonly char[] SegmentDelimiter = ['/'];

	public static ITopic ParseTopic(string topic)
	{
		// Should topics send from a producer be able to be "/"?
		// If so, then topics are required to have a length 1, and
		// Not be /* or **  or */
		if (topic == null || topic.Length < 2 && topic.Any(c => c == Wildcard && c == SegmentDelimiter[0]))
		{
			throw new ArgumentException("Topic must be a valid non-root or wildcard path and must beginning with '/'.");
		}

		var tokens = new StringTokenizer(topic, SegmentDelimiter)
			.Where(s => s.HasValue && !string.IsNullOrWhiteSpace(s.Value))
			.ToArray();

		return new Topic(topic, tokens);
	}

	public static ITopicTemplate ParseTemplate(string template)
	{
		var tokens = new StringTokenizer(template, SegmentDelimiter)
			.Where(s => s.HasValue && !string.IsNullOrWhiteSpace(s.Value))
			.Select(s => new TopicSegment(s, Wildcard))
			.ToArray();

		if (tokens.Length > 0)
		{
			return new TopicTemplate(template, tokens);
		}

		return new TopicTemplate();
	}
}
