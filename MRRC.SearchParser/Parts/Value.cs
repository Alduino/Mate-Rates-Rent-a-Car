using System.Collections.Generic;

namespace MRRC.SearchParser.Parts
{
    public class Value
    {
        /// <summary>
        /// Parses a value or an inverted value
        /// </summary>
        public static IParseResult<Value> Parse(IEnumerator<Token.Match> tokens)
        {
            var token = tokens.Next();

            var inverted = false;
            if (token.Type == Token.Type.Not)
            {
                inverted = true;
                token = tokens.Next();
            }
            
            if (token.Type != Token.Type.Value) 
                return new FailedParseResult<Value>(Token.Type.Value, token, 
                    "Expecting a value type here - can be any text that isn't a keyword, or text surrounded by" +
                    "double quotes.");
            
            return new SuccessfulParseResult<Value>(
                new Value(inverted, token.Content.StartsWith("\"") ? 
                    token.Source.Substring(1, token.Source.Length - 2) : token.Source));
        }
        
        private Value(bool inverted, string source)
        {
            Inverted = inverted;
            Source = source;
        }
        
        public bool Inverted { get; }
        public string Source { get; }
    }
}