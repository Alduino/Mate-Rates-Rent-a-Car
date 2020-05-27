using System.Collections.Generic;

namespace MRRC.SearchParser
{
    public readonly struct FailedParseResult : IParseResult<object>
    {
        public FailedParseResult(IEnumerable<Token.Type> expectedTokens, Token.Match foundToken, string help)
        {
            ExpectedTokens = expectedTokens;
            FoundToken = foundToken;
            Help = help;
            Result = default;
        }

        public bool Successful => false;
        public object Result { get; }
        public string Message => $"Expected {string.Join(", ", ExpectedTokens)}, found {FoundToken.Type}. {Help}";
        public IEnumerable<Token.Type> ExpectedTokens { get; }
        public Token.Match FoundToken { get; }
        public string Help { get; }
    }
}