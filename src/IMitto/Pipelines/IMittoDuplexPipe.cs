namespace IMitto.Pipelines;

public interface IMittoDuplexPipe
{
	MittoPipeReader Reader { get; }
	MittoPipeWriter Writer { get; }
}