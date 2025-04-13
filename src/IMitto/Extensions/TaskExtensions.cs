#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Runtime.CompilerServices;

namespace IMitto;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class TaskExtensions
{
	public static ConfiguredTaskAwaitable Await(this Task task, bool configureAwait = false)
	{
		return task.ConfigureAwait(configureAwait);
	}

	public static ConfiguredTaskAwaitable<T> Await<T>(this Task<T> task, bool configureAwait = false)
	{
		return task.ConfigureAwait(configureAwait);
	}

	public static ConfiguredValueTaskAwaitable Await(this ValueTask task, bool configureAwait = false)
	{
		return task.ConfigureAwait(configureAwait);
	}

	public static ConfiguredValueTaskAwaitable<T> Await<T>(this ValueTask<T> task, bool configureAwait = false)
	{
		return task.ConfigureAwait(configureAwait);
	}
}
