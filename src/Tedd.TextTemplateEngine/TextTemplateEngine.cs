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
            var s = templateString.AsSpan();
            StringBuilder sb = null; // If no replace happens then we won't create this object
            var startPos = 0;
            var inKeyword = false;
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
                    // First use of sb, create
                    if (sb == null)
                        sb = new StringBuilder();
                    
                    // Copy data, skip escape char
                    sb.Append(s.Slice(startPos, i - startPos-1));
                    startPos = i;
                    i++; 
                    inEscape = false;
                    continue;
                }

                if (s[i] == startChar)
                {
                    startPos = i;
                    inKeyword = true;
                    continue;
                }

                if (inKeyword)
                    continue;

                // Look for end char
                if (s[i] == endChar)
                {
                    inKeyword = false;
                    var keyword = s.Slice(startPos, i);
                    if (!lookup.TryGetValue(keyword.ToString(), out var value))
                        // No match in dictionary so we pretend like we never found a keyword.
                        continue;

                    // First use of sb, create
                    if (sb == null)
                        sb = new StringBuilder();

                    sb.Append(s.Slice(startPos, i - startPos));
                    sb.Append(value);
                    startPos = i;
                }
            }

            // Copy remainder if any
            if (startPos != s.Length - 1)
                sb.Append(s.Slice(startPos));

            // If sb is not null then we did replacement
            if (sb != null)
                // Return replaced string
                return sb.ToString();
            // Nothing changed, return string
            return templateString;
        }

    }
}