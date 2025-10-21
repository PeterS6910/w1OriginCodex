using System;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class ResizeableArray<T>
    {
        private T[] _array;
        private int _count;
        private readonly int _arrayIncrementationLength;

        public int Count { get { return _count; } }

        public ResizeableArray(int arrayIncrementationLength)
        {
            _arrayIncrementationLength = arrayIncrementationLength;
        }

        public void AddLast(T item)
        {
            CreateOrIncrementArrayLengthIfNeeded();
            _array[_count++] = item;
        }

        private void CreateOrIncrementArrayLengthIfNeeded()
        {
            if (_array == null)
            {
                _array = new T[_arrayIncrementationLength];
                return;
            }

            if (_count == _array.Length)
                Array.Resize(
                    ref _array,
                    _array.Length + _arrayIncrementationLength);
        }

        public void RemoveLast()
        {
            if (_array == null)
                return;

            _count--;
            _array[_count] = default(T);
            DecrementArrayLengthIfNeeded();
        }

        private void DecrementArrayLengthIfNeeded()
        {
            if (_array.Length <= _arrayIncrementationLength)
                return;

            if ((_array.Length/_count) >= 2)
                Array.Resize(
                    ref _array,
                    _count > _arrayIncrementationLength
                        ? _count
                        : _arrayIncrementationLength);
        }

        public void ForEach(Action<T> lambda)
        {
            if (_array == null)
                return;

            for (var i = 0; i < _count; i++)
                lambda(_array[i]);
        }

        public T this[int key]
        {
            get
            {
                if (_array == null || key > _count || key < 0)
                    throw new IndexOutOfRangeException();

                return _array[key];
            }
            set
            {
                if (_array == null || key > _count || key < 0)
                    throw new IndexOutOfRangeException();

                _array[key] = value;
            }
        }

        public void Clear()
        {
            _count = 0;
            _array = null;
        }
    }
}
