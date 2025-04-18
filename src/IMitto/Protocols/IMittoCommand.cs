namespace IMitto.Protocols;

public interface IMittoCommand
{
	static abstract byte VersionLength { get; }

	static abstract byte ActionLength { get; }

	static abstract byte ModifierLength { get; }

	MittoProtocolVersion Version { get; }

	MittoAction Action { get; }

	MittoModifier Modifier { get; }
}
