using System;
using System.Collections.Generic;
using Xunit;
using Tedd;

namespace Tedd.TextTemplateEngineTests
{
    public class TextTemplateEngineTests
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", null, "")]
        [InlineData("hello", null, "hello")]
        [InlineData("hello [world]", null, "hello [world]")] // no match, keeps original
        public void EdgeCases_NullOrEmpty(string template, Dictionary<string, string> overrideLookup, string expected)
        {
            var lookup = overrideLookup ?? new Dictionary<string, string>();
            var result = TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("hello [world]", "world", "earth", "hello earth")]
        [InlineData("[world] hello", "world", "earth", "earth hello")]
        [InlineData("hello [world]!", "world", "earth", "hello earth!")]
        [InlineData("[world]", "world", "earth", "earth")]
        [InlineData("[-[world]-]", "world", "earth", "[-[world]-]")]
        public void BasicReplacements(string template, string key, string value, string expected)
        {
            var lookup = new Dictionary<string, string> { { key, value } };
            var result = TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("hello \\[world]", "world", "earth", "hello [world]")]
        [InlineData("hello \\\\[world]", "world", "earth", "hello \\earth")]
        public void EscapeCharacters(string template, string key, string value, string expected)
        {
            var lookup = new Dictionary<string, string> { { key, value } };
            var result = TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("hello {world}", "world", "earth", "hello earth")]
        [InlineData("hello <world>", "world", "earth", "hello earth")]
        [InlineData("hello $world$", "world", "earth", "hello earth")]
        public void CustomDelimiters(string template, string key, string value, string expected)
        {
            var lookup = new Dictionary<string, string> { { key, value } };

            char startChar = template[6]; // Grab the first delimiter
            char endChar = template[template.Length - 1]; // Grab the last delimiter

            var result = TextTemplateEngine.Replace(template, lookup, startChar, endChar, '\\');
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("abc [boop] [key] ghi", "def", "abc [boop] def ghi")]
        [InlineData("abc[boop][key]ghi", "def", "abc[boop]defghi")]
        [InlineData("abc [key] [key] ghi", "def", "abc def def ghi")]
        [InlineData("abc [missing] ghi", "def", "abc [missing] ghi")]
        public void MultipleKeywordsAndMissing(string template, string keyValue, string expected)
        {
            var lookup = new Dictionary<string, string> { { "key", keyValue } };
            var result = TextTemplateEngine.Replace(template, lookup, '[', ']', '\\');
            Assert.Equal(expected, result);
        }
    }
}
