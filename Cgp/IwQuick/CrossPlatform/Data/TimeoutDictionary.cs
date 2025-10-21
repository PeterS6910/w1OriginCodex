using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;
using Contal.IwQuick.Threads;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [LwSerialize(1021)]
    [LwSerializeMode(LwSerializationMode.SelectiveWithCollectionOptimization, DirectMemberType.Public)]
    public class TimeoutDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly SyncDictionary<TKey, TimeoutDictionaryKVTCarrier> _syncDictionary
            = new SyncDictionary<TKey, TimeoutDictionaryKVTCarrier>();


        /// <summary>
        /// 
        /// </summary>
        [LwSerialize(1022)]
        [LwSerializeMode(LwSerializationMode.Selective)]
        private class TimeoutDictionaryKVTCarrier : APoolable<TimeoutDictionaryKVTCarrier>
        {
            [LwSerialize]
            private TKey _key;
            internal TKey Key { get { return _key; }}

            

            [LwSerialize]
            internal TValue _value;
            
            [LwSerialize]
            internal int _timeout;

            internal TimerPool.TimerParams _timerParams;

            internal volatile int _syncItemTimeoutMarking = 0;

            private TimeoutDictionaryKVTCarrier(IObjectPool objectPool) : base(objectPool)
            {
            }

            [Obsolete("Use parametric static Get variant", true)]
// ReSharper disable once UnusedMember.Local
            internal new static TimeoutDictionaryKVTCarrier Get()
            {
                throw new InvalidOperationException("Use parametric static Get variant");
            }

            [Obsolete("Use parametric static Get variant", true)]
            // ReSharper disable once UnusedMember.Local
            // ReSharper disable once UnusedParameter.Local
            internal new static TimeoutDictionaryKVTCarrier Get(Action<TimeoutDictionaryKVTCarrier> initializationLambda)
            {
                throw new InvalidOperationException("Use parametric static Get variant");
            }

            public static TimeoutDictionaryKVTCarrier Get(TKey key, TValue value, int timeout)
            {
                var valueCarrier = APoolable<TimeoutDictionaryKVTCarrier>.Get();
                valueCarrier._key = key;
                valueCarrier._value = value;
                valueCarrier._timeout = timeout < Timeout.Infinite ? Timeout.Infinite : timeout;

                return valueCarrier;
            }

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
                    return ReferenceEquals(_value, null);

                if (ReferenceEquals(obj, this))
                    return true;

                if (obj is TValue)
                {
                    // don't do other-way-round _value.Equals(obj), cause _value can be null
                    return obj.Equals(_value);
                }

                return false;
            }

            public override int GetHashCode()
            {
// ReSharper disable once NonReadonlyFieldInGetHashCode
                // not expected to be changed by TimeoutDictionary after instantiation
                return _key.GetHashCode();
            }

            internal void ReturnTimerParams()
            {

                // to ensure atomicity of the _timerParams==null comparison outside of SyncDictionary's locking sections
                var formerTimerParams = Interlocked.Exchange(ref _timerParams, null);
                if (formerTimerParams != null)
                {
                    // also stops the timer
                    TimerPool.Implicit.Return(formerTimerParams);
                }

            }

            protected override bool FinalizeBeforeReturn()
            {
                ReturnTimerParams();

                _key = default(TKey);
                _value = default(TValue);

                return true;

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

        /// <summary>
        /// Constructor used for serialization. Default timeout is 1000 ms
        /// </summary>
        public TimeoutDictionary()
        {
            _defaultTimeout = 1000;
        }            

        [LwSerialize]
        private volatile int _defaultTimeout;

        /// <summary>
        /// default timeout of the items added without specific timeout
        /// </summary>
        public int DefaultTimeout
        {
            get { return _defaultTimeout; }
            set
            {
                if (value < Timeout.Infinite)
                    value = Timeout.Infinite;

                _defaultTimeout = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout">if less than 0, considered infinite</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(
            [NotNull] TKey key,
            TValue value,
            int timeout)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            var kvtCarrier = TimeoutDictionaryKVTCarrier.Get(key, value, timeout);


            _syncDictionary.Add(
                key, kvtCarrier,
                null,
                (keyPassed, valuePassed) =>
                {
                    if (timeout > 0) // this condition can be here, as
                        // if the item would be present already
                        // it will produce ArgumentException
                        // thus stopping the old timer with 0/-1 should not be necessary
                    {
                        PlanItemTimeout(kvtCarrier);
                    }
                }
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(
            [NotNull] TKey key,
            TValue value,
            TimeSpan timeout)
        {
            Add(key, value, (int) timeout.TotalMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        public bool SetValue(
            [NotNull] TKey key,
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
        /// <param name="timeout">if less than 0, considered infinite</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        public bool SetValue(
            [NotNull] TKey key,
            TValue value,
            int timeout
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (timeout < Timeout.Infinite)
                timeout = Timeout.Infinite;

            bool keyIntroduced = _syncDictionary.SetValue(
                key,
                keyPassed =>
                {
                    var kvtCarrier = TimeoutDictionaryKVTCarrier.Get(key, value, timeout);
                    

                    PlanItemTimeout(kvtCarrier);

                    return kvtCarrier;
                },
                (keyPassed, oldValuePassed) =>
                {
                    // if the OnItemTimeout is already in progress
                    // however pending on _syncDictionary's Remove operation
                    //
                    // mark that this operation was switched-in by incrementing the value
#pragma warning disable 420
                    Interlocked.Increment(ref oldValuePassed._syncItemTimeoutMarking);
#pragma warning restore 420

                    oldValuePassed._value = value;
                    oldValuePassed._timeout = timeout;

                    PlanItemTimeout(oldValuePassed);

                    return oldValuePassed;
                }
                , null

                );

            return keyIntroduced;
        }

        /// <summary>
        /// SetValue lambda variant, that tries to renew the timeout either way
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda"></param>
        /// <param name="modifyExistingValueLambda"></param>
        /// <returns></returns>
        public bool SetValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [NotNull] DModifyExistingLambda modifyExistingValueLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            Validator.CheckForNull(getNewValueLambda,"getNewValueLambda");

            Validator.CheckForNull(modifyExistingValueLambda,"modifyExistingValueLambda");

            bool keyIntroduced = _syncDictionary.SetValue(
                key,
                keyPassed =>
                {
                    TValue value;
                    int timeout;

                    getNewValueLambda(key, out value, out timeout);

                    if (timeout <= Timeout.Infinite)
                        timeout = Timeout.Infinite;

                    var kvtCarrier = TimeoutDictionaryKVTCarrier.Get(key, value, timeout);

                    PlanItemTimeout(kvtCarrier);

                    return kvtCarrier;
                },
                (keyPassed, oldValuePassed) =>
                {
                    // if the OnItemTimeout is already in progress
                    // however pending on _syncDictionary's Remove operation
                    //
                    // mark that this operation was switched-in by incrementing the value
#pragma warning disable 420
                    Interlocked.Increment(ref oldValuePassed._syncItemTimeoutMarking);
#pragma warning restore 420

                    modifyExistingValueLambda(key, ref oldValuePassed._value, ref oldValuePassed._timeout);

                    PlanItemTimeout(oldValuePassed);

                    return oldValuePassed;
                }
                , null

                );

            return keyIntroduced;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        public bool SetValue(
            [NotNull] TKey key,
            TValue value,
            TimeSpan timeout
            )
        {
            return SetValue(key, value, (int) timeout.TotalMilliseconds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="timeout"></param>
        public delegate void DGetNewValueLambda(
            [NotNull] TKey key, 
            out TValue newValue, 
            out int timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="timeout"></param>
        public delegate void DGetNewValueLambda2(
           [NotNull] TKey key,
           out TValue newValue,
           out TimeSpan timeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <param name="newlyAdded"></param>
        public delegate void DGetOrAddProcessingLambda(
            [NotNull] TKey key, 
            [CanBeNull] TValue value, 
            int timeout,
            bool newlyAdded
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        public delegate void DModifyExistingLambda(
            [NotNull] TKey key,
            ref TValue value,
            ref int timeout
            
            );


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda"></param>
        /// <param name="getNewValueLambda2"></param>
        /// <param name="postProcessingLambda"></param>
        /// <returns></returns>
        private bool GetOrAddValue(
            [NotNull] TKey key,
            DGetNewValueLambda getNewValueLambda,
            DGetNewValueLambda2 getNewValueLambda2,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {

            SyncDictionary<TKey,TimeoutDictionaryKVTCarrier>.DGetOrAddProcessingLambda postProcLocal = null;

            if (postProcessingLambda != null)
                postProcLocal =
                    (keyPassed, valuePassed, newlyAdded) =>
                        postProcessingLambda(keyPassed, valuePassed._value, valuePassed._timeout, newlyAdded);



            return _syncDictionary.GetOrAddValue(
                key,
                keyPassed =>
                {
                    TValue newValue;
                    int timeout;
                    if (null != getNewValueLambda)
                        getNewValueLambda(keyPassed, out newValue, out timeout);
                    else
                    {
                        TimeSpan ts;
                        if (getNewValueLambda2 != null)
                            getNewValueLambda2(keyPassed, out newValue, out ts);
                        else
                        {
                            
                            throw new ArgumentException("No getNewValueLambda present");
                        }

                        timeout = (int)ts.TotalMilliseconds;
                    }

                    if (timeout < Timeout.Infinite)
                        timeout = Timeout.Infinite;

                    var kvtCarrier = TimeoutDictionaryKVTCarrier.Get(key, newValue, timeout);

                    PlanItemTimeout(kvtCarrier);

                    return kvtCarrier;
                },
                postProcLocal

                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda"></param>
        /// <param name="postProcessingLambda"></param>
        /// <returns></returns>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(getNewValueLambda, null))
                throw new ArgumentNullException("getNewValueLambda");

            return GetOrAddValue(key, getNewValueLambda, null, postProcessingLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda2"></param>
        /// <param name="postProcessingLambda"></param>
        /// <returns></returns>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda2 getNewValueLambda2,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(getNewValueLambda2, null))
                throw new ArgumentNullException("getNewValueLambda2");

            return GetOrAddValue(key, null, getNewValueLambda2, postProcessingLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout">if less than 0, considered infinite</param>
        /// <returns>true, if key was found</returns>
        /// <exception cref="ArgumentNullException">if key was null</exception>
        public bool SetTimeout([NotNull] TKey key, int timeout)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (timeout < Timeout.Infinite)
                timeout = Timeout.Infinite;

            bool success = false;

            _syncDictionary.TryGetValue(key,
                (keyPassed, found, valuePassed) =>
                {
                    if (found)
                    {
                        valuePassed._timeout = timeout;
                        PlanItemTimeout(valuePassed);

                        success = true;
                    }
                });

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout"></param>
        /// <returns>true, if key was found</returns>
        /// <exception cref="ArgumentNullException">if key was null</exception>
        public bool SetTimeout([NotNull] TKey key, TimeSpan timeout)
        {
            return SetTimeout(key,(int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// SHOULD BE CALLED WITHING SYNCHRONIZED SECTION !!!
        /// </summary>
        /// <param name="kvtCarrier"></param>
        private void PlanItemTimeout(TimeoutDictionaryKVTCarrier kvtCarrier)
        {
            if (ReferenceEquals(kvtCarrier, null))
                return;

            if (kvtCarrier._timerParams == null)
            {
                if (kvtCarrier._timeout > 0)
                    kvtCarrier._timerParams =
                        TimerPool.Implicit.GetTimer(
                            OnItemTimeout, 
                            kvtCarrier,
                            kvtCarrier._timeout,
                            Timeout.Infinite);
                    //new Timer(OnItemTimeout, kvtCarrier, kvtCarrier._timeout,Timeout.Infinite);
            }
            else
                kvtCarrier._timerParams.Timer.Change(
                    kvtCarrier._timeout,
                    Timeout.Infinite);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void OnItemTimeout(object state)
        {
            var valueCarrierFromState = state as TimeoutDictionaryKVTCarrier;
            if (valueCarrierFromState == null)
                return;

#pragma warning disable 420
            int snapshot = Interlocked.Increment(ref valueCarrierFromState._syncItemTimeoutMarking);
#pragma warning restore 420

            TimeoutDictionaryKVTCarrier removedKvtCarrier = null;
            

            if (_syncDictionary.Remove(
                valueCarrierFromState.Key,
                (TKey key, TimeoutDictionaryKVTCarrier remove, out bool continueInRemove) =>
                {
                    // if ve._syncItemTimeoutMarking wasn't incremented, 
                    // it means that no SetValue operation was in progress
                    //
                    // thus the removal by timeout can proceed
                    continueInRemove = (snapshot == valueCarrierFromState._syncItemTimeoutMarking);

#if DEBUG
                    if (!continueInRemove)
                        DebugHelper.NOP(snapshot, valueCarrierFromState._syncItemTimeoutMarking);
#endif
                },
                (key, wasRemoved, removedInstance) =>
                {
                    // lambda enclosure failsafe
                    removedKvtCarrier = removedInstance;

                    if (wasRemoved)
                    {
                        // stop the timeout and return timer here, due synchronized nature of this section
                        //
                        // later the returning of the removedValue._timerParams would not be atomic
                        // however , disposal of the values/references behind the removedValue will be done later
                        removedKvtCarrier.ReturnTimerParams();
                    }
                }                
                ))
            {
                // this should be called outside of the lock
                if (removedKvtCarrier != null)
                    try
                    {
                        if (ItemTimedOut != null)
                            ItemTimedOut(
                                removedKvtCarrier.Key, 
                                removedKvtCarrier._value, 
                                removedKvtCarrier._timeout);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        removedKvtCarrier.TryReturn();
                    }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        public delegate void DItemTimedOut([NotNull] TKey key, TValue value, int timeout);

        /// <summary>
        /// 
        /// </summary>
        public event DItemTimedOut ItemTimedOut;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(
            [NotNull] TKey key, 
            TValue value
            )
        {
            Add(key, value, _defaultTimeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                if (ReferenceEquals(key, null))
                    throw new ArgumentNullException("key");

                TimeoutDictionaryKVTCarrier vo;

                if (_syncDictionary.TryGetValue(key, out vo))
                    if (!ReferenceEquals(vo, null))
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
        public bool ContainsKey([NotNull] TKey key)
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
        public bool Remove([NotNull] TKey key)
        {
            return Remove(key, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="postProcessingLambda"></param>
        /// <returns></returns>
        public bool Remove(
            [NotNull] TKey key, 
            [CanBeNull] SyncDictionary<TKey, TValue>.DRemovePostProcessingLambda postProcessingLambda
            )
        {
            return _syncDictionary.Remove(
                key,
                (keyPassed, wasRemoved, kvtCarrier) =>
                {
                    TValue valueSnapshot = default(TValue);
                    try
                    {
                        if (wasRemoved)
                        {
                            if (kvtCarrier != null)
                                valueSnapshot = kvtCarrier._value;
                            kvtCarrier.TryReturn();
                        }
                    }
                    catch(Exception e)
                    {
                        Sys.HandledExceptionAdapter.Examine(e);
                    }

                    if (null != postProcessingLambda)
                        try
                        {
                            postProcessingLambda(
                                keyPassed, 
                                wasRemoved, 
                                valueSnapshot);
                        }
                        catch(Exception e)
                        {
                            Sys.HandledExceptionAdapter.Examine(e);
                        }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            TimeoutDictionaryKVTCarrier ve;

            if (_syncDictionary.TryGetValue(key, out ve))
            {
                if (!ReferenceEquals(ve, null))
                {
                    value = ve._value;
                    return true;
                }
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
                ICollection<TimeoutDictionaryKVTCarrier> veS = _syncDictionary.ValuesSnapshot;

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
            _syncDictionary.Clear((key, value) => value.TryReturn(),null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TimeoutDictionaryKVTCarrier kvtCarrier;
            if (_syncDictionary.TryGetValue(item.Key, out kvtCarrier))
            {
                if (!ReferenceEquals(kvtCarrier, null))
                    return
                        ReferenceEquals(kvtCarrier._value, item.Value) ||
                        (!ReferenceEquals(kvtCarrier._value,null) && kvtCarrier._value.Equals(item.Value));
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
                (TKey key, TimeoutDictionaryKVTCarrier valueToRemove, out bool continueInRemove) => 
                {
                    continueInRemove = 
                        ReferenceEquals(item.Value,valueToRemove._value) ||
                        (!ReferenceEquals(item.Value,null) && item.Value.Equals(valueToRemove._value));
                },
                (key, wasRemoved, removedInstance) =>
                {
                    if (wasRemoved)
                    {
                        removedInstance.TryReturn();
                    }
                }


                );
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        private IEnumerator<KeyValuePair<TKey, TValue>> CreateYieldEnumerator()
        {
            var snapshot = _syncDictionary.PairsSnapshot;

            foreach (var keyValuePair in snapshot)
            {
                var projectedKeyValuePair = new KeyValuePair<TKey, TValue>(
                    keyValuePair.Key,
                    keyValuePair.Value._value
                    );

                yield return projectedKeyValuePair;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return CreateYieldEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return CreateYieldEnumerator();
        }

        #endregion
    }
}
