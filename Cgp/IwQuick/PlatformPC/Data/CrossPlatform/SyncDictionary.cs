using System;
using System.Collections.Generic;
using System.Threading;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    /// <summary>
    /// child of System.Collections.Generic.Dictionary with
    /// implicit locking over usual operations
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SyncDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IWaitCollection
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
            :base(capacity)
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
        public new virtual TValue this[TKey key]
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
        public virtual void SetValue(TKey key, TValue newValue, DMediatorLambda postprocessingLambda)
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
        public virtual bool SetValue(
            TKey key, 
            DGetNewValueLambda getNewValueIfKeyDoesntExistLambda,
            DGetNewValueLambda2 getNewValueIfKeyExistsLambda,
            DMediatorLambda postprocessingLambda)
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
                Exception lambdaError = null;

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
                catch (Exception error)
                {
                    lambdaError = error;
                }
                finally
                {
                    _writeOpsRefusedDueLambdaCall = false;
                    _lambdaInProgress = null;
                }



                if (lambdaError != null)
                    throw lambdaError;

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
        /// <param name="state"></param>
        public delegate void DMediatorRoutine(TKey key, TValue value, object state);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public delegate void DMediatorLambda(TKey key, TValue value);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public new virtual void Add(TKey key, TValue value)
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
        private volatile bool  _writeOpsRefusedDueLambdaCall = false;
        private volatile Delegate _lambdaInProgress;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="preRegistrationRoutine">routine, that will be called BEFORE the addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="state"></param>
        /// <exception cref="ArgumentNullException">if preRegistrationRoutine is null</exception>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(TKey key, TValue value, DMediatorRoutine preRegistrationRoutine, object state)
        {
            if (ReferenceEquals(preRegistrationRoutine, null))
                throw new ArgumentNullException("preRegistrationRoutine");

            Add(
                key, 
                value,
                (keyPassed, valuePassed) => preRegistrationRoutine(keyPassed, valuePassed, state),
                null
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="preRegistrationLambda">routine, that will be called BEFORE the addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <exception cref="ArgumentNullException">if preRegistrationRoutine is null</exception>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(TKey key, TValue value, DMediatorLambda preRegistrationLambda)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="preRegistrationRoutine">mediator routine, going to be executed BEFORE addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <param name="stateBeforeRegistration"></param>
        /// <param name="postRegistrationRoutine">mediator routine, going to be executed AFTER addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <param name="stateAfterRegistration"></param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(TKey key, TValue value, 
            DMediatorRoutine preRegistrationRoutine, object stateBeforeRegistration,
            DMediatorRoutine postRegistrationRoutine, object stateAfterRegistration)
        {
            Add(
                key,
                value,
                (keyPassed, valuePassed) => preRegistrationRoutine(keyPassed, valuePassed, stateBeforeRegistration),
                (keyPassed, valuePassed) => postRegistrationRoutine(keyPassed, valuePassed, stateAfterRegistration)
                );
        }

        private const string ERROR_MANIPULATION_FROM_LAMBDA =
            "Only SyncDictionary's Read operations are allowed within the lambda execution .\r\n ManagedThreadId={0}\r\n Delegate={1}\r\n Target={2}";

        private void CheckManipulationFromLambda()
        {
            if (_writeOpsRefusedDueLambdaCall)
                throw new InvalidOperationException(
                    string.Format(
                        ERROR_MANIPULATION_FROM_LAMBDA ,
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
        /// <param name="preRegistrationRoutine">mediator routine, going to be executed BEFORE addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <param name="postRegistrationRoutine">mediator routine, going to be executed AFTER addition of key/value pair ; 
        /// exceptions not being caught ; can be null and then not executed</param>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentException">if key is already in the dictionary</exception>
        public virtual void Add(TKey key, TValue value,
            DMediatorLambda preRegistrationRoutine,
            DMediatorLambda postRegistrationRoutine)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                if (preRegistrationRoutine != null)
                {
                    Exception lambdaError = null;
                    try
                    {
                        _writeOpsRefusedDueLambdaCall = true;

                        _lambdaInProgress = preRegistrationRoutine;
                        preRegistrationRoutine(key, value);
                    }
                    catch (Exception e)
                    {
                        lambdaError = e;
                    }
                    finally
                    {
                        _writeOpsRefusedDueLambdaCall = false;
                        _lambdaInProgress = null;
                    }

                    if (lambdaError != null)
                        throw lambdaError;
                }

                base.Add(key, value);

                CheckWaitingMutexes();

                if (postRegistrationRoutine != null)
                    postRegistrationRoutine(key, value);
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
        public virtual bool GetOrAddValue(TKey key, out TValue value, TValue newValue)
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
                
                base.Add(key,newValue);

                CheckWaitingMutexes();

                value = newValue;
                return true;
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public delegate TValue DGetNewValueRoutine(TKey key,object state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public delegate TValue DGetNewValueLambda(TKey key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public delegate TValue DGetNewValueLambda2(TKey key,TValue oldValue);

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="getNewValueRoutine">routine, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="state"></param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool GetOrAddValue(
            TKey key, 
            out TValue value, 
            DGetNewValueRoutine getNewValueRoutine,
            object state)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(getNewValueRoutine, null))
                throw new ArgumentNullException("getNewValueRoutine");

            return GetOrAddValue(
                key,
                out value,
                keyPassed => (getNewValueRoutine(keyPassed, state)),
                null,
                null
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="newlyAdded"></param>
        public delegate void DGetOrAddProcessingLambda(TKey key, TValue value, bool newlyAdded);

        /// <summary>
        /// gets the element identified by key, or adds it if it does not exist filling with newValue ; 
        /// returns true, if the newValue has been added ; 
        /// returns false, if already in dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">either old or new value being passed out</param>
        /// <param name="getNewValueLambda">routine, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="preAdditionLamba">if not null, will be called BEFORE the actual addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public bool GetOrAddValue(
            TKey key,
            out TValue value, 
            DGetNewValueLambda getNewValueLambda,
            DMediatorLambda preAdditionLamba,
            DGetOrAddProcessingLambda postProcessingLambda
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
                
                Exception lambdaError = null;

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
                catch (Exception e)
                {
                    lambdaError = e;
                }
                finally
                {
                    _writeOpsRefusedDueLambdaCall = false;
                    _lambdaInProgress = null;
                }

                if (lambdaError != null)
                    throw lambdaError;

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
        public bool GetOrAddValue(
            TKey key,
            out TValue value,
            DGetNewValueLambda getNewValueLambda,
            DGetOrAddProcessingLambda postProcessingLambda
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
        /// <param name="getNewValueLambda">routine, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="preAdditionLamba">if not null, will be called BEFORE the actual addition within the lock ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if postProcessingLambda is null, as there is no other way to get the value in this overload</exception>
        public bool GetOrAddValue(
            TKey key,
            DGetNewValueLambda getNewValueLambda,
            DMediatorLambda preAdditionLamba,
            DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            TValue dummyValue;

            if (ReferenceEquals(postProcessingLambda, null))
                throw new ArgumentNullException("postProcessingLambda");

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
        /// <param name="getNewValueLambda">routine, that will be called if the key does not exist, to provide the new value ; 
        /// raising the exception in the lambda will prevent the addition to proceed</param>
        /// <param name="postProcessingLambda">if not null, will be called AFTER the actual addition within the lock 
        /// with the parameter newlyAdded </param>
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        /// <exception cref="ArgumentNullException">if postProcessingLambda is null, as there is no other way to get the value in this overload</exception>
        public bool GetOrAddValue(
            TKey key,
            DGetNewValueLambda getNewValueLambda,
            DGetOrAddProcessingLambda postProcessingLambda
            )
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            TValue dummyValue;

            if (ReferenceEquals(postProcessingLambda, null))
                throw new ArgumentNullException("postProcessingLambda");

            return GetOrAddValue(
                key,
                out dummyValue,
                getNewValueLambda,
                null,
                postProcessingLambda
                );
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
        /// <returns>true, if the newValue has been added ; 
        /// false, if already in dictionary</returns>
        public bool GetOrAddValue(
           TKey key,
           out TValue value,
           DGetNewValueLambda getNewValueLambda
           )
        {
            return GetOrAddValue(key, out value, getNewValueLambda, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool Remove(TKey key)
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
        public bool Remove(TKey key, out TValue value)
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
        /// <param name="removePreProcessingLambda"></param>
        /// <param name="removePostProcessingLambda"></param>
        /// <returns></returns>
        public bool Remove(
            TKey key, 
            DRemovePreProcessingLambda removePreProcessingLambda,
            DRemovePostProcessingLambda removePostProcessingLambda)
        {
            if (ReferenceEquals(key, null))
                throw new ArgumentNullException("key");

            if (ReferenceEquals(removePostProcessingLambda, null))
                throw new ArgumentNullException("removePostProcessingLambda");

            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                TValue tmpValue;
                
                bool removeResult = false;

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

                removePostProcessingLambda(key, removeResult, tmpValue);

                return removeResult;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="removePostProcessingLambda"></param>
        /// <returns></returns>
        public bool Remove(TKey key, DRemovePostProcessingLambda removePostProcessingLambda)
        {
            return Remove(key, null, removePostProcessingLambda);
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Clear()
        {
            lock (_dictionarySync)
            {
                CheckManipulationFromLambda();

                base.Clear();

                CheckWaitingMutexes();
            }
        }

        /// <summary>
        /// tries to retrieve the value from dictionary defined by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if key is null</exception>
        public new bool TryGetValue(TKey key, out TValue value)
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
        public delegate void DRemovePostProcessingLambda(TKey key, bool removed, TValue removedValue);

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
        public bool TryGetValue(TKey key, DTryGetProcessingLambda postProcessingLambda)
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
        /// Executes action for each key-value pair in a synchonized manner
        /// </summary>
        /// <param name="lambda">action to perform on a key-value pair, Ctrl+Alt+Space</param>
        /// <exception cref="ArgumentNullException">if lambda is null</exception>
        public void ForEach(DMediatorLambda lambda)
        {
            if (ReferenceEquals(lambda, null))
                throw new ArgumentNullException("lambda");

            lock (_dictionarySync)
                foreach (var keyValuePair in this)
                    lambda(keyValuePair.Key, keyValuePair.Value);
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
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool ContainsKey(TKey key)
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
        public new bool ContainsValue(TValue value)
        {
            lock (_dictionarySync)
            {
                return base.ContainsValue(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TKey[] KeysSnapshot
        {
            get { return GetKeysSnapshot(false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNullIfEmpty"></param>
        /// <returns></returns>
        public TKey[] GetKeysSnapshot(bool returnNullIfEmpty)
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
        public TValue[] ValuesSnapshot
        {
            get { return GetValuesSnapshot(false); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnNullIfEmpty"></param>
        /// <returns></returns>
        public TValue[] GetValuesSnapshot(bool returnNullIfEmpty)
        {
            TValue[] valuesArray;

            lock (_dictionarySync)
            {
                int count = Values.Count;

                if (count == 0 && returnNullIfEmpty)
                    return null;
                
                valuesArray = new TValue[count];

                if (count > 0)
                    Values.CopyTo(valuesArray, 0);
            }

            return valuesArray;
        }


        /// <summary>
        /// 
        /// </summary>
        public ICollection<KeyValuePair<TKey,TValue>> PairsSnapshot
        {
            get
            {
                KeyValuePair<TKey, TValue>[] valuesArray;

                lock (_dictionarySync)
                {
                    valuesArray = new KeyValuePair<TKey, TValue>[base.Count];

                    int i=0;
                    foreach (KeyValuePair<TKey, TValue> kvp in this)
                    {
                        valuesArray[i++] = kvp;
                    }
                }

                return valuesArray;
            }
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

            return _mreWaitWhileEmpty.WaitOne(timeout,false);
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
        /// <exception cref="ArgumentNullException"></exception>
        public static ICollection<TValue> GetValuesSnapshot<TKey,TValue>(this Dictionary<TKey, TValue> dictionary,object lockingObject)
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
        public static ICollection<TKey> GetKeysSnapshot<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,object lockingObject)
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
    }
}
