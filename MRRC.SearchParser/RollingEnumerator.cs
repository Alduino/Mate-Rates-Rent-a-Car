namespace MRRC.SearchParser
{
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

        public bool Shift(out T result)
        {
            if (_first == null)
            {
                result = default;
                return false;
            }
            
            var value = _first.Value;
            _first = _first.Next;
            result = value;
            return true;
        }

        public void Push(T value)
        {
            var item = new Item(value, null);
            _last.Next = item;
            _last = item;
        }

        public bool HasItems() => _first != null;
    }
}