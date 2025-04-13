using BenchmarkDotNet.Attributes;
using IMitto.Middlware;

namespace IMitto.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
//[SimpleJob]
public class MiddlewareExecutorBenchmarks
{
#nullable disable
	private MiddlewareCollection<string> _middleware;
	private readonly string _context = "TestState";
	private MiddlewareCollection _middlewareCollection;
	private MiddlewareContext _middlewareContext;
	private CancellationToken _cancellationToken;
#nullable restore

	[GlobalSetup]
	public void Setup()
	{
		_middlewareCollection = [
			(next, context, token) => next(context, token),
			(next, context, token) => Task.CompletedTask
		];

		_middlewareContext = new();
		_cancellationToken = CancellationToken.None;
	}

	[Params(1_000, 10_000)]
	public int IterationCount;

	[Benchmark]
	public async Task ExecuteAsync_Middleware()
	{
		for (int i = 0; i < IterationCount; i++)
		{
			await _middleware.HandleAsync(_context, _cancellationToken);
		}
	}

	[Benchmark]
	public async Task ExecuteAsync_NonGeneric()
	{
		for (int i = 0; i < IterationCount; i++)
		{
			await _middlewareCollection.HandleAsync(_middlewareContext, _cancellationToken);
		}
	}

	//[Benchmark]
	//public async Task ExecuteAsync_NonGeneric_New()
	//{
	//	for (int i = 0; i < IterationCount; i++)
	//	{
	//		await MiddlewareExecutor.ExecuteAsyncOld(_middlewareCollection, _middlewareContext, _cancellationToken);
	//	}
	//}

	//[Benchmark]
	//public async Task ExecuteAsync_Generic_New()
	//{
	//	for (int i = 0; i < IterationCount; i++)
	//	{
	//		await MiddlewareExecutor.ExecuteAsyncOld(_genericMiddlewareCollection, _genericMiddlewareContext, _cancellationToken);
	//	}
	//}
}
