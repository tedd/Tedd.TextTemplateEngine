using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tedd
{
    public class TextTemplateEngine
    {
        public readonly char StartChar;
        public readonly char EndChar;
        public readonly char EscapeChar;
        private readonly Regex _regex;

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="regex">Regex to match variables, for example:
        ///// @"(?!\\)\[[a-zA-Z0-9]+\]" Letters a-z, A-Z and 0-9 inside [ ] where [ is not preceeded by \. So [this] but not \[this].
        ///// @"\$\p{L}+\$" Any unicode alphabet (any country) letters inside $ and $. So $𝓽𝓱𝓲𝓼$ and $ፕዘልፕ$.
        ///// @"\$[^\$]+\$" Anything between $ and $. So $t^h*i-s$
        ///// @"\[[^\]]+\]" Anything between [ and ]. So [-=*THIS*=-].
        ///// </param>
        //public TemplateEngine(string regex) : this(new Regex(regex, RegexOptions.Compiled))
        //{ }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="regex">Custom regex matcher. First capture must be variable match.</param>
        //public TemplateEngine(Regex regex, char[] escapedCleanup)
        //{
        //    _regex = regex;
        //}

        //public static string Replace(Regex regex, string templateString, Dictionary<string, string> lookup)
        //{
        //    var content = Regex.Replace(template, @"(?<!\\)\[(?<word>[^\]]{1,50})\](?<!\\)", delegate (Match match)
        //    {
        //        var word = match.Groups["word"].Value;
        //        string ret = null;
        //        if (data.TryGetValue(word, out ret))
        //            return ret;
        //        return match.ToString();
        //    }, RegexOptions.Compiled);
        //    // Replace \[ with [ and \] with ]
        //    content = Regex.Replace(content, @"\\([\[\]])", @"$1", RegexOptions.Compiled);
        //}

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
                    
                    sb.Append(s.Slice(appendPos, i - appendPos - 1)); // Append everything up to the escape character
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
                            sb.Append(s.Slice(appendPos, startPos - appendPos));
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
                    sb.Append(s.Slice(appendPos));
                return sb.ToString();
            }

            return templateString;
        }

    }
}