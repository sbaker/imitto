using BenchmarkDotNet.Attributes;
using IMitto.Protocols;
using Microsoft.VSDiagnostics;

namespace IMitto.Benchmarks;

[CPUUsageDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class SmallBodyProtocolTransportBenchmarks : TransportBenchmarkBase
{
	protected override IMittoPackage CreatePackageBuilder(IDictionary<string, string> kvpHeaders)
	{
		return MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Session)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.SmallBody)
			.Build();
	}
}


[CPUUsageDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class MediumBodyProtocolTransportBenchmarks : TransportBenchmarkBase
{
	protected override IMittoPackage CreatePackageBuilder(IDictionary<string, string> kvpHeaders)
	{
		return MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Produce)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.MediumBody)
			.Build();
	}
}


[CPUUsageDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class LargeBodyProtocolTransportBenchmarks : TransportBenchmarkBase
{
	protected override IMittoPackage CreatePackageBuilder(IDictionary<string, string> kvpHeaders)
	{
		return MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Consume)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.LargeBody)
			.Build();
	}
}


[CPUUsageDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class TenKbBodyProtocolTransportBenchmarks : TransportBenchmarkBase
{
	protected override IMittoPackage CreatePackageBuilder(IDictionary<string, string> kvpHeaders)
	{
		return MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Consume)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.TenKb)
			.Build();
	}
}


[CPUUsageDiagnoser]
[MemoryDiagnoser]
[ThreadingDiagnoser]
[MarkdownExporterAttribute.Default]
//[SimpleJob]
public class TenMbBodyProtocolTransportBenchmarks : TransportBenchmarkBase
{
	protected override IMittoPackage CreatePackageBuilder(IDictionary<string, string> kvpHeaders)
	{
		return MittoProtocol.CreatePackageBuilder()
			.WithAction(MittoAction.Consume)
			.AddHeaders(kvpHeaders)
			.WithPackage(Data.TenMb)
			.Build();
	}
}
