namespace MRRC.SearchParser.Matcher
{
    public class LiteralMatcher : IMatcher
    {
        public LiteralMatcher(Token.Type type, string literal)
        {
            Type = type;
            Literal = literal;
        }

        private string Literal { get; }
        
        public Token.Type Type { get; }

        public int Match(string remaining)
        {
            return remaining.StartsWith(Literal) ? Literal.Length : 0;
        }
    }
}