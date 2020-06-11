using System.Text.RegularExpressions;
using MRRC.SearchParser.Matcher;

namespace MRRC.SearchParser
{
    public class Token
    {
        /// <summary>
        /// List of token
        /// </summary>
        public enum Type
        {
            And,
            Or,
            Not,
            Value,
            OpenBracket,
            CloseBracket,
            Invalid
        }

        public static IMatcher[] TokenTypes { get; } = {
            new LiteralMatcher(Type.And, "AND"),
            new LiteralMatcher(Type.Or, "OR"),
            new LiteralMatcher(Type.Not, "NOT"),
            // following regex from https://stackoverflow.com/a/30737232
            new RegexMatcher(Type.Value, "^(?:\"(?:[^\"\\\\]*(?:\\\\.)?)*\")|^\\w+"),
            new LiteralMatcher(Type.OpenBracket, "("),
            new LiteralMatcher(Type.CloseBracket, ")")
        };

        public readonly struct Match
        {
            public Match(Type type, string content, string source, int index)
            {
                Type = type;
                Content = content;
                Source = source;
                Index = index;
            }

            public Type Type { get; }
            public string Content { get; }
            public string Source { get; }
            public int Index { get; }
        }
    }
}