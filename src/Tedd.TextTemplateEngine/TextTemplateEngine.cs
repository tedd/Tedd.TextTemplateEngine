using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

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
        /// Replaces keywords in a string.
        /// Time Complexity: O(N) where N is the length of templateString, as it requires a single pass over the string.
        /// Space Complexity: O(N + M) where M is the combined length of the substituted strings. Memory is optimized by predicting StringBuilder capacity.
        /// </summary>
        public static string Replace(string templateString, Dictionary<string, string> lookup, char startChar, char endChar, char escapeChar = (char)0)
        {
#if NET9_0_OR_GREATER
            var lookupSpan = lookup.GetAlternateLookup<ReadOnlySpan<char>>();
#endif

            if (string.IsNullOrEmpty(templateString))
                return templateString;

            var s = templateString.AsSpan();
            StringBuilder sb = null;
            var appendPos = 0;
            var startPos = -1;
            var inEscape = false;

            for (var i = 0; i < s.Length; i++)
            {
                if (escapeChar != (char)0 && s[i] == escapeChar && !inEscape)
                {
                    inEscape = true;
                    continue;
                }

                if (inEscape)
                {
                    if (sb == null)
                        sb = new StringBuilder(templateString.Length);

#if NETSTANDARD2_0
                    sb.Append(s.Slice(appendPos, i - appendPos - 1).ToString());
#else
                    sb.Append(s.Slice(appendPos, i - appendPos - 1));
#endif
                    appendPos = i;
                    inEscape = false;
                    if (startPos != -1)
                    {
                        startPos = -1;
                    }
                    continue;
                }

                if (s[i] == startChar && startPos == -1)
                {
                    startPos = i;
                    continue;
                }

                if (startPos != -1 && s[i] == endChar)
                {
                    var keywordLen = i - startPos - 1;
                    if (keywordLen > 0)
                    {
                        var keyword = s.Slice(startPos + 1, keywordLen);
                        bool match = false;
                        string value = null;

#if NET9_0_OR_GREATER
                        match = lookupSpan.TryGetValue(keyword, out value);
#else
                        var keyStr = keyword.ToString();
                        match = lookup.TryGetValue(keyStr, out value);
#endif
                        if (match)
                        {
                            if (sb == null)
                                sb = new StringBuilder(templateString.Length + value.Length);

#if NETSTANDARD2_0
                            sb.Append(s.Slice(appendPos, startPos - appendPos).ToString());
#else
                            sb.Append(s.Slice(appendPos, startPos - appendPos));
#endif
                            sb.Append(value);
                            appendPos = i + 1;
                            startPos = -1;
                            continue;
                        }
                    }
                    startPos = -1;
                }
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
