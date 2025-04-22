namespace IMitto.Topics;

public interface ITopic
{
	string Template { get; }

	Microsoft.Extensions.Primitives.StringSegment[] Segments { get; }
}
