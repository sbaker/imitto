using BenchmarkDotNet.Running;
using IMitto.Benchmarks;


//BenchmarkRunner.Run(typeof(Program).Assembly);


BenchmarkRunner.Run<TenMbBodyProtocolTransportBenchmarks>();