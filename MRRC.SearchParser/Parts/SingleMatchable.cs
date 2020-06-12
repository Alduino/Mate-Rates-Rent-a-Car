using System;
using System.Collections.Generic;
using System.Linq;

namespace MRRC.SearchParser.Parts
{
    public class SingleMatchable : IMatchable
    {
        public SingleMatchable(Value value)
        {
            Value = value;
        }

        public Value Value { get; }

        public string[] Matches(string[] options)
        {
            if (!options.Contains(Value.Source, StringComparer.CurrentCultureIgnoreCase)) return new string[0];
            return new []
            {
                options.First(o => string.Equals(o, Value.Source, StringComparison.CurrentCultureIgnoreCase))
            };
        }

        public Tuple<Tuple<string, T>[], T[]> Matches<T>(Tuple<string, T>[] options)
        {
            var skipped = new List<T>();
            
            var data = options.Where(o =>
            {
                var result = string.Equals(o.Item1, Value.Source, StringComparison.CurrentCultureIgnoreCase);
                if (!Value.Inverted) return result;
                
                result = !result;
                if (!result) skipped.Add(o.Item2);
                return result;
            }).ToArray();
            
            return new Tuple<Tuple<string, T>[], T[]>(data, skipped.ToArray());
        }
    }
}