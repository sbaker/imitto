using Microsoft.Extensions.Primitives;

using System;
using System.Linq;

namespace IMitto.Topics;

public class TopicTemplate : ITopicTemplate
{
	private const string _rootValue = "/";

	private readonly bool _isRoot = false;

	public TopicTemplate()
	{
		Template = _rootValue;
		_isRoot = true;
		Segments = [];
	}

	public TopicTemplate(string template, TopicSegment[] segments)
	{
		Template = template;
		Segments = segments;
	}

	public string Template { get; }

	public TopicSegment[] Segments { get; }

	public bool IsRoot => _isRoot;

	public bool IsMatch(ITopic topic)
	{
		if (_isRoot)
		{
			return topic.Value == _rootValue;
		}

		var topicSegments = topic.Segments;

		// Simple compare: template: /* and topic /[any]
		// Simple compare: template: /*/value and topic /[any]/value
		// Simple compare: template: /*/value/* and topic /[any]/value/[any]
		if (Segments.Length == topicSegments.Length)
		{
			for (int i = 0; i < topicSegments.Length; i++)
			{
				if (!Segments[i].IsMatch(topicSegments[i]))
				{
					return false;
				}
			}
		}

		return true;
	}

	public bool IsMatch(ITopicTemplate topic)
	{
		return false;
	}
}
