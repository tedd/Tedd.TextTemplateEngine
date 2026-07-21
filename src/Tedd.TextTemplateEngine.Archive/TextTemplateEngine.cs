using System;
using System.Collections.Generic;
using System.Text;

namespace Tedd.Archive
{
    public class TextTemplateEngine
    {
        public readonly char StartChar;
        public readonly char EndChar;
        public readonly char EscapeChar;

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
