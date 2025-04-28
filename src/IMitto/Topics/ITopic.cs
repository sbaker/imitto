using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace IMitto.Topics;

public interface ITopic
{
	[StringSyntax(StringSyntaxAttribute.Uri)]
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

	[StringSyntax(StringSyntaxAttribute.Uri)]
	public string Value { get; }

	public StringSegment[] Segments { get; }
}
