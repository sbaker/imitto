namespace IMitto.Protocols.V1;

public sealed class MittoPackage : IMittoPackage
{
	public MittoPackage(IMittoCommand command, IMittoHeaders header, IMittoContent content)
	{
		Command = command;
		Header = header;
		Content = content;
	}

	public IMittoHeaders Header { get; }

	public IMittoCommand Command { get; }

	public IMittoContent Content { get; }
}
