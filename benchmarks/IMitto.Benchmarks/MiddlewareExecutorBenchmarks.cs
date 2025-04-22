using BenchmarkDotNet.Attributes;
using IMitto.Middlware;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IMitto.Benchmarks;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class MiddlewareExecutorBenchmarks
{
#nullable disable
	private MiddlewareCollection _middlewareNonGeneric;
	private MiddlewareContext _contextNonGeneric;

	private MiddlewareCollection<string> _middlewareGeneric;
	private readonly string _contextGeneric = "TestState";
	
	private CancellationToken _cancellationToken;
#nullable restore

	[GlobalSetup]
	public void Setup()
	{
		_middlewareNonGeneric = [
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => Task.CompletedTask
		];

		_middlewareGeneric = [
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => next(context, token),
			(next, context, token) => Task.CompletedTask
		];

		_contextNonGeneric = new();

		_cancellationToken = CancellationToken.None;
	}

	[Params(1_000, 10_000)]
	public int IterationCount;

	[Benchmark]
	public async Task ExecuteAsync_Middleware()
	{
		for (int i = 0; i < IterationCount; i++)
		{
			await _middlewareGeneric.HandleAsync(_contextGeneric, _cancellationToken);
		}
	}

	[Benchmark]
	public async Task ExecuteAsync_NonGeneric()
	{
		for (int i = 0; i < IterationCount; i++)
		{
			await _middlewareNonGeneric.HandleAsync(_contextNonGeneric, _cancellationToken);
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
