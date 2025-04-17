namespace IMitto.Protocols.V1;

public sealed class MittoPackage : IMittoPackage
{
	public MittoPackage(IMittoCommand command, IMittoHeaders header, IMittoContent content)
	{
		Command = command;
		Headers = header;
		Content = content;
	}

	public IMittoHeaders Headers { get; }

	public IMittoCommand Command { get; }

	public IMittoContent Content { get; }
}
