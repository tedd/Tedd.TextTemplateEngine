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
            StringBuilder sb = null; // If no replace happens then we won't create this object
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
                    // If we just encountered an escape character, we append the literal text up to the escape character
                    if (sb == null)
                        sb = new StringBuilder();
                    
#if NETSTANDARD2_0
                    sb.Append(s.Slice(appendPos, i - appendPos - 1).ToString());
#else
                    sb.Append(s.Slice(appendPos, i - appendPos - 1));
#endif
                    appendPos = i; // Next text begins at the escaped character itself
                    inEscape = false;
                    // Note: If we are inside a keyword, escaping characters inside a keyword means we should probably abandon the keyword.
                    // But typically, escape is for escaping the start character or another escape.
                    if (startPos != -1)
                    {
                        // Found escaped char inside keyword, abort keyword mode
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
                        if (lookup.TryGetValue(keyword.ToString(), out var value))
                        {
                            if (sb == null)
                                sb = new StringBuilder();

                            // Append everything since last appendPos up to the start character of this keyword
#if NETSTANDARD2_0
                            sb.Append(s.Slice(appendPos, startPos - appendPos).ToString());
#else
                            sb.Append(s.Slice(appendPos, startPos - appendPos));
#endif
                            sb.Append(value);
                            appendPos = i + 1;
                            startPos = -1; // Reset for next keyword
                            continue;
                        }
                    }
                    // No match or empty keyword, pretend it's not a keyword
                    startPos = -1;
                }
            }

            if (sb != null)
            {
                // Copy remainder
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