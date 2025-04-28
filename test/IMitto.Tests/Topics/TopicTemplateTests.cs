using IMitto.Topics;

namespace IMitto.Tests.Topics;

public class TopicTemplateTests
{
	[Fact]
	public void IsRootTest()
	{
		var topicTemplate = TopicParser.ParseTemplate("/");

		Assert.NotNull(topicTemplate);
		Assert.True(topicTemplate.IsRoot);
	}

	[Fact]
	public void ParsedTemplateHas2Segments()
	{
		const string topicString = "/topic/template";

		var topicTemplate = TopicParser.ParseTemplate(topicString);

		Assert.NotNull(topicTemplate);
		Assert.False(topicTemplate.IsRoot);
		Assert.Equal(topicString, topicTemplate.Template);

		Assert.Collection(topicTemplate.Segments,
			t => Assert.Equal("topic", t.Segment),
			t => Assert.Equal("template", t.Segment)
		);
	}

	[Fact]
	public void ParsedTemplateHas3Segments()
	{
		const string topicString = "/topic/template/text";

		var topicTemplate = TopicParser.ParseTemplate(topicString);

		Assert.NotNull(topicTemplate);
		Assert.False(topicTemplate.IsRoot);
		Assert.Equal(topicString, topicTemplate.Template);

		Assert.Collection(topicTemplate.Segments,
			t => Assert.Equal("topic", t.Segment),
			t => Assert.Equal("template", t.Segment),
			t => Assert.Equal("text", t.Segment)
		);
	}

	[Fact]
	public void ParsedTemplateHas3SegmentsWithOneWildcard()
	{
		const string topicString = "/topic/*/text";

		var topicTemplate = TopicParser.ParseTemplate(topicString);

		Assert.NotNull(topicTemplate);
		Assert.False(topicTemplate.IsRoot);
		Assert.Equal(topicString, topicTemplate.Template);

		Assert.Collection(topicTemplate.Segments,
			t => Assert.Equal("topic", t.Segment),
			t => { Assert.Equal("*", t.Segment); Assert.True(t.IsWildcard); },
			t => Assert.Equal("text", t.Segment)
		);
	}

	[Fact]
	public void ParsedTemplateMatchesTopic()
	{
		const string topicString = "/topic/template";

		ParseAndAssertTopics(topicString, topicString);
	}

	[Fact]
	public void ParsedTemplateWithWildcardMatchesTopicEndSegment()
	{
		const string topicString = "/topic/template";
		const string topicTemplateString = "/topic/*";

		ParseAndAssertTopics(topicString, topicTemplateString);
	}

	[Fact]
	public void ParsedTemplateWithWildcardMatchesTopicMiddleSegment()
	{
		const string topicString = "/topic/template/text";
		const string topicTemplateString = "/topic/*/text";

		ParseAndAssertTopics(topicString, topicTemplateString);
	}

	[Fact]
	public void ParsedTemplateWithWildcardMatchesTopicMiddleAndEndSegment()
	{
		const string topicString = "/topic/template/text";
		const string topicTemplateString = "/topic/*/*";

		ParseAndAssertTopics(topicString, topicTemplateString);
	}

	private static void ParseAndAssertTopics(string topicString, string topicTemplateString)
	{
		var topic = TopicParser.ParseTopic(topicString);
		var topicTemplate = TopicParser.ParseTemplate(topicTemplateString);

		Assert.NotNull(topicTemplate);
		Assert.False(topicTemplate.IsRoot);
		Assert.Equal(topicTemplateString, topicTemplate.Template);

		Assert.NotNull(topic);
		Assert.Equal(topicString, topic.Value);

		Assert.True(topicTemplate.IsMatch(topic));
	}
}
