using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// class implementing dictionary via array
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TinyCache<TKey, TValue> : IDictionary<TKey,TValue> 
        where TKey : struct, IConvertible
    {
        private sealed class TinyCacheValue : APoolable<TinyCacheValue>
        {
            internal TKey Key;
            internal TValue Value;

            private TinyCacheValue(AObjectPool<TinyCacheValue> objectPool) : base(objectPool)
            {
            }

            protected override bool FinalizeBeforeReturn()
            {
                Key = default(TKey);
                Value = default(TValue);
                return true;
            }
        }

        private volatile TinyCacheValue[] _values;

        private readonly object _syncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        public TinyCache()
            : this(-1)
        {

        }

        private readonly bool _tkeyIsEnum;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public TinyCache(int capacity)
        {
            var tkeyType = typeof (TKey);

            if (typeof (Enum).IsAssignableFrom(tkeyType))
            {
                _tkeyIsEnum = true;

#if COMPACT_FRAMEWORK
                // saves conversion time later, possibly due internal caching 
                // of reflections in Convert class
                var tmp = ((IConvertible)default(TKey)).ToInt32(null);
                DebugHelper.Keep(tmp);
#endif
            }
            else
                if (!typeof (int).IsAssignableFrom(tkeyType))
                    throw new InvalidOperationException("Key of type " + typeof (TKey) +
                                                        " should be able to be casted to int");

            if (capacity > 0)
                ResizeIfNecessary(capacity);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newSize"></param>
        private void ResizeIfNecessary(int newSize)
        {
            if (_values == null ||
                _values.Length < newSize)
            {
                TinyCacheValue[] newValues = new TinyCacheValue[newSize];

                if (_values != null)
                    Array.Copy(_values, 0, newValues, 0, _values.Length);

                _values = newValues;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int KeyToIndex(TKey key)
        {
            int itemIndex;
            if (_tkeyIsEnum)
                itemIndex = ((IConvertible)key).ToInt32(null);
            else
                itemIndex = (int) (object) key;
            

            if (itemIndex < 0 || itemIndex > ushort.MaxValue)
                throw new ArgumentException(
                    "Only keys with values from interval <0,65535> are accepted. Current value : " + itemIndex);

            return itemIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(TKey key, TValue value)
        {
            int itemIndex = KeyToIndex(key);

            lock (_syncRoot)
            {
                ResizeIfNecessary(itemIndex+1);

                if (_values[itemIndex] == null)
                {
                    var tcv = TinyCacheValue.Get(passed =>
                    {
                        passed.Key = key;
                        passed.Value = value;
                    });

                    _values[itemIndex] = tcv;
                }
                else
                    _values[itemIndex].Value = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool UnsetValue(TKey key)
        {
            int itemIndex = KeyToIndex(key);

            bool found = false;
            lock (_syncRoot)
            {
                if (_values[itemIndex] != null)
                {
                    _values[itemIndex].Return();
                    found = true;
                }

                _values[itemIndex] = null;
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            SetValue(item.Key,item.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot)
            {
                foreach (var tinyCacheValue in _values)
                {
                    tinyCacheValue.Return();
                }

                _values = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(
            [NotNull] KeyValuePair<TKey, TValue>[] array, 
            int arrayIndex)
        {
            Validator.CheckForNull(array,"array");

            int i = 0;

            lock(_syncRoot)
                foreach (var tinyCacheValue in _values)
                {
                    if (tinyCacheValue != null)
                    {
                        if (i + arrayIndex < array.Length)
                        {
                            array[i + arrayIndex] = new KeyValuePair<TKey, TValue>(tinyCacheValue.Key,
                                tinyCacheValue.Value);
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return UnsetValue(item.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _values.Count(value => value != null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="DoesNotExistException"></exception>
        /// <returns></returns>
        public TValue GetValue(TKey key)
        {
            int itemIndex = KeyToIndex(key);

            lock (_syncRoot)
            {
                if (itemIndex >= _values.Length)
                    throw new DoesNotExistException(key);

                var tcv = _values[itemIndex];

                if (tcv == null)
                    throw new DoesNotExistException(key);

                return tcv.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue outValue)
        {
            int itemIndex = KeyToIndex(key);

            lock (_syncRoot)
            {
                if (itemIndex < _values.Length)
                {
                    var tcv = _values[itemIndex];
                    if (tcv != null)
                    {
                        outValue = tcv.Value;
                        return true;
                    }
                }
            }

            outValue = default(TValue);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            int itemIndex = KeyToIndex(key);

            lock (_syncRoot)
            {
                if (itemIndex < _values.Length)
                {
                    var tcv = _values[itemIndex];
                    if (tcv != null)
                        return true;

                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            SetValue(key,value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return UnsetValue(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="DoesNotExistException"></exception>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get { return GetValue(key); }
            set { SetValue(key,value); }
        }

        [PublicAPI]
        public ICollection<TKey> Keys { get; private set; }

        [PublicAPI]
        public ICollection<TValue> Values { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (TinyCacheValue tinyCacheValue in _values)
            {
                if (tinyCacheValue != null)
                    yield return new KeyValuePair<TKey, TValue>(tinyCacheValue.Key, tinyCacheValue.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
