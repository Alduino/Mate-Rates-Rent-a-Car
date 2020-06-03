using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MRRC.SearchParser
{
    /// <summary>
    /// Linked list that allows pushing to the end and shifting out from the start
    /// </summary>
    /// <remarks>
    /// Provides O(1) pushing and shifting, and O(n) indexing.
    /// </remarks>
    public class RollingEnumerator<T> : IEnumerable<T>
    {
        private class Item
        {
            public Item(T value, Item next)
            {
                Value = value;
                Next = next;
            }

            public T Value { get; internal set; }
            public Item Next { get; set; }
        }

        private Item _first;
        private Item _last;

        private Item GetItem(int index)
        {
            if (index >= Length) throw new IndexOutOfRangeException();
            var nextItem = _first;
            for (var i = 1; i <= index; i++) nextItem = nextItem.Next;
            return nextItem;
        }

        /// <summary>
        /// The current length of the list
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Removes the first item, and sets result to it (or null if false is returned)
        /// </summary>
        /// <returns>
        /// True if there was an item left to remove, false if not
        /// </returns>
        public bool Shift(out T result)
        {
            if (_first == null)
            {
                result = default;
                return false;
            }

            Length--;

            var value = _first.Value;
            _first = _first.Next;
            result = value;
            return true;
        }

        /// <summary>
        /// Inserts a new item at the start of the list
        /// </summary>
        public void Unshift(T value)
        {
            _first = new Item(value, _first);
            Length++;
        }

        /// <summary>
        /// Appends an item to the end of the list
        /// </summary>
        public void Push(T value)
        {
            var item = new Item(value, null);
            _last.Next = item;
            _last = item;
            if (_first == null) _first = item;
            Length++;
        }

        /// <summary>
        /// Returns the item at the specified index
        /// </summary>
        /// <remarks>
        /// This is an O(n) operation, unlike array-based lists. If you are using this method more than pushing and
        /// shifting, you probably want to use a normal list instead.
        /// </remarks>
        public T Get(int index) => GetItem(index).Value;

        /// <summary>
        /// Sets the value of the item at the specified index
        /// </summary>
        /// <seealso cref="Get">See Get remarks for speed</seealso>
        public void Set(int index, T value) => GetItem(index).Value = value;

        /// <summary>
        /// Returns true when there is at least one item in the list
        /// </summary>
        public bool HasItems() => _first != null;

        /// <summary>
        /// Provides O(n) indexing
        /// </summary>
        [IndexerName("Indexer")]
        public T this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var next = _first;
            while (next != null)
            {
                yield return next.Value;
                next = next.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}