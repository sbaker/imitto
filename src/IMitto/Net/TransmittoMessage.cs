using IMitto.Net.Models;

namespace IMitto.Net;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public abstract class TransmittoMessage(TransmittoHeader header) : IMittoMessage
{
	protected TransmittoMessage() : this(new TransmittoHeader())
	{
	}

	public TransmittoHeader Header { get; set; } = header;

	public abstract bool HasBody();

	public abstract TransmittoMessageBody GetBody();

	public abstract TBody GetBody<TBody>() where TBody : notnull, TransmittoMessageBody;
}

public class TransmittoMessage<TBody> : TransmittoMessage, IMittoMessage<TBody> where TBody : TransmittoMessageBody
{
	public TransmittoMessage() : base(new TransmittoHeader())
	{
	}

	public TransmittoMessage(TBody body) : this(body, new TransmittoHeader())
	{
		Body = body;
	}

	public TransmittoMessage(TBody body, TransmittoHeader header) : base(header)
	{
	}

	public TBody Body { get; set; }

	public override TransmittoMessageBody GetBody()
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
