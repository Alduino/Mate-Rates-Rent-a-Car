using System;
using System.Linq;

namespace MRRC.SearchParser.Parts
{
    public class ConjunctionMatchable : IMatchable
    {
        public ConjunctionMatchable(IMatchable left, Conjunction conjunction, IMatchable right)
        {
            Left = left;
            Conjunction = conjunction;
            Right = right;
        }

        public IMatchable Left { get; }
        public Conjunction Conjunction { get; }
        public IMatchable Right { get; }

        public string[] Matches(string[] options)
        {
            var leftMatches = Left.Matches(options);
            if (leftMatches.Length > 0 && Conjunction.Type == Conjunction.CType.And) 
                return leftMatches.Concat(Right.Matches(options)).ToArray();
            return Conjunction.Type == Conjunction.CType.Or ? Right.Matches(options) : new string[0];
        }

        public Tuple<string, T>[] Matches<T>(Tuple<string, T>[] options)
        {
            // exactly the same as string[] Matches but with a different data type
            var leftMatches = Left.Matches(options);
            if (leftMatches.Length > 0 && Conjunction.Type == Conjunction.CType.And) 
                return leftMatches.Concat(Right.Matches(options)).ToArray();
            return Conjunction.Type == Conjunction.CType.Or ? Right.Matches(options) : new Tuple<string, T>[0];
        }
    }
}