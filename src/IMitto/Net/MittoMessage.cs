using IMitto.Net.Models;

namespace IMitto.Net;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public abstract class MittoMessage(MittoHeader header) : IMittoMessage
{
	protected MittoMessage() : this(new MittoHeader())
	{
	}

	public MittoHeader Header { get; set; } = header;

	public abstract bool HasBody();

	public abstract MittoMessageBody GetBody();

	public abstract TBody GetBody<TBody>() where TBody : notnull, MittoMessageBody;
}

public class MittoMessage<TBody> : MittoMessage, IMittoMessage<TBody> where TBody : MittoMessageBody
{
	public MittoMessage() : base(new MittoHeader())
	{
	}

	public MittoMessage(TBody body) : this(body, new MittoHeader())
	{
		Body = body;
	}

	public MittoMessage(TBody body, MittoHeader header) : base(header)
	{
	}

	public TBody Body { get; set; }

	public override MittoMessageBody GetBody()
	{
		return Body;
	}

	public override TBody1 GetBody<TBody1>()
	{
		var obj = Body as TBody1;
		return obj;
	}

	public override bool HasBody()
	{
		return Body is not null;
	}
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
