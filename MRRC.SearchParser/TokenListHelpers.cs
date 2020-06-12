using System;
using System.Collections.Generic;

namespace MRRC.SearchParser
{
    public static class TokenListHelpers
    {
        public static Token.Match Next(this IEnumerator<Token.Match> list)
        {
            if (!list.MoveNext()) throw new IndexOutOfRangeException("Reached end of token stream");
            return list.Current;
        }

        public static Token.Match Next(this IEnumerator<Token.Match> list, Token.Match def) =>
            !list.MoveNext() ? def : list.Current;

        // TODO lookahead support
        // This might not be needed but here's the link for my future reference:
        // https://www.codeproject.com/Tips/5254600/LookAheadEnumerator-Implement-Backtracking-in-Your
    }
}