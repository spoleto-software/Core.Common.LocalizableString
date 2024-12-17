namespace Core.Common
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Count of sub-string occurrences within the string.
        /// </summary>
        public static int CountSubstring(this string text, string value)
        {
            return (text.Length - text.Replace(value, string.Empty).Length) / value.Length;
        }
    }
}
