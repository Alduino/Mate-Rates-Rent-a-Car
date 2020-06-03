using System.Collections.Generic;

namespace MRRC.SearchParser.Parts
{
    public class Conjunction
    {
        public enum CType
        {
            And,
            Or
        }

        public static IParseResult<Conjunction> Parse(IEnumerator<Token.Match> tokens)
        {
            var next = tokens.Next();
            switch (next.Type)
            {
                case Token.Type.And:
                    return new SuccessfulParseResult<Conjunction>(new Conjunction(CType.And));
                case Token.Type.Or:
                    return new SuccessfulParseResult<Conjunction>(new Conjunction(CType.Or));
                default:
                    return new FailedParseResult<Conjunction>(
                        new [] { Token.Type.And, Token.Type.Or }, next, 
                        "Expecting AND or OR here");
            }
        }
        
        public Conjunction(CType type)
        {
            Type = type;
        }
        
        public CType Type { get; }
    }
}