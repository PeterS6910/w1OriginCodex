using System;
using System.Collections;
using System.Collections.Generic;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class SimpleSortedList<T> : ICollection<T>
    {
        private class Enumerator : IEnumerator<T>
        {
            private readonly SimpleSortedList<T> _list;

            private T _current;
            private int _idx;

            public Enumerator(SimpleSortedList<T> list)
            {
                _list = list;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_idx < _list._size)
                {
                    _current = _list._items[_idx++];
                    return true;
                }

                _idx = _list._size + 1;
                _current = default (T);

                return false;
            }

            public void Reset()
            {
                _idx = 0;
                _current = default(T);
            }

            public T Current
            {
                get { return _current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private T[] _items;
        private int _size;

        private readonly IComparer<T> _comparer;

        public SimpleSortedList(IComparer<T> comparer, ICollection<T> values)
        {
            _comparer = comparer;

            _size = values.Count;
            _items = new T[_size];

            values.CopyTo(
                _items, 
                0);

            Array.Sort(
                _items, 
                _comparer);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T AddAndGetPredecessor(T item)
        {
            if (_size > _items.Length)
            {
                int newLength = 
                    _items.Length == 0
                        ? 4 
                        : _items.Length * 2;

                if (newLength > 2146435071)
                    newLength = 2146435071;

                if (newLength < _size + 1)
                    newLength = _size + 1;

                var newItems = new T[newLength];
                Array.Copy(_items, 0, newItems, 0, _items.Length);

                _items = newItems;
            }

            int idx = Array.BinarySearch(
                _items,
                0,
                _size,
                item,
                _comparer);

            if (idx < 0)
                idx = ~idx;

            Array.Copy(
                _items, 
                idx, 
                _items, 
                idx + 1, 
                _size - idx);

            _items[idx] = item;
            ++_size;

            return
                --idx >= 0
                    ? _items[idx]
                    : default(T);
        }

        public void Add(T item)
        {
            AddAndGetPredecessor(item);
        }

        public void Clear()
        {
            _size = 0;
        }

        public bool Contains(T item)
        {
            var idx = 
                Array.BinarySearch(
                    _items,
                    0,
                    _size,
                    item,
                    _comparer);

            if (idx >= 0)
                do
                {
                    if (_items[idx].Equals(item))
                        return true;

                    ++idx;
                }
                while (idx >= _size || _comparer.Compare(_items[idx], item) != 0);

            return false;
        }

        public void CopyTo(
            T[] array,
            int arrayIndex)
        {
            if (array == null)
                throw new ArgumentException();

            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        public bool Remove(T item)
        {
            int idx = Array.BinarySearch(
                _items,
                0, 
                _size,
                item,
                _comparer);

            if (idx < 0)
                return false;

            do
            {
                if (_items[idx].Equals(item))
                {
                    Array.Copy(
                        _items,
                        idx + 1,
                        _items,
                        idx,
                        --_size - idx);

                    return true;
                }

                ++idx;
            }
            while (idx >= _size || _comparer.Compare(_items[idx], item) != 0);

            return false;
        }

        public int Count
        {
            get { return _size; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
