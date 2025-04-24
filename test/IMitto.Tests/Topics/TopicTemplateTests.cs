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
	public void ParsedTemplateMatchesTopic()
	{
		const string topicString = "/topic/template";

		var topic = TopicParser.ParseTopic(topicString);
		var topicTemplate = TopicParser.ParseTemplate(topicString);

		Assert.NotNull(topicTemplate);
		Assert.False(topicTemplate.IsRoot);
		Assert.Equal(topicString, topicTemplate.Template);

		Assert.NotNull(topic);
		Assert.Equal(topicString, topic.Value);

		Assert.True(topicTemplate.IsMatch(topic));
	}

	[Fact]
	public void ParsedTemplateWithWildcardMatchesTopic()
	{
		const string topicString = "/topic/template";
		const string topicTemplateString = "/topic/*";

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
