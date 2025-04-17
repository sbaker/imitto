using BenchmarkDotNet.Attributes;
using IMitto.Middlware;
using IMitto.Pipelines;
using IMitto.Protocols;
using IMitto.Settings;
using System.Buffers;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

[MemoryDiagnoser]
[ThreadingDiagnoser]
//[SimpleJob]
public class ProtocolTransportBenchmarks
{
#nullable disable
	private MemoryStream _memoryStream;
	private MittoPipeReader _reader;
	private MittoPipeWriter _writer;
	private IMittoPackage _package;
	private IMittoTransport _transport;
#nullable restore

	[GlobalSetup]
	public void Setup()
	{
		_memoryStream = new MemoryStream(ArrayPool<byte>.Shared.Rent(4096));
		_reader = new MittoPipeReader(_memoryStream, new MittoPipeOptions());
		_writer = new MittoPipeWriter(_memoryStream, new MittoPipeOptions());

		var kvpHeaders = Data.GetHeaders(Data.Small);

		_package = MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Session)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.SmallBody)
			.Build();
		_transport = MittoProtocol.CreateTransport(MittoProtocolVersion.V1);
	}

	[Params(10, 100)]
	public int IterationCount;

	[Benchmark]
	public async Task WriteAndReadPackage()
	{
		for (int i = 0; i < IterationCount; i++)
		{
			await WriteAndReadPackageAsync();
		}
	}

	private async Task WriteAndReadPackageAsync()
	{
		_memoryStream.Position = 0;

		await _transport.WritePackageAsync(_writer, _package, CancellationToken.None);

		_memoryStream.Position = 0;

		var package = await _transport.ReadPackageAsync(_reader, CancellationToken.None);
	}

	private static class Data
	{
		public const string Small = nameof(Small);
		public const string Medium = nameof(Medium);
		public const string Large = nameof(Large);

		private static readonly Dictionary<string, MittoHeader> _headers = new()
		{
			[Small] = new MittoHeader(SmallHeaders),
			[Medium] = new MittoHeader(MediumHeaders),
			[Large] = new MittoHeader(LargeHeaders)
		};

		public const string SmallHeaders = "encoding:value1\nkey2:value2";
		public const string SmallBody = "Lorem ipsum dolor sit amet, consectetur.";

		public const string MediumHeaders = "key1:value1\ncontent-type:value2\nkey3:value3\ncorrelation-id:value4\nkey5:value5";
		public const string MediumBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore.";

		public const string LargeHeaders = "timestamp:value1\ncorrelation-id:value2\nkey3:value3\nkey4:value4\nkey5:value5\nkey6:value6\nkey7:value7\nkey8:value8";
		public const string LargeBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

		public static IDictionary<string, string> GetHeaders(string name) => _headers.ContainsKey(name) ? _headers[name].Headers : [];

		public sealed class MittoHeader(string headers)
		{
			public Dictionary<string, string> Headers { get; } = headers.Split('\n')
				.Select(x => x.Split(':'))
				.ToDictionary(x => x[0], x => x[1]);
		}
	}
}
