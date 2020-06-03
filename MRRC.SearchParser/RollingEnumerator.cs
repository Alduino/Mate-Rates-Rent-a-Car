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

        public int Length { get; private set; }

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

        public void Push(T value)
        {
            var item = new Item(value, null);
            _last.Next = item;
            _last = item;
            if (_first == null) _first = item;
            Length++;
        }

        public bool HasItems() => _first != null;
    }
}