using System.Text.RegularExpressions;

namespace MateRatesRentACar
{
    public static class StringExtensions
    {
        private static readonly Regex EmptyRegex = new Regex("^\\s+$");
        
        public static bool IsEmpty(this string source)
        {
            return source.Length == 0 || EmptyRegex.IsMatch(source);
        }
    }
}