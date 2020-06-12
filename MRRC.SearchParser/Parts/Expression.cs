using System;

namespace MRRC.SearchParser.Parts
{
    public class Expression : IMatchable
    {
        private static IParseResult<Expression> ParseValue(LLEnumerator<Token.Match> tokens)
        {
            var a = Parts.Value.Parse(tokens);
            if (a is FailedParseResult<Value> aFailure) return aFailure.Cast<Expression>();

            if (tokens.LookAhead(Token.Match.Eof).Type != Token.Type.And &&
                tokens.LookAhead(Token.Match.Eof).Type != Token.Type.Or)
                return a.Then(v => new Expression(v));
            
            var conjunction = Conjunction.Parse(tokens);

            var b = Parse(tokens);

            return a.AndThen(conjunction, b, (aR, cR, bR) => new Expression(aR, cR, bR));
        }

        /// <summary>
        /// Parses an expression, which can be a value or two expressions separated by an AND or OR.
        /// May also start with an open bracket, which will just parse the expression inside.
        /// </summary>
        /// <param name="tokens">List of tokens from tokeniser</param>
        /// <returns>Expression result</returns>
        public static IParseResult<Expression> Parse(LLEnumerator<Token.Match> tokens)
        {
            switch (tokens.LookAhead(Token.Match.Eof).Type)
            {
                case Token.Type.Not:
                case Token.Type.Value:
                    return ParseValue(tokens);
                case Token.Type.OpenBracket:
                    tokens.Next();
                    var res = Parse(tokens);
                    var next = tokens.Next();
                    if (next.Type != Token.Type.CloseBracket)
                        return new FailedParseResult<Expression>(Token.Type.CloseBracket, next,
                            "A closing bracket should be used after an expression that started with " +
                            "an opening bracket.");
                    return res;
                default:
                    return new FailedParseResult<Expression>(
                        new [] {Token.Type.Not, Token.Type.Value, Token.Type.OpenBracket},
                        tokens.LookAhead(Token.Match.Eof), "An expression should start with NOT, (, or a value.");
            }
        }

        private Expression(Value val)
        {
            Value = new SingleMatchable(val);
        }

        private Expression(Value a, Conjunction conjunction, Value b)
        {
            Value = new ConjunctionMatchable(
                new SingleMatchable(a),
                conjunction,
                new SingleMatchable(b)
            );
        }

        private Expression(Value a, Conjunction conjunction, IMatchable b)
        {
            Value = new ConjunctionMatchable(
                new SingleMatchable(a),
                conjunction,
                b
            );
        }
        
        public IMatchable Value { get; }

        public string[] Matches(string[] options) => Value.Matches(options);
        public Tuple<string, T>[] Matches<T>(Tuple<string, T>[] options) => Value.Matches(options);
    }
}