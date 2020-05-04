using System.Text.RegularExpressions;

namespace MateRatesRentACar
{
    public static class StringExtensions
    {
        private static readonly Regex EmptyRegex = new Regex("^\\s+$");
        
        /// <summary>
        /// Returns true when the source is empty or only whitespace
        /// </summary>
        public static bool IsEmpty(this string source)
        {
            return source.Length == 0 || EmptyRegex.IsMatch(source);
        }
    }
}