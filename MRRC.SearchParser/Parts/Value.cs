using System.Collections.Generic;

namespace MRRC.SearchParser.Parts
{
    public class Value
    {
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
                return new FailedParseResult<Value>(Token.Type.Value, token, "");
            
            return new SuccessfulParseResult<Value>(new Value(inverted, token.Content));
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