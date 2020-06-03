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

        /// <summary>
        /// Returns a FailedParseResult containing the same data, but with a different generic type
        /// </summary>
        /// <typeparam name="TO">New generic type for the result</typeparam>
        public FailedParseResult<TO> Cast<TO>()
        {
            return new FailedParseResult<TO>(ExpectedTokens, FoundToken, Help);
        }

        public bool Successful => false;
        public T Result { get; }
        public string Message => $"Expected {string.Join(", ", ExpectedTokens)}, found {FoundToken.Type}. {Help}";
        public IEnumerable<Token.Type> ExpectedTokens { get; }
        public Token.Match FoundToken { get; }
        public string Help { get; }
    }
}