using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using System.Diagnostics;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TimeoutDictionary<TKey, TValue>:  IDictionary<TKey, TValue>
    {
        private readonly SyncDictionary<TKey, TDValueEncapsulation> _syncDictionary 
            = new SyncDictionary<TKey,TDValueEncapsulation>();


        /// <summary>
        /// 
        /// </summary>
        private class TDValueEncapsulation
        {
            internal TKey _key;
            internal TValue _value;
            internal int _timeout;
            internal Timer _timer;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
#pragma warning disable 659
            public override bool Equals(object obj)
#pragma warning restore 659
            {
                if (ReferenceEquals(obj, null))
                    return ReferenceEquals(_value,null);

                if (ReferenceEquals(obj, this))
                    return true;

                if (obj is TValue)
                {
                    // don't do other-way-round _value.Equals(obj), cause _value can be null
                    return obj.Equals(_value);
                }

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultTimeout"></param>
        /// <exception cref="ArgumentException"></exception>
        public TimeoutDictionary(int defaultTimeout)
        {
            if (defaultTimeout <= 0)
                throw new ArgumentException("Default timeout must be higher than 0");

            _defaultTimeout = defaultTimeout;
        }


        private volatile int _defaultTimeout;

        /// <summary>
        /// default timeout of the items added without specific timeout
        /// </summary>
        public int DefaultTimeout
        {
            get { return _defaultTimeout; }
            set
            {
                if (value <= 0)
                    value = -1;

                _defaultTimeout = value;
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        public void Add(TKey key, TValue value,int timeout)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            var ve = new TDValueEncapsulation {_key = key, _value = value};

            if (timeout < 0)
                ve._timeout = -1;
            else
                ve._timeout = timeout;


            _syncDictionary.Add(
                key, ve,
                null,
                (keyPassed,valuePassed) =>
                {
                    if (timeout > 0)
                    {
                        PlanItemTimeout(ve);
                    }
                }
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        public bool SetValue(
            TKey key, 
            TValue value
            )
        {
            return SetValue(key, value, _defaultTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        public bool SetValue(
            TKey key, 
            TValue value, 
            int timeout
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (timeout < -1)
                timeout = -1;

            return _syncDictionary.SetValue(
                key,
                keyPassed =>
                {
                    var ve = new TDValueEncapsulation()
                    {
                        _key = key,
                        _value = value,
                        _timeout = timeout
                    };

                    PlanItemTimeout(ve);

                    return ve;
                },
                (keyPassed, oldValuePassed) =>
                {
                    oldValuePassed._value = value;
                    oldValuePassed._timeout = timeout;

                    PlanItemTimeout(oldValuePassed);

                    return oldValuePassed;
                }
                , null

                );
        }

        /// <summary>
        /// SHOULD BE CALLED WITHING SYNCHRONIZED SECTION !!!
        /// </summary>
        /// <param name="valueEncapsulation"></param>
        private void PlanItemTimeout(TDValueEncapsulation valueEncapsulation)
        {
            if (ReferenceEquals(valueEncapsulation, null))
                return;

            if (valueEncapsulation._timer == null)
                valueEncapsulation._timer =
                    Threads.TimerPool.Implicit.GetTimer(OnItemTimeout, valueEncapsulation, valueEncapsulation._timeout, -1);
                //new Timer(OnItemTimeout, valueEncapsulation, valueEncapsulation._timeout,-1);
            else
                valueEncapsulation._timer.Change(valueEncapsulation._timeout, -1);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void OnItemTimeout(object state)
        {
            var ve = state as TDValueEncapsulation;
            if (ve == null)
                return;

            TDValueEncapsulation actualVe = null;

            if (_syncDictionary.Remove(
                ve._key,
                (key, removed, removedValue) =>
                {
                    actualVe = removedValue;

                    if (removed)
                    {
                        ReturnTimer(ve);
                    }
                }                
                ))
            {
                // this should be called outside of the lock
                if (ItemTimedOut != null &&
                    actualVe != null)
                    try
                    {
                        ItemTimedOut(actualVe._key, actualVe._value, actualVe._timeout);
                    }
                    catch
                    {

                    }
            }

        }

        private void ReturnTimer(TDValueEncapsulation valueEncapsulation)
        {
            if (valueEncapsulation._timer != null)
            {
                Threads.TimerPool.Implicit.Return(valueEncapsulation._timer);
                valueEncapsulation._timer = null;
            }
            else
                // should not happen
                Debugger.Break();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        public delegate void DItemTimedOut(TKey key, TValue value, int timeout);

        /// <summary>
        /// 
        /// </summary>
        public event DItemTimedOut ItemTimedOut;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            Add(key, value, _defaultTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public TValue this[TKey key]
        {
            get
            {
                if (ReferenceEquals(key, null))
                    throw new ArgumentNullException("key");

                TDValueEncapsulation vo;

                if (_syncDictionary.TryGetValue(key, out vo))
                    return vo._value;

                throw new KeyNotFoundException(string.Format("Key {0} was not found", key));
            }
            set
            {
                if (ReferenceEquals(key, null))
                    throw new ArgumentNullException("key");

                SetValue(key, value, _defaultTimeout);
            }
        }

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _syncDictionary.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _syncDictionary.KeysSnapshot; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return _syncDictionary.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            TDValueEncapsulation ve;

            if (_syncDictionary.TryGetValue(key, out ve))
            {
                value = ve._value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                ICollection<TDValueEncapsulation> veS = _syncDictionary.ValuesSnapshot;

                return veS.Select(valueEncapsulation => valueEncapsulation._value).ToArray();
            }
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value, _defaultTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _syncDictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TDValueEncapsulation valueEncapsulation;
            if (_syncDictionary.TryGetValue(item.Key, out valueEncapsulation))
            {
                return
                    ReferenceEquals(valueEncapsulation._value, item.Value) ||
                    (!ReferenceEquals(valueEncapsulation._value,null) && valueEncapsulation._value.Equals(item.Value));
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _syncDictionary.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }





        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _syncDictionary.Remove(
                item.Key,
                (TKey key, TDValueEncapsulation valueToRemove, out bool continueInRemove) => 
                {
                    continueInRemove = 
                        ReferenceEquals(item.Value,valueToRemove._value) ||
                        (!ReferenceEquals(item.Value,null) && item.Value.Equals(valueToRemove._value));
                },
                (key, removed, removedValue) =>
                {
                    if (removed && !ReferenceEquals(removedValue, null))
                    {
                        ReturnTimer(removedValue);
                    }
                }


                );
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
