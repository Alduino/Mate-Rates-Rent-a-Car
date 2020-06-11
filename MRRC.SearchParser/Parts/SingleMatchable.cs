using System;
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
    }
}