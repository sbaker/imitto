using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using IMitto.Benchmarks;
using System.Runtime.CompilerServices;
using System.Text;


//BenchmarkRunner.Run(typeof(Program).Assembly);

BenchmarkRunner.Run<TenKbBodyProtocolTransportBenchmarks>();