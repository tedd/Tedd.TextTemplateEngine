using System;
using System.Collections.Generic;
using System.Text;

namespace Tedd
{
    public class TextTemplateEngine
    {
        public readonly char StartChar;
        public readonly char EndChar;
        public readonly char EscapeChar;

        public TemplateMatchType MatchType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startChar">Char in front of variable to replace, for example '['.</param>
        /// <param name="endChar">Char after variable to replace, for example ']'.</param>
        /// <param name="escapeChar">Escape char. 0=none. For example set to '\' then \[test] will be come [test] instead of being processed as replacement keyword.</param>
        public TextTemplateEngine(char startChar, char endChar, char escapeChar = (char)0)
        {
            StartChar = startChar;
            EndChar = endChar;
            EscapeChar = escapeChar;
        }

        /// <summary>
        /// Replaces tokens within a template string.
        /// Time Complexity: O(n) where n is the length of the string, leveraging vectorized span searches (IndexOfAny) to amortize traversal costs.
        /// Space Complexity: O(n) in the worst case where string concatenation requires allocating a StringBuilder proportional to the output. No intermediate string allocations are produced for scanning.
        /// </summary>
        public static string Replace(string templateString, Dictionary<string, string> lookup, char startChar, char endChar, char escapeChar = (char)0)
        {
            if (string.IsNullOrEmpty(templateString)) return templateString;

            var s = templateString.AsSpan();
            StringBuilder sb = null;
            var appendPos = 0;
            var startPos = -1;

            for (var i = 0; i < s.Length;)
            {
                int nextIdx;
                if (escapeChar != '\0')
                {
                    nextIdx = s.Slice(i).IndexOfAny(escapeChar, startPos == -1 ? startChar : endChar);
                }
                else
                {
                    nextIdx = s.Slice(i).IndexOf(startPos == -1 ? startChar : endChar);
                }

                if (nextIdx == -1)
                {
                    break;
                }

                i += nextIdx;
                char c = s[i];

                if (escapeChar != '\0' && c == escapeChar)
                {
                    if (i + 1 >= s.Length)
                    {
                        break;
                    }
                    if (sb == null) sb = new StringBuilder();
#if NETSTANDARD2_0
                    sb.Append(s.Slice(appendPos, i - appendPos).ToString());
#else
                    sb.Append(s.Slice(appendPos, i - appendPos));
#endif
                    appendPos = i + 1;
                    i += 2;
                    if (startPos != -1) startPos = -1;
                    continue;
                }

                if (c == startChar && startPos == -1)
                {
                    startPos = i;
                    i++;
                    continue;
                }

                if (c == endChar && startPos != -1)
                {
                    var keywordLen = i - startPos - 1;
                    if (keywordLen > 0)
                    {
                        var keyword = s.Slice(startPos + 1, keywordLen);
                        if (lookup.TryGetValue(keyword.ToString(), out var value))
                        {
                            if (sb == null) sb = new StringBuilder();
#if NETSTANDARD2_0
                            sb.Append(s.Slice(appendPos, startPos - appendPos).ToString());
#else
                            sb.Append(s.Slice(appendPos, startPos - appendPos));
#endif
                            sb.Append(value);
                            appendPos = i + 1;
                            startPos = -1;
                            i++;
                            continue;
                        }
                    }
                    startPos = -1;
                    i++;
                    continue;
                }

                i++;
            }

            if (sb != null)
            {
                if (appendPos < s.Length)
#if NETSTANDARD2_0
                    sb.Append(s.Slice(appendPos).ToString());
#else
                    sb.Append(s.Slice(appendPos));
#endif
                return sb.ToString();
            }

            return templateString;
        }

    }
}
