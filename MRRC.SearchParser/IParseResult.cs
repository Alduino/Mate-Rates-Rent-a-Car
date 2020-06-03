using System;

namespace MRRC.SearchParser
{
    public static class ParseResultExtensions
    {
        /// <summary>
        /// When the parse result is failed, returns that, otherwise returns a successful parse result
        /// containing the mapped data
        /// </summary>
        public static IParseResult<TV> Then<TI, TV>(this IParseResult<TI> parseResult, Func<TI, TV> map)
        {
            if (parseResult is FailedParseResult<TI> failure) return failure.Cast<TV>();
            
            var res = (SuccessfulParseResult<TI>) parseResult;
            return new SuccessfulParseResult<TV>(map(res.Result));
        }
    }
    
    public interface IParseResult<out T>
    {
        bool Successful { get; }
        T Result { get; }
    }
}