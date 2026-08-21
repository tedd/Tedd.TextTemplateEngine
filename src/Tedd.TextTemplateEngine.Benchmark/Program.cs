using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;

namespace Tedd.TextTemplateEngineBenchmark
{
    [MemoryDiagnoser]
    public class TemplateEngineBenchmark
    {
        private string _template;
        private Dictionary<string, string> _lookup;

        [GlobalSetup]
        public void Setup()
        {
            _template = "Hello [Name], you have [Count] new messages from [Sender]. [Nothing] This is a longer string [With] multiple [Replacements] and an escape \\[Name].";
            _lookup = new Dictionary<string, string>
            {
                { "Name", "John Doe" },
                { "Count", "5" },
                { "Sender", "System Administrator" },
                { "With", "with" },
                { "Replacements", "replacements" }
            };
        }

        [Benchmark(Baseline = true)]
        public string LegacyEngine()
        {
            return Tedd.Archive.TextTemplateEngine.Replace(_template, _lookup, '[', ']', '\\');
        }

        [Benchmark]
        public string ModernEngine()
        {
            return Tedd.TextTemplateEngine.Replace(_template, _lookup, '[', ']', '\\');
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<TemplateEngineBenchmark>();
        }
    }
}
