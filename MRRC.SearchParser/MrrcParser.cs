using System.Collections.Generic;

namespace MRRC.SearchParser
{
    public class Parser
    {
        private static readonly Tokeniser Tokeniser = new Tokeniser(Token.TokenTypes);

        public Parser(string source)
        {
            Source = source;
        }

        private string Source { get; }

        public IEnumerable<Token.Match> Tokenise() => Tokeniser.Tokenise(Source);
    }
}