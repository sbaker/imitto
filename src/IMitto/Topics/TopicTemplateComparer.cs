namespace IMitto.Topics;

public class TopicTemplateComparer : IEqualityComparer<ITopicTemplate>, IEqualityComparer<ITopic>
{
	public static readonly TopicTemplateComparer Instance = new TopicTemplateComparer();

	private TopicTemplateComparer()
	{
	}

	public bool Equals(ITopicTemplate template, ITopic topic)
	{
		if (template == null || topic == null)
		{
			return false;
		}

		return template.IsMatch(topic);
	}

	public bool Equals(ITopic? x, ITopic? y)
	{
		if (x == null || y == null)
		{
			return false;
		}

		return x.Value == y.Value;
	}

	public int GetHashCode(ITopic obj)
	{
		return obj.GetHashCode();
	}

	public bool Equals(ITopicTemplate? x, ITopicTemplate? y)
	{
		if (x == null || y == null)
		{
			return false;
		}

		return x.Template == y.Template;
	}

	public int GetHashCode(ITopicTemplate obj)
	{
		return obj.GetHashCode();
	}
}