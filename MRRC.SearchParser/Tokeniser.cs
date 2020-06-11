using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.SearchParser
{
    public class Tokeniser
    {
        public Tokeniser(IMatcher[] matchers)
        {
            Matchers = matchers;
        }

        private IMatcher[] Matchers { get; }

        public IEnumerable<Token.Match> Tokenise(string source, bool neverFail = false, bool skipWhitespace = true)
        {
            var sourceLeft = source;

            while (sourceLeft.Length > 0)
            {
                var match = Matchers
                    .Select(m => Tuple.Create(m.Type, m.Match(sourceLeft)))
                    .FirstOrDefault(v => v.Item2 > 0);

                var index = source.Length - sourceLeft.Length;
                if (match == null)
                {
                    if (!neverFail) throw new TokenException($"Invalid token at {index}", index);
                    
                    yield return new Token.Match(Token.Type.Invalid, 
                        sourceLeft.Substring(0, 1), source, index);
                    sourceLeft = sourceLeft.Substring(1);
                    continue;
                }

                var matchText = sourceLeft.Substring(0, match.Item2);
                sourceLeft = sourceLeft.Substring(match.Item2);

                if (skipWhitespace && match.Item1 == Token.Type.Whitespace) continue;
                yield return new Token.Match(match.Item1, matchText, source, index);
            }
        }
    }
}