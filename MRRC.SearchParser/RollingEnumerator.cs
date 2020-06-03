namespace MRRC.SearchParser
{
    /// <summary>
    /// Linked list that allows pushing to the end and shifting out from the start
    /// </summary>
    public class RollingEnumerator<T>
    {
        private class Item
        {
            public Item(T value, Item next)
            {
                Value = value;
                Next = next;
            }

            public T Value { get; }
            public Item Next { get; set; }
        }

        private Item _first;
        private Item _last;

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
        /// Appends an item to the end of the list
        /// </summary>
        /// <param name="value"></param>
        public void Push(T value)
        {
            var item = new Item(value, null);
            _last.Next = item;
            _last = item;
            if (_first == null) _first = item;
            Length++;
        }

        /// <summary>
        /// Returns true when there is at least one item in the list
        /// </summary>
        public bool HasItems() => _first != null;
    }
}