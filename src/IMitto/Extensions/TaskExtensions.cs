#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TaskExtensions
{
	public static async Task Await(this Task task, bool configureAwait = false)
	{
		await task.ConfigureAwait(configureAwait);
	}

	public static async Task<T> Await<T>(this Task<T> task, bool configureAwait = false)
	{
		return await task.ConfigureAwait(configureAwait);
	}
}
