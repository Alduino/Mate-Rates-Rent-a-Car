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

        /// <summary>
        /// Same as <see cref="Then{TI,TV}"/>, but supports multiple items
        /// </summary>
        public static IParseResult<TV> AndThen<TA, TB, TC, TV>(this IParseResult<TA> a, IParseResult<TB> b,
            IParseResult<TC> c, Func<TA, TB, TC, TV> map)
        {
            if (a is FailedParseResult<TA> fA) return fA.Cast<TV>();
            if (b is FailedParseResult<TA> fB) return fB.Cast<TV>();
            if (c is FailedParseResult<TA> fC) return fC.Cast<TV>();

            var rA = (SuccessfulParseResult<TA>) a;
            var rB = (SuccessfulParseResult<TB>) b;
            var rC = (SuccessfulParseResult<TC>) c;
            
            return new SuccessfulParseResult<TV>(map(rA.Result, rB.Result, rC.Result));
        }
    }
    
    public interface IParseResult<out T>
    {
        bool Successful { get; }
        T Result { get; }
    }
}