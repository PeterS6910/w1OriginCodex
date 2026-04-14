using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// child of System.Collections.Generic.Dictionary with
    /// implicit locking over usual operations
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [LwSerialize(0x3FF)]
    [LwSerializeNoParent]
    [LwSerializeMode(LwSerializationMode.SelectiveWithCollectionOptimization, DirectMemberType.Public)]
    public class SyncDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>
        // ReSharper disable once RedundantExtendsListEntry
        , IEnumerable<KeyValuePair<TKey, TValue>>
        , IWaitCollection
        , ICollection<KeyValuePair<TKey, TValue>>
        , ICloneable
    {
        /// <summary>
        /// be careful to not reveal this object to the outside world
        /// </summary>
        protected readonly object _dictionarySync = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capacity"></param>
        public SyncDictionary(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public SyncDictionary()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public new virtual TValue this[[NotNull] TKey key]
        {
            get
            {
                if (ReferenceEquals(key, null))
                    throw new ArgumentNullException("key");

                lock (_dictionarySync)
                {
                    return base[key];
                }

            }
            set { SetValue(key, value, null); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="postprocessingLambda"></param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public virtual void SetValue(
            [NotNull] TKey key,
            [NotNull] TValue newValue,
            [CanBeNull] DKeyValueLambda postprocessingLambda)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                base[key] = newValue;

                CheckWaitingMutexes();

                if (postprocessingLambda != null)
                    try
                    {
                        postprocessingLambda(key, newValue);
                    }
                    catch
                    {

                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueIfKeyDoesntExistLambda"></param>
        /// <param name="getNewValueIfKeyExistsLambda"></param>
        /// <param name="postprocessingLambda"></param>
        /// <returns>false, if the key was already present, true if the key needed to be introduced</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueIfKeyDoesntExistLambda is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueIfKeyExistsLambda is null</exception>
        public virtual bool SetValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda getNewValueIfKeyDoesntExistLambda,
            [NotNull] DGetNewValueIfKeyExistsLambda getNewValueIfKeyExistsLambda,
            [CanBeNull] DKeyValueLambda postprocessingLambda)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(getNewValueIfKeyDoesntExistLambda, null))
                throw new ArgumentNullException("getNewValueIfKeyDoesntExistLambda");

            if (ReferenceEquals(getNewValueIfKeyExistsLambda, null))
                throw new ArgumentNullException("getNewValueIfKeyExistsLambda");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                TValue originalValue;

                TValue newValue = default(TValue);

                bool found = base.TryGetValue(key, out originalValue);

                try
                {
                    _writeOpsRefusedDueLambdaCall = true;

                    if (found)
                    {
                        _lambdaInProgress = getNewValueIfKeyDoesntExistLambda;
                        newValue = getNewValueIfKeyExistsLambda(key, originalValue);
                    }
                    else
                    {
                        _lambdaInProgress = getNewValueIfKeyDoesntExistLambda;
                        newValue = getNewValueIfKeyDoesntExistLambda(key);
                    }


                }
                finally
                {
                    _writeOpsRefusedDueLambdaCall = false;
                    _lambdaInProgress = null;
                }





                // slight optimisation
                if ((found && !ReferenceEquals(originalValue, newValue)) || !found)
                {
                    base[key] = newValue;
                    CheckWaitingMutexes();
                }

                if (postprocessingLambda != null)
                    try
                    {
                        postprocessingLambda(key, newValue);
                    }
                    catch
                    {

                    }

                return !found;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public delegate void DKeyValueLambda(
            [NotNull] TKey key,
            [NotNull] TValue value
            );


        /// <summary>
        /// without synchronisation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void AddWithoutSync(
            [NotNull] TKey key,
            [NotNull] TValue value)
        {
            base.Add(key, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public new virtual void Add(
            [NotNull] TKey key,
            [CanBeNull] TValue value)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                base.Add(key, value);

                CheckWaitingMutexes();
            }

        }


        // write access to both of this members should be done 
        // only within the lock(_dictionarySync) sections
        private volatile bool _writeOpsRefusedDueLambdaCall = false;
        private volatile Delegate _lambdaInProgress;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="preRegistrationLambda">lambda, that will be called BEFORE the addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if preRegistrationLambda is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(
            [NotNull] TKey key,
            [NotNull] TValue value,
            [NotNull] DKeyValueLambda preRegistrationLambda)
        {
            if (ReferenceEquals(preRegistrationLambda, null))
                throw new ArgumentNullException("preRegistrationLambda");

            Add(
                key,
                value,
                preRegistrationLambda,
                null
                );
        }



        private const string ErrorManipulationFromLambda =
            "Only SyncDictionary's Read operations are allowed within the lambda execution .\r\n ManagedThreadId={0}\r\n Delegate={1}\r\n Target={2}";

        /// <summary>
        /// used to be called on all modifying methods right after entering _dictionarySync
        /// </summary>
        private void CheckManipulationFromLambda()
        {
            if (_writeOpsRefusedDueLambdaCall)
                throw new InvalidOperationException(
                    string.Format(
                        ErrorManipulationFromLambda,
                        _writeOpsRefusedDueLambdaCall,
                        _lambdaInProgress,
                        _lambdaInProgress != null ? _lambdaInProgress.Target : null
                        ));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="preRegistrationLambda">key/value lambda, going to be executed BEFORE addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <param name="postRegistrationLambda">key/value lambda, going to be executed AFTER addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(
            [NotNull] TKey key,
            [NotNull] TValue value,
            [CanBeNull] DKeyValueLambda preRegistrationLambda,
            [CanBeNull] DKeyValueLambda postRegistrationLambda)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                if (preRegistrationLambda != null)
                {
                    try
                    {
                        _writeOpsRefusedDueLambdaCall = true;

                        _lambdaInProgress = preRegistrationLambda;
                        preRegistrationLambda(key, value);
                    }
                    finally
                    {
                        _writeOpsRefusedDueLambdaCall = false;
                        _lambdaInProgress = null;
                    }
                }

                // can throw ArgumentException, if the key is already present
                base.Add(key, value);

                CheckWaitingMutexes();

                if (postRegistrationLambda != null)
                    postRegistrationLambda(key, value);
            }
        }

        /// <summary>
        /// BE CAREFUL USING THIS METHOD WHEN PASSING NEWLY CONSTRUCTED OBJECT INTO newValue ; 
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="newValue">newValue, to be paired with key inside dictionary, if the key does not exist</param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public virtual bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] out TValue value,  //warned to use NotNull, but no runtime checks restricting it
            [NotNull] TValue newValue)   //warned to use NotNull, but no runtime checks restricting it
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                TValue tmp;

                if (base.TryGetValue(key, out tmp))
                {
                    value = tmp;
                    return false;
                }

                CheckManipulationFromLambda();

                base.Add(key, newValue);

                CheckWaitingMutexes();

                value = newValue;
                return true;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [NotNull]
        public delegate TValue DGetNewValueLambda([NotNull] TKey key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        [NotNull]
        public delegate TValue DGetNewValueIfKeyExistsLambda(
            [NotNull] TKey key,
            [NotNull] TValue oldValue);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="newlyAdded"></param>
        public delegate void DGetOrAddProcessingLambda(
            [NotNull] TKey key,
            [NotNull] TValue value, //warned to use NotNull, but no runtime checks restricting it
            bool newlyAdded);

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">either old or new value being passed out</param>
        /// <param name="getNewValueLambda">lambda, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="preAdditionLamba">if not null, will be called BEFORE the actual addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueLambda is null</exception>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] out TValue value, // NotNull as result of input TValue NotNull resharper constraint
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [CanBeNull] DKeyValueLambda preAdditionLamba,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(getNewValueLambda, null))
                throw new ArgumentNullException("getNewValueLambda");

            lock (_dictionarySync)
            {
                TValue tmp;

                if (base.TryGetValue(key, out tmp))
                {
                    value = tmp;

                    if (postProcessingLambda != null)
                        postProcessingLambda(key, value, false);

                    return false;
                }

                CheckManipulationFromLambda();

                TValue newValue = default(TValue);

                try
                {
                    _writeOpsRefusedDueLambdaCall = true;
                    _lambdaInProgress = getNewValueLambda;

                    // will try to retrieve the new value
                    newValue = getNewValueLambda(key);

                    if (preAdditionLamba != null)
                    {
                        _lambdaInProgress = preAdditionLamba;
                        preAdditionLamba(key, newValue);
                    }
                }
                finally
                {
                    _writeOpsRefusedDueLambdaCall = false;
                    _lambdaInProgress = null;
                }

                base.Add(key, newValue);

                CheckWaitingMutexes();

                #region alternative defensive evaluation
                /* 
                try 
                {
                    base.Add(key, newValue);
                }
                catch (ArgumentException)
                {
                    // SUBSTANTIATION OF THIS WORKAROUND :
                    // the getNewValueLambda could have had made a SyncDictionary operation (e.g. Add call),
                    // and as it's called within this very same thread (thus lock(_dictionarySync) would made no blocking)
                    //
                    // the Add will produce ArgumentException 

                    if (base.TryGetValue(key, out tmp))
                    {
                        value = tmp;

                        if (!ReferenceEquals(postProcessingLambda, null))
                            postProcessingLambda(key, value, false);

                        return false;
                    }
                    
                    throw;
                }*/
                #endregion

                value = newValue;

                if (postProcessingLambda != null)
                    postProcessingLambda(key, newValue, true);

                return true;

            }
        }

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="getNewValueLambda">lambda will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed
        /// </param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition  within the lock
        /// with the parameter newlyAdded</param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueLambda is null</exception>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] out TValue value, // NotNull as result of input TValue NotNull resharper constraint
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            return GetOrAddValue(
                key,
                out value,
                getNewValueLambda,
                null,
                postProcessingLambda);
        }

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda">lambda, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="preAdditionLamba">if not null, will be called BEFORE the actual addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueLambda is null</exception>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [CanBeNull] DKeyValueLambda preAdditionLamba,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            TValue dummyValue;

            return GetOrAddValue(
                key,
                out dummyValue,
                getNewValueLambda,
                preAdditionLamba,
                postProcessingLambda
                );
        }

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getNewValueLambda">lambda, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if getNewValueLambda is null</exception>
        public bool GetOrAddValue(
            [NotNull] TKey key,
            [NotNull] DGetNewValueLambda getNewValueLambda,
            [CanBeNull] DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            TValue dummyValue;

            return GetOrAddValue(
                key,
                out dummyValue,
                getNewValueLambda,
                null,
                postProcessingLambda
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true, if the key was found and removed</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public new bool Remove([NotNull] TKey key)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                bool ret = base.Remove(key);

                CheckWaitingMutexes();

                return ret;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool Remove(
            [NotNull] TKey key,
            out TValue value)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                TValue tmpValue;

                bool removeResult = false;
                if (base.TryGetValue(key, out tmpValue))
                {
                    removeResult = base.Remove(key);
                    CheckWaitingMutexes();
                }

                value = tmpValue;

                return removeResult;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removePreProcessingLambda">being executed only if key is found</param>
        /// <param name="removePostProcessingLambda"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool Remove(
            [NotNull] TKey key,
            [CanBeNull] DRemovePreProcessingLambda removePreProcessingLambda,
            [CanBeNull] DRemovePostProcessingLambda removePostProcessingLambda)
        {
            return Remove(key, false, removePreProcessingLambda, removePostProcessingLambda);
        }

        /// <summary>
        /// REQUIRED : key, removePostProcessingLambda
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removeIfNoOperationInProgress">if true, and there is other SyncDictionary's method in execution it'll
        /// finish without executing any of the lambdas</param>
        /// <param name="removePreProcessingLambda"></param>
        /// <param name="removePostProcessingLambda"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool Remove(
            [NotNull] TKey key,
            bool removeIfNoOperationInProgress,
            [CanBeNull] DRemovePreProcessingLambda removePreProcessingLambda,
            [CanBeNull] DRemovePostProcessingLambda removePostProcessingLambda)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            bool removeResult = false;

            if (removeIfNoOperationInProgress)
            {
                if (!Monitor.TryEnter(_dictionarySync))
                    return false;
            }
            else
                Monitor.Enter(_dictionarySync);

            try
            {
                CheckManipulationFromLambda();

                TValue tmpValue;

                if (base.TryGetValue(key, out tmpValue))
                {
                    bool removalContinue = true;
                    if (null != removePreProcessingLambda)
                        removePreProcessingLambda(key, tmpValue, out removalContinue);

                    if (removalContinue)
                    {
                        removeResult = base.Remove(key);
                        CheckWaitingMutexes();
                    }
                    else
                        tmpValue = default(TValue);
                }

                if (!ReferenceEquals(removePostProcessingLambda, null))
                    removePostProcessingLambda(key, removeResult, tmpValue);

            }
            finally
            {
                Monitor.Exit(_dictionarySync);
            }

            return removeResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removePostProcessingLambda"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool Remove(
            [NotNull] TKey key,
            [CanBeNull] DRemovePostProcessingLambda removePostProcessingLambda)
        {
            return Remove(key, false, null, removePostProcessingLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public delegate bool DRemoveWhereLambda(
            [NotNull] TKey key,
            [NotNull] TValue value
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereCondition"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveWhere([NotNull] DRemoveWhereLambda whereCondition)
        {
            if (ReferenceEquals(whereCondition, null))
                throw new ArgumentNullException("whereCondition");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                var keysToDelete =
                    new LinkedList<TKey>();

                foreach (var kvPair in this)
                    if (whereCondition(kvPair.Key, kvPair.Value))
                        keysToDelete.AddLast(kvPair.Key);

                foreach (var key in keysToDelete)
                    base.Remove(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear([CanBeNull] Action postProcessingLambda)
        {
            Clear(null, postProcessingLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear(
            [CanBeNull] DKeyValueLambda preProcessingLambda,
            [CanBeNull] Action postProcessingLambda)
        {
            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                if (null != preProcessingLambda)
                    foreach (var kvPair in this)
                        preProcessingLambda(kvPair.Key, kvPair.Value);

                base.Clear();

                CheckWaitingMutexes();

                if (null != postProcessingLambda)
                    postProcessingLambda();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Clear()
        {
            Clear(null, null);
        }

        /// <summary>
        /// tries to retrieve the value from dictionary defined by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public new bool TryGetValue(
            [NotNull] TKey key,
            out TValue value)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                return base.TryGetValue(key, out value);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="found"></param>
        /// <param name="value"></param>
        public delegate void DTryGetProcessingLambda(TKey key, bool found, TValue value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removed"></param>
        /// <param name="removedValue"></param>
        public delegate void DRemovePostProcessingLambda(
            [NotNull] TKey key,
            bool removed,
            TValue removedValue);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="possibleValueForRemove"></param>
        /// <param name="continueInRemove"></param>
        public delegate void DRemovePreProcessingLambda(TKey key, TValue possibleValueForRemove, out bool continueInRemove);

        /// <summary>
        /// tries to retrieve the value from dictionary defined by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="postProcessingLambda">lambda called after value retrieval 
        /// , including key, whether found or not, found value or default(T)</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if postProcessingLambda is null</exception>
        public bool TryGetValue(
            [NotNull] TKey key,
            [NotNull] DTryGetProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (postProcessingLambda == null)
                throw new ArgumentNullException("postProcessingLambda");

            lock (_dictionarySync)
            {
                TValue value;
                bool found = base.TryGetValue(key, out value);

                postProcessingLambda(key, found, value);

                return found;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>true, if foreach cycle is to be finished ; otherwise false</returns>
        public delegate bool DForeachBreakableLambda(
            [NotNull] TKey key,
            [NotNull] TValue value);

        /// <summary>
        /// Executes action for each key-value pair in a synchonized manner <c>UNTIL lambda
        /// returns true</c>
        /// </summary>
        /// <param name="foreachLambda">action to perform on a key-value pair, Ctrl+Alt+Space</param>
        /// <exception cref="ArgumentNullException">if lambda is null</exception>
        public void ForEach([NotNull] DForeachBreakableLambda foreachLambda)
        {
            if (ReferenceEquals(foreachLambda, null))
                throw new ArgumentNullException("lambda");

            lock (_dictionarySync)
                foreach (var keyValuePair in this)
                {
                    if (foreachLambda(keyValuePair.Key, keyValuePair.Value))
                        break;
                }
        }

        /// <summary>
        /// Executes action for each key-value pair in a synchonized manner
        /// </summary>
        /// <param name="lambda">action to perform on a key-value pair, Ctrl+Alt+Space</param>
        /// <exception cref="ArgumentNullException">if lambda is null</exception>
        public void ForEach([NotNull] DKeyValueLambda lambda)
        {
            ForEach(lambda, null);
        }

        /// <summary>
        /// Executes action for each key-value pair in a synchonized manner with postprocessing
        /// </summary>
        /// <param name="lambda">action to perform on a key-value pair, Ctrl+Alt+Space</param>
        /// <param name="postProcessingLambda">action to perform after the operation</param>
        /// <exception cref="ArgumentNullException">if lambda is null</exception>
        public void ForEach(
            [NotNull] DKeyValueLambda lambda,
            Action postProcessingLambda)
        {
            if (ReferenceEquals(lambda, null))
                throw new ArgumentNullException("lambda");

            lock (_dictionarySync)
            {
                foreach (var keyValuePair in this)
                {
                    lambda(keyValuePair.Key, keyValuePair.Value);
                }

                if (!ReferenceEquals(
                    postProcessingLambda,
                    null))
                {
                    postProcessingLambda();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public new int Count
        {
            get
            {
                lock (_dictionarySync)
                {
                    return base.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withinLockLambda"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if withinLockLambda is null</exception>
        public int GetCount([NotNull] Action<int> withinLockLambda)
        {

            if (null == withinLockLambda)
                throw new ArgumentNullException("withinLockLambda");


            lock (_dictionarySync)
            {
                int count = base.Count;

                withinLockLambda(count);

                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool ContainsKey([NotNull] TKey key)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                return base.ContainsKey(key);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public new bool ContainsValue([NotNull] TValue value)
        {
            lock (_dictionarySync)
            {
                return base.ContainsValue(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public IList<TKey> KeysSnapshot
        {
            get { return GetKeysSnapshot(false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNullIfEmpty"></param>
        /// <returns></returns>
        public IList<TKey> GetKeysSnapshot(bool returnNullIfEmpty)
        {
            TKey[] keysArray;

            lock (_dictionarySync)
            {
                int count = Keys.Count;

                if (count == 0 && returnNullIfEmpty)
                    return null;

                keysArray = new TKey[count];

                if (count > 0)
                    Keys.CopyTo(keysArray, 0);
            }

            return keysArray;
        }


        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public IList<TValue> ValuesSnapshot
        {
            get { return GetValuesSnapshot(false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNullIfEmpty"></param>
        /// <returns></returns>
        public IList<TValue> GetValuesSnapshot(bool returnNullIfEmpty)
        {
            TValue[] valuesArray;

            lock (_dictionarySync)
            {
                var count = Values.Count;

                if (count == 0 && returnNullIfEmpty)
                    return null;

                valuesArray = new TValue[count];

                if (count > 0)
                    Values.CopyTo(valuesArray, 0);
            }

            return valuesArray;
        }

        /// <summary>
        /// stores the value snapshot into given array ; if array to small, only the values
        /// that can fit inside will be stored
        /// </summary>
        /// <param name="destinationSnaphot"></param>
        /// <returns></returns>
        public int GetValuesSnapshot(
            [NotNull] TValue[] destinationSnaphot)
        {
            if (ReferenceEquals(destinationSnaphot, null))
                throw new ArgumentNullException("destinationSnapshot");

            int count;
            lock (_dictionarySync)
            {
                count = Values.Count;

                if (count > 0)
                {
                    if (count <= destinationSnaphot.Length)
                        Values.CopyTo(destinationSnaphot, 0);
                    else
                    {
                        int i = 0;
                        foreach (var value in Values)
                        {
                            if (i < destinationSnaphot.Length)
                            {
                                destinationSnaphot[i] = value;
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNullIfEmpty"></param>
        /// <typeparam name="TDestinationValue"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidCastException">if TValue is not assignable to TDestinationValue</exception>
        public IList<TDestinationValue> GetValuesSnapshot<TDestinationValue>(bool returnNullIfEmpty)
        {
            TDestinationValue[] valuesArray;

            lock (_dictionarySync)
            {
                var count = Values.Count;

                if (count == 0 && returnNullIfEmpty)
                    return null;

                valuesArray = new TDestinationValue[count];

                if (count > 0)
                {
                    int i = 0;
                    foreach (var value in Values)
                    {
                        valuesArray[i] = (TDestinationValue)(object)value;
                        i++;
                    }
                }
            }

            return valuesArray;
        }


        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public ICollection<KeyValuePair<TKey, TValue>> PairsSnapshot
        {
            get
            {
                KeyValuePair<TKey, TValue>[] pairsArray;

                lock (_dictionarySync)
                {
                    pairsArray = new KeyValuePair<TKey, TValue>[base.Count];

                    int i = 0;
                    foreach (var keyValuePair in this)
                    {
                        pairsArray[i++] = keyValuePair;
                    }
                }

                return pairsArray;
            }
        }

        [NotNull]
        public ICollection<KeyValuePair<TKey, TValue>> KeyValuePairsSnapshot
        {
            get { return PairsSnapshot; }
        }

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return PairsSnapshot.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return PairsSnapshot.GetEnumerator();
        }

        #endregion

        private volatile ManualResetEvent _mreWaitWhileEmpty = null;
        private volatile ManualResetEvent _mreWaitUntilEmpty = null;

        /// <summary>
        /// must be called from lock(_syncDictionary) sections
        /// </summary>
        private void CheckWaitingMutexes()
        {
            if (null != _mreWaitWhileEmpty)
            {
                if (base.Count > 0)
                    _mreWaitWhileEmpty.Set();
                else
                    _mreWaitWhileEmpty.Reset();
            }

            if (null != _mreWaitUntilEmpty)
            {
                if (base.Count == 0)
                    _mreWaitUntilEmpty.Set();
                else
                    _mreWaitUntilEmpty.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool WaitWhileEmpty(int timeout)
        {
            lock (_dictionarySync)
            {
                if (_mreWaitWhileEmpty == null)
                    _mreWaitWhileEmpty = new ManualResetEvent(base.Count > 0);
            }

            return _mreWaitWhileEmpty.WaitOne(timeout, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitWhileEmpty()
        {
            WaitWhileEmpty(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitUntilEmpty(int timeout)
        {
            lock (_dictionarySync)
            {
                if (_mreWaitUntilEmpty == null)
                    _mreWaitUntilEmpty = new ManualResetEvent(base.Count == 0);
            }

            return _mreWaitUntilEmpty.WaitOne(timeout, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitUntilEmpty()
        {
            WaitUntilEmpty(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            SyncDictionary<TKey, TValue> dst;

            lock (_dictionarySync)
            {
                dst = new SyncDictionary<TKey, TValue>(Count);

                foreach (var keyValuePair in this)
                {
                    dst.AddWithoutSync(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return dst;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="lockingObject"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if lockingObject is null</exception>
        public static IList<TValue> GetValuesSnapshot<TKey, TValue>(
            [NotNull] this Dictionary<TKey, TValue> dictionary,
            [NotNull] object lockingObject)
        {
            // the implementation is intentionally duplicate to SyncDictionary.ValuesSnapshot

            if (ReferenceEquals(lockingObject, null))
                throw new ArgumentNullException("lockingObject");

            if (ReferenceEquals(dictionary, null))
                throw new ArgumentNullException("dictionary");

            if (dictionary is SyncDictionary<TKey, TValue>)
                return (dictionary as SyncDictionary<TKey, TValue>).ValuesSnapshot;

            TValue[] valuesArray;

            lock (lockingObject)
            {
                valuesArray = new TValue[dictionary.Values.Count];

                dictionary.Values.CopyTo(valuesArray, 0);
            }

            return valuesArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockingObject"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if lockingObject is null</exception>
        public static ICollection<TKey> GetKeysSnapshot<TKey, TValue>(
            [NotNull] this Dictionary<TKey, TValue> dictionary,
            [NotNull] object lockingObject)
        {
            // the implementation is intentionally duplicate to SyncDictionary.KeysSnapshot

            if (ReferenceEquals(lockingObject, null))
                throw new ArgumentNullException("lockingObject");

            if (ReferenceEquals(dictionary, null))
                throw new ArgumentNullException("dictionary");

            if (dictionary is SyncDictionary<TKey, TValue>)
                return (dictionary as SyncDictionary<TKey, TValue>).KeysSnapshot;

            TKey[] keysArray;

            lock (lockingObject)
            {
                keysArray = new TKey[dictionary.Keys.Count];
                dictionary.Keys.CopyTo(keysArray, 0);
            }

            return keysArray;
        }

        /// <summary>
        /// TryGetValue variant with casting TValue into TNewValueType
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TNewValueType"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool TryGetValue<TKey, TValue, TNewValueType>(
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            out TNewValueType outValue)
        {
            try
            {
                TValue intermediate;
                var ret = dictionary.TryGetValue(key, out intermediate);
                outValue = (TNewValueType)(object)intermediate;

                return ret;
            }
            catch
            {
                outValue = default(TNewValueType);
                return false;
            }
        }
    }
}
