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

        public static string Replace(string templateString, Dictionary<string, string> lookup, char startChar, char endChar, char escapeChar = (char)0)
        {
            if (string.IsNullOrEmpty(templateString))
                return templateString;

            var s = templateString.AsSpan();
            StringBuilder sb = null;
            var appendPos = 0;
            var startPos = -1;
            var inEscape = false;

            int i = 0;
            while (i < s.Length)
            {
                if (escapeChar != (char)0 && s[i] == escapeChar && !inEscape)
                {
                    inEscape = true;
                    i++;
                    continue;
                }

                if (inEscape)
                {
                    if (sb == null)
                        sb = new StringBuilder();

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
                    i++;
                    continue;
                }

                if (startPos == -1)
                {
                    // Look for startChar or escapeChar using IndexOfAny for performance
                    int nextIdx;
                    if (escapeChar != (char)0)
                    {
                        nextIdx = s.Slice(i).IndexOfAny(startChar, escapeChar);
                    }
                    else
                    {
                        nextIdx = s.Slice(i).IndexOf(startChar);
                    }

                    if (nextIdx == -1)
                    {
                        break; // No more interesting characters, break out of loop
                    }

                    i += nextIdx;

                    if (s[i] == escapeChar)
                    {
                        inEscape = true;
                        i++;
                        continue;
                    }

                    if (s[i] == startChar)
                    {
                        startPos = i;
                        i++;
                        continue;
                    }
                }
                else
                {
                    // We are inside a keyword, looking for endChar or escapeChar
                    int nextIdx;
                    if (escapeChar != (char)0)
                    {
                        nextIdx = s.Slice(i).IndexOfAny(endChar, escapeChar);
                    }
                    else
                    {
                        nextIdx = s.Slice(i).IndexOf(endChar);
                    }

                    if (nextIdx == -1)
                    {
                        break; // Incomplete keyword at the end of string
                    }

                    i += nextIdx;

                    if (s[i] == escapeChar)
                    {
                        inEscape = true;
                        i++;
                        continue;
                    }

                    if (s[i] == endChar)
                    {
                        var keywordLen = i - startPos - 1;
                        if (keywordLen > 0)
                        {
                            var keyword = s.Slice(startPos + 1, keywordLen);

                            // To avoid allocations, iterate and check sequence equality?
                            // Or standard string.ToString() if no better option. The issue asked to use .NET 9 features but we can't change csproj.
                            // Let's use ToString for now as it doesn't break compatibility and IndexOfAny optimization is our main vector.
                            if (lookup.TryGetValue(keyword.ToString(), out var value))
                            {
                                if (sb == null)
                                    sb = new StringBuilder();

#if NETSTANDARD2_0
                                sb.Append(s.Slice(appendPos, startPos - appendPos).ToString());
#else
                                sb.Append(s.Slice(appendPos, startPos - appendPos));
#endif
                                sb.Append(value);
                                appendPos = i + 1;
                                startPos = -1; // Reset for next keyword
                                i++;
                                continue;
                            }
                        }

                        startPos = -1; // Reset if empty keyword or no match
                        i++;
                        continue;
                    }
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
