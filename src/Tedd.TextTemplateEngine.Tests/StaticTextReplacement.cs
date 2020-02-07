using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace Tedd.TextTemplateEngineTests
{
    public class StaticTextReplacement
    {
        private const int Iterations = 1000;
        private Random _random = new Random();

        [Fact]
        public void EscapeTest()
        {
            var template = @"abc\[boop][key]ghi[key]\\[beep]";
            var answer = @"abc[boop]defghidef\[beep]";
            var result = TextTemplateEngine.Replace(template, new Dictionary<string, string>() { { "key", "def" } }, '[', ']', '\\');
            Assert.Equal(answer, result);
        }

        [Fact]
        public void FuzzTest()
        {

            //var tte = new TextTemplateEngine();
            var templateStringBuilder = new StringBuilder();
            var resultStringBuilder = new StringBuilder();
            var lookup = new Dictionary<string, string>();
            for (var n = 0; n < Iterations; n++)
            {
                templateStringBuilder.Clear();
                resultStringBuilder.Clear();
                for (var i = 0; i < 1000; i++)
                {
                    if (_random.Next(0, 10) == 0)
                    {
                        var rk = _random.NextString("abcdefghijklmnopqrstuvwxyz0123456789", _random.Next(1, 20));
                        var rv = _random.NextString("abcdefghijklmnopqrstuvwxyz0123456789", _random.Next(0, 20));
                        lookup.Add(rk, rv);
                        templateStringBuilder.Append($"[{rk}]");
                        resultStringBuilder.Append(rv);
                    }

                    var rs = _random.NextString("abcdefghijklmnopqrstuvwxyz0123456789", _random.Next(1, 20));
                    templateStringBuilder.Append(rs);
                    resultStringBuilder.Append(rs);

                }


                var result = TextTemplateEngine.Replace(templateStringBuilder.ToString(), lookup, '[', ']', '\\');
                Assert.Equal(resultStringBuilder.ToString(), result);
            }


        }
    }
}
