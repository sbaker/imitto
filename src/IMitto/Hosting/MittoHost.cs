using IMitto.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IMitto.Hosting;

public abstract class MittoHost : Disposables, IMittoHost
{
	private readonly ILogger _logger;
	private readonly MittoOptions _options;

	private CancellationTokenSource? _tokenSource;
	private Task? _startedTask = null;

	protected MittoHost(ILogger logger, IOptions<MittoOptions> options)
	{
		_logger = logger;
		_options = options.Value;
	}

	protected Task StartedTask => _startedTask ?? Task.CompletedTask;

	protected Queue<Task> BackgroundTasks { get; } = [];

	protected CancellationTokenSource TokenSource => GetOrCreateTokenSource();

	public bool IsRunning => !StartedTask.IsCompleted;

	public Task StartAsync(CancellationToken? token = null)
	{
		_tokenSource ??= GetOrCreateTokenSource(token);

		_startedTask = RunAsync(_tokenSource.Token);

		return Task.CompletedTask;
	}

	public virtual async Task RunAsync(CancellationToken? token = null)
	{
		_tokenSource ??= CreateLinkedSource(token);

		await RunInternalAsync(_tokenSource.Token).Await();
	}

	public virtual Task StopAsync(CancellationToken? token = null)
	{
		_tokenSource!.CancelAfter(TimeSpan.FromSeconds(_options.StoppingTimeoutInSeconds));

		return Task.WhenAll(
			BackgroundTasks.ToArray().Select(t => StopBackgroundTask(t, _tokenSource.Token))
		);
	}

	protected abstract Task RunInternalAsync(CancellationToken token = default);

	protected async Task StopBackgroundTask(Task task, CancellationToken token)
	{
		if (!task.IsCompleted)
		{
			try
			{
				await task.WaitAsync(token).Await();
			}
			catch (TaskCanceledException tce)
			{
				_logger.LogWarning(tce, "Timeout waiting for task. Stop background tasks");
			}
			catch (Exception e)
			{
				_logger.LogWarning(e, "Unknown error waiting for tasks shutdown. Stop background tasks");
			}
		}
	}

	protected void StartBackgroundTask(Func<CancellationToken, Task> factory, CancellationToken token)
		=> BackgroundTasks.Enqueue(factory(token));

	protected void RegisterBackgroundTask(Func<Task> factory)
		=> BackgroundTasks.Enqueue(factory());

	private CancellationTokenSource GetOrCreateTokenSource(CancellationToken? token = null)
	{
		if (_tokenSource != null)
		{
			return _tokenSource;
		}

		return Add(_tokenSource = !token.HasValue
			? new CancellationTokenSource()
			: CreateLinkedSource(token.Value));
	}

	private CancellationTokenSource CreateLinkedSource(CancellationToken? token = null)
	{	
		return CancellationTokenSource.CreateLinkedTokenSource(token ?? default);
	}
}

public abstract class MittoHost<TOptions> : MittoHost where TOptions : MittoOptions
{
	protected MittoHost(ILogger logger, IOptions<TOptions> options) : base(logger, options)
		=> Options = options.Value;

	protected TOptions Options { get; }
}

public class DelegatingMittoHost<T> : MittoHost
{
	public DelegatingMittoHost(ILogger<DelegatingMittoHost<T>> logger, Func<T, CancellationToken, Task> action) : base(logger, MittoOptions.Default.AsOptions())
	{

	}

	protected override Task RunInternalAsync(CancellationToken token = default)
	{
		throw new NotImplementedException();
	}
}
