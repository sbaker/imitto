namespace IMitto.Protocols.V1;

public sealed class MittoCommand : IMittoCommand
{
	public static byte VersionLength => 1;

	public static byte ActionLength => 2;

	public static byte ModifierLength => 1;

	public MittoCommand(MittoAction action, MittoModifier modifier)
	{
		Action = action;
		Modifier = modifier;
	}

	public MittoProtocolVersion Version { get; } = MittoProtocolVersion.V1;

	public MittoAction Action { get; }

	public MittoModifier Modifier { get; }
}
