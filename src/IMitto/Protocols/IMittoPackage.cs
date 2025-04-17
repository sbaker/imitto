namespace IMitto.Protocols;

public interface IMittoPackage
{
	IMittoCommand Command { get; }

	IMittoHeaders Header { get; }

	IMittoContent Content { get; }
}
