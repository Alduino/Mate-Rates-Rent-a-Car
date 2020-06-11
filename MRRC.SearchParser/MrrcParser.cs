using System.Collections.Generic;
using MRRC.SearchParser.Parts;

namespace MRRC.SearchParser
{
    public class MrrcParser
    {
        private static readonly Tokeniser Tokeniser = new Tokeniser(Token.TokenTypes);

        public MrrcParser(string source)
        {
            Source = source;
        }

        private string Source { get; }

        public IEnumerable<Token.Match> Tokenise() => Tokeniser.Tokenise(Source);

        public IParseResult<Expression> Parse(IEnumerable<Token.Match> tokens) =>
            Expression.Parse(tokens.GetLLEnumerator());
    }
}