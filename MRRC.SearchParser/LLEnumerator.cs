using System;
using System.Collections;
using System.Collections.Generic;

namespace MRRC.SearchParser
{
    /// <summary>
    /// Infinite and lazy look-ahead enumerator
    /// </summary>
    /// <remarks>
    /// This class will not enumerate multiple times, instead storing viewed data in a temporary queue
    /// </remarks>
    /// <typeparam name="T">Type of the values stored</typeparam>
    public class LLEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _base;
        private readonly RollingEnumerator<T> _lookedAt = new RollingEnumerator<T>();

        private bool MoveLookAhead()
        {
            var res = _lookedAt.Shift(out var curr);
            Current = curr;
            return res;
        }

        private bool MoveBase()
        {
            var res = _base.MoveNext();
            Current = _base.Current;
            return res;
        }

        private bool MoveNext(out bool fromCache)
        {
            if (_lookedAt.HasItems())
            {
                fromCache = true;
                return MoveLookAhead();
            }

            fromCache = false;
            return MoveBase();
        }
        
        public LLEnumerator(IEnumerator<T> @base)
        {
            _base = @base;
        }

        public bool MoveNext() => _lookedAt.HasItems() ? MoveLookAhead() : MoveBase();

        public void Reset() => _base.Reset();

        /// <summary>
        /// Looks ahead in the enumerator and returns the specified item
        /// </summary>
        /// <param name="count">The amount of items to look ahead by</param>
        /// <exception cref="ArgumentOutOfRangeException">count is not over or equal to 1</exception>
        /// <exception cref="IndexOutOfRangeException">reached the end of the enumerator</exception>
        public T LookAhead(int count = 1)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), "Count must be at least 1");

            if (count <= _lookedAt.Length) return _lookedAt[count - 1];
            
            var tempItems = new RollingEnumerator<T>();
            
            for (var i = 0; i < count; i++)
            {
                var canLookAhead = MoveNext();
                if (!canLookAhead) throw new IndexOutOfRangeException("Reached end of enumerator");
                tempItems.Push(Current);
            }

            tempItems.PrependInto(_lookedAt);
            
            return Current;
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _base.Dispose();
        }
    }
}