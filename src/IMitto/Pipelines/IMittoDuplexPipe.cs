namespace IMitto.Pipelines
{
	public interface IMittoDuplexPipe<T>
	{
		SerializingPipeReader<T> Reader { get; }
		SerializingPipeWriter<T> Writer { get; }
	}
}