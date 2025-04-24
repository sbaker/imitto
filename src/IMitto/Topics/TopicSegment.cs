using Microsoft.Extensions.Primitives;

namespace IMitto.Topics;

public sealed class TopicSegment
{
	private char _wildcard;

	public TopicSegment(StringSegment segment, char wildcard)
	{
		_wildcard = wildcard;

		Segment = segment;

		IsWildcard = Segment.Length == 1 && Segment[0].Equals(wildcard);
	}

	public StringSegment Segment { get; }

	public bool IsWildcard { get; }

	public bool IsMatch(StringSegment stringSegment)
	{
		if (IsWildcard)
		{
			return true;
		}

		return stringSegment.Equals(Segment);
	}
}
