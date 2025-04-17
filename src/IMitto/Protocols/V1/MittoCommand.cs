namespace IMitto.Protocols.V1;

public sealed class MittoCommand : IMittoCommand
{
	public static byte VersionLength => 1;

	public static byte ActionLength => 2;

	public static byte ModifierLength => 1;

	public static byte HeaderCountLength => 2;

	public MittoCommand(MittoProtocolVersion version, MittoAction action, MittoModifier modifier)
	{
		Version = version;
		Action = action;
		Modifier = modifier;
	}

	public MittoProtocolVersion Version { get; }

	public MittoAction Action { get; }

	public MittoModifier Modifier { get; }
}
