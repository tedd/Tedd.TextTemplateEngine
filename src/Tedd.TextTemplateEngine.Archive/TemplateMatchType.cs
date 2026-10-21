using System;

namespace Tedd.Archive
{
    public enum TemplateMatchType
    {
        /// <summary>
        /// Fast implementation, but only supports simplified start/end patterns
        /// </summary>
        FastNaive,
        /// <summary>
        /// Slightly slower, but supports complex regex to identify strings to replace
        /// </summary>
        Regex
    }
}
