namespace IMitto.Protocols;

public interface IMittoPackage
{
	IMittoCommand Command { get; }

	IMittoHeaders Headers { get; }

	IMittoContent Content { get; }
}
