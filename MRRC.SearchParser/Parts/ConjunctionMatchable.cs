using System;
using System.Collections.Generic;
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

        public Tuple<Tuple<string, T>[], T[]> Matches<T>(Tuple<string, T>[] options)
        {
            // exactly the same as string[] Matches but with a different data type
            var leftMatches = Left.Matches(options);
            var rightMatches = Right.Matches(options);
            
            if (leftMatches.Item1.Length > 0 && Conjunction.Type == Conjunction.CType.And)
            {
                return new Tuple<Tuple<string, T>[], T[]>(
                    leftMatches.Item1.Where(a => 
                        rightMatches.Item1.Any(b =>
                            EqualityComparer<T>.Default.Equals(a.Item2, b.Item2))).ToArray(),
                    leftMatches.Item2.Concat(rightMatches.Item2).ToArray()
                );
            }
            
            return new Tuple<Tuple<string, T>[], T[]>(
                leftMatches.Item1.Concat(rightMatches.Item1).ToArray(),
                leftMatches.Item2.Concat(rightMatches.Item2).ToArray()
            );

            return Conjunction.Type == Conjunction.CType.Or ?
                Right.Matches(options) : new Tuple<Tuple<string, T>[], T[]>(new Tuple<string, T>[0], new T[0]);
        }
    }
}