using System;
using System.Collections;
using System.Collections.Generic;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedCollection<T> : ICollection<T>
    {
        private class Enumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _sourceCollectionEnumerator;
            private readonly int _size;
            private int _currentPosition;

            public Enumerator(
                IEnumerator<T> sourceCollectionEnumerator,
                int size)
            {
                _sourceCollectionEnumerator = sourceCollectionEnumerator;
                _size = size;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentPosition < _size)
                {
                    _currentPosition++;
                    _sourceCollectionEnumerator.MoveNext();

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _currentPosition = 0;
                _sourceCollectionEnumerator.Reset();
            }

            public T Current
            {
                get
                {
                    return _sourceCollectionEnumerator.Current;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private readonly ICollection<T> _sourceCollection;
        private readonly int _maxSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceCollection"></param>
        /// <param name="maxSize"></param>
        public LimitedCollection(
            ICollection<T> sourceCollection,
            int maxSize)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");

            _sourceCollection = sourceCollection;
            _maxSize = maxSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(
                _sourceCollection.GetEnumerator(),
                Count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Equals(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                array[arrayIndex] = enumerator.Current;
                arrayIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                if (_maxSize < 0)
                    return _sourceCollection.Count;

                return Math.Min(
                    _maxSize,
                    _sourceCollection.Count);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly { get { return true; } }
    }
}
