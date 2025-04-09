namespace IMitto.Pipelines;

public interface IMittoDuplexPipe
{
	SerializingPipeReader Reader { get; }
	SerializingPipeWriter Writer { get; }
}