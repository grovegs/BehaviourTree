using BenchmarkDotNet.Running;

namespace DotnetBenchmark;

public class Program
{
    public static void Main()
    {
        _ = BenchmarkRunner.Run<Benchmark>();
    }
}
