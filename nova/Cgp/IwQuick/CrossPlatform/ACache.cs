using System.Collections.Generic;
using System.Linq;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ACache<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;
        private readonly HashSet<TKey> _modifiedKeys;

        /// <summary>
        /// 
        /// </summary>
        protected ACache()
            : this (-1)
        {
        }

        protected ACache(int timeout)
        {
            _dictionary = new Dictionary<TKey, TValue>();

            if (timeout >= 0)
            {
                _modifiedKeys = new HashSet<TKey>();

                TimerManager.Static.StartTimer(
                    timeout,
                    false,
                    OnTimerEvent);
            }
        }

        private bool OnTimerEvent(TimerCarrier timerCarrier)
        {
            lock (_dictionary)
            {
                var keysToRemove = new LinkedList<TKey>(
                    _dictionary.Keys
                        .Where(
                            key =>
                                !_modifiedKeys.Contains(key)));


                foreach (var key in keysToRemove)
                {
                    _dictionary.Remove(key);
                }

                _modifiedKeys.Clear();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialValues"></param>
        protected ACache(IEnumerable<KeyValuePair<TKey, TValue>> initialValues) :
            this()
        {
            foreach (var kvPair in initialValues)
                _dictionary.Add(kvPair);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equalitsComparer"></param>
        protected ACache(IEqualityComparer<TKey> equalitsComparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(equalitsComparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetValue(TKey key)
        {
            TValue result;

            lock (_dictionary)
            {
                if (!_dictionary.TryGetValue(key, out result))
                {
                    result = CreateValue(key);

                    _dictionary.Add(key, result);
                }

                if (_modifiedKeys != null)
                    _modifiedKeys.Add(key);
            }

            return result;
        }

        public void SetValue(TKey key, TValue value)
        {
            lock (_dictionary)
            {
                _dictionary[key] = value;

                if (_modifiedKeys != null)
                    _modifiedKeys.Add(key);
            }
        }

        protected abstract TValue CreateValue(TKey key);
    }
}
