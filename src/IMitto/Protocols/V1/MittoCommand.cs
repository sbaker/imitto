namespace IMitto.Protocols.V1;

public sealed class MittoCommand(MittoAction action, MittoModifier modifier) : IMittoCommand
{
	public static byte VersionLength => 1;

	public static byte ActionLength => 2;

	public static byte ModifierLength => 1;

	public MittoProtocolVersion Version { get; } = MittoProtocolVersion.V1;

	public MittoAction Action { get; } = action;

	public MittoModifier Modifier { get; } = modifier;
}
