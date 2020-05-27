using System.Text.RegularExpressions;

namespace MRRC.SearchParser.Matcher
{
    /// <summary>
    /// Matches the specified regex
    /// </summary>
    public class RegexMatcher : IMatcher
    {
        private Regex Regex { get; }

        public RegexMatcher(Token.Type type, Regex regex)
        {
            Type = type;
            Regex = regex;
        }
        
        public RegexMatcher(Token.Type type, string regex) : this(type, new Regex(regex)) {}

        public Token.Type Type { get; }

        public int Match(string remaining) => Regex.Match(remaining).Length;
    }
}