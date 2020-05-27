using System;
using System.Runtime.Serialization;

namespace MRRC.SearchParser
{
    [Serializable]
    public class TokenException : Exception
    {
        public int Index { get; }

        public TokenException()
        {
        }

        public TokenException(string message, int index) : base(message)
        {
            Index = index;
        }

        public TokenException(string message, int index, Exception inner) : base(message, inner)
        {
            Index = index;
        }

        protected TokenException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}