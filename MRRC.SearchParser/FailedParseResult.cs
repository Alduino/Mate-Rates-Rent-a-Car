using System.Collections.Generic;
using MRRC.SearchParser.Parts;

namespace MRRC.SearchParser
{
    public readonly struct FailedParseResult<T> : IParseResult<T>
    {
        public FailedParseResult(IEnumerable<Token.Type> expectedTokens, Token.Match foundToken, string help)
        {
            ExpectedTokens = expectedTokens;
            FoundToken = foundToken;
            Help = help;
            Result = default;
        }

        public FailedParseResult(Token.Type expected, Token.Match found, string help) :
            this(new[] {expected}, found, help) {}

        public bool Successful => false;
        public T Result { get; }
        public string Message => $"Expected {string.Join(", ", ExpectedTokens)}, found {FoundToken.Type}. {Help}";
        public IEnumerable<Token.Type> ExpectedTokens { get; }
        public Token.Match FoundToken { get; }
        public string Help { get; }
    }
}