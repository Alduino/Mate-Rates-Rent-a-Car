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

        public IEnumerable<Token.Match> Tokenise(string source)
        {
            var sourceLeft = source;

            while (sourceLeft.Length > 0)
            {
                var match = Matchers
                    .Select(m => Tuple.Create(m.Type, m.Match(sourceLeft)))
                    .FirstOrDefault(v => v.Item2 > 0);

                var index = source.Length - sourceLeft.Length;
                if (match == null) throw new TokenException($"Invalid token at {index}", index);

                var matchText = sourceLeft.Substring(0, match.Item2);
                sourceLeft = sourceLeft.Substring(match.Item2);
                
                yield return new Token.Match(match.Item1, matchText, source, index);
            }
        }
    }
}