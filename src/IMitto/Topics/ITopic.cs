using Microsoft.Extensions.Primitives;

namespace IMitto.Topics;

public interface ITopic
{
	string Value { get; }

	StringSegment[] Segments { get; }
}

public class Topic : ITopic
{
	public Topic(string topic, StringSegment[] segments)
	{
		Value = topic;
		Segments = segments;
	}

	public string Value { get; }

	public StringSegment[] Segments { get; }
}
