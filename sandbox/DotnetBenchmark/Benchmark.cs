using BenchmarkDotNet.Attributes;

namespace DotnetBenchmark;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 100, invocationCount: 1000, iterationCount: 1000)]
public class Benchmark
{
    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public void First()
    {
    }

    [Benchmark]
    public void Second()
    {
    }
}
