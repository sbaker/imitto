using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TaskExtensions
{
	public static ConfiguredTaskAwaitable Await(this Task task, bool await = false)
	{
		return task.ConfigureAwait(await);
	}

	public static ConfiguredTaskAwaitable<T> Await<T>(this Task<T> task, bool await = false)
	{
		return task.ConfigureAwait(await);
	}
}
