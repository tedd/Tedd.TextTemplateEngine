using System;
using BenchmarkDotNet.Running;

namespace Tedd.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TemplateEngineBenchmark>();
        }
    }
}
