namespace IMitto.Topics;

public interface ITopicTemplate
{
	bool IsRoot { get; }

	string Template { get; }

	TopicSegment[] Segments { get; }

	bool IsMatch(ITopic topic);

	bool IsMatch(ITopicTemplate topic);
}
