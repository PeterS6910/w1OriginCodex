using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// Code-sugar class
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class InstanceParameters<TKey>
    {
        private volatile SyncDictionary<TKey, object> _parameters;
        private readonly object _lockParameters = new object();

        private IDictionary<TKey, object> EnsureParametersDictionary()
        {
            if (_parameters == null)
                lock (_lockParameters)
                {
                    if (_parameters == null)
                        _parameters = new SyncDictionary<TKey, object>();
                }

            return _parameters;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramId"></param>
        /// <param name="value"></param>
        public void Set(
            [NotNull] TKey paramId, 
            [CanBeNull] object value)
        {
            Validator.CheckObjectNull(paramId,"paramId");

            EnsureParametersDictionary()[paramId] = value;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramId"></param>
        /// <exception cref="DoesNotExistException">if the specified parameter is not found</exception>
        /// <returns></returns>
        public object Unset([NotNull] TKey paramId)
        {
            Validator.CheckObjectNull(paramId, "paramId");

            if (ReferenceEquals(_parameters,null))
                throw new DoesNotExistException(paramId);

            object originalValue;

            bool success = _parameters.Remove(paramId, out originalValue);

            if (success)
                return originalValue;

            throw new DoesNotExistException(paramId);
        }

        /// <summary>
        /// only instantiation method for InstanceParameters class instance,
        /// to proceed with double-locking schema
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parametersStorage"></param>
        /// <param name="lockForStorage"></param>
        [NotNull]
        public static InstanceParameters<T> CreateOnDemand<T>(
            ref InstanceParameters<T> parametersStorage,
            [NotNull] object lockForStorage )
        {
            if (ReferenceEquals(null,parametersStorage))
                lock(lockForStorage)
                    if (ReferenceEquals(null,parametersStorage))
                        parametersStorage = new InstanceParameters<T>();

            return parametersStorage;
        }

        /// <summary>
        /// instance only via CreateOnDemand static method
        /// </summary>
        private InstanceParameters()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="paramId"></param>
        /// <exception cref="DoesNotExistException">if the specified parameter is not found</exception>
        /// <returns></returns>
        [CanBeNull]
        public TReturn Unset<TReturn>([NotNull] TKey paramId)
        {
            Validator.CheckObjectNull(paramId,"paramId");

            if (ReferenceEquals(_parameters, null))
                throw new DoesNotExistException(paramId);

            object originalValue;

            bool success = _parameters.Remove(paramId, out originalValue);

            if (success)
                return (TReturn)originalValue;

            throw new DoesNotExistException(paramId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramId"></param>
        /// <returns></returns>
        [CanBeNull]
        public object Get([NotNull] TKey paramId)
        {
            Validator.CheckObjectNull(paramId, "paramId");

            object val;

            if (ReferenceEquals(_parameters, null))
                return null;

            _parameters.TryGetValue(paramId, out val);

            return val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="paramId"></param>
        /// <param name="createIfParameterDoesNotExist">if not null, the parameter is going to be created by lambda</param>
        /// <returns></returns>
        [CanBeNull]
        public TReturn Get<TReturn>(
            [NotNull] TKey paramId,
            [CanBeNull] SyncDictionary<TKey,TReturn>.DGetNewValueLambda createIfParameterDoesNotExist
            )
            where TReturn : class
        {
            Validator.CheckObjectNull(paramId, "paramId");

            object val;



            if (createIfParameterDoesNotExist != null)
            {
                EnsureParametersDictionary();

                // need to use overload with postprocessing , because CLR otherwise favourizes other overload
                // public virtual bool GetOrAddValue(TKey key, out TValue value, TValue newValue), due fact TValue is object
                _parameters.GetOrAddValue(
                    paramId,
                    out val,
                    key => (object)createIfParameterDoesNotExist(key),
                    null);
            }
            else
            {
                if (ReferenceEquals(_parameters, null))
                    return null;

                _parameters.TryGetValue(paramId, out val);
            }

            return val as TReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="paramId"></param>
        /// <param name="preregisterIfNonExisting"></param>
        /// <param name="defaultValueToPreregister"></param>
        /// <exception cref="DoesNotExistException">if preregister is false, and the param is not found</exception>
        /// <returns></returns>
        public TReturn Get<TReturn>(
            [NotNull] TKey paramId,
            bool preregisterIfNonExisting,
            TReturn defaultValueToPreregister)
            where TReturn : struct
        {
            Validator.CheckObjectNull(paramId,"paramId");

            object val;

            if (preregisterIfNonExisting)
            {
                EnsureParametersDictionary();

                // should be safe to use this GetOrAddValue overload, as default(T) will likely be null, or some primitive value
                _parameters.GetOrAddValue(paramId, out val, defaultValueToPreregister);
            }
            else
            {
                if (ReferenceEquals(_parameters, null))
                    throw new DoesNotExistException(paramId);

                if (!_parameters.TryGetValue(paramId, out val))
                    throw new DoesNotExistException(paramId);
            }

// ReSharper disable once PossibleNullReferenceException
            return (TReturn)val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        [CanBeNull]
        public TReturn Get<TReturn>(
            [NotNull] TKey paramName) where TReturn : class
        {
            return Get<TReturn>(paramName, null);
        }

        
    }
}
