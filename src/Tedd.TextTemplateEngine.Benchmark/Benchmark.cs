using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Tedd.Benchmark
{
    [MemoryDiagnoser]
    public class TemplateEngineBenchmark
    {
        private string template;
        private Dictionary<string, string> lookup;

        [GlobalSetup]
        public void Setup()
        {
            lookup = new Dictionary<string, string> { { "key", "VAL" }, { "other", "OTHER" } };
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append("This is a [key] with some \\[escaped] tags and a [other] right here. Let's make it long enough to measure differences. ");
            }
            template = sb.ToString();
        }

        [Benchmark(Baseline = true)]
        public string Original()
        {
            return Tedd.Archive.TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
        }

        [Benchmark]
        public string Optimized()
        {
            return Tedd.TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
        }
    }
}
