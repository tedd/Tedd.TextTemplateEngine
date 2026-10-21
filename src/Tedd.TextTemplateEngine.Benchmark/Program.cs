using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;

namespace TextTemplateEngineBenchmark
{
    [MemoryDiagnoser]
    public class TemplateBenchmark
    {
        private string template;
        private Dictionary<string, string> lookup;

        [GlobalSetup]
        public void Setup()
        {
            template = "System metrics for [Hostname] indicate operational status. Use \\[Escape\\] to emit raw delimiters. This is a longer template to show [SomeKey] differences. [AnotherKey] is here.";
            lookup = new Dictionary<string, string>
            {
                { "Hostname", "Nexus-Core-01" },
                { "SomeKey", "SomeValue" },
                { "AnotherKey", "AnotherValue" }
            };
        }

        [Benchmark(Baseline = true)]
        public string RunArchive()
        {
            return Tedd.Archive.TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
        }

        [Benchmark]
        public string RunOptimized()
        {
            return Tedd.TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TemplateBenchmark>();
        }
    }
}
