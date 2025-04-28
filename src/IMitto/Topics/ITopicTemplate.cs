using System.Diagnostics.CodeAnalysis;

namespace IMitto.Topics;

public interface ITopicTemplate
{
	bool IsRoot { get; }

	[StringSyntax(StringSyntaxAttribute.Uri)]
	string Template { get; }

	TopicSegment[] Segments { get; }

	bool IsMatch(ITopic topic);

	bool IsMatch(ITopicTemplate topic);
}
