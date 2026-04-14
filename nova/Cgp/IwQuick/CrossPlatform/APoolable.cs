using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDefaultObjectPool
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class PoolableException : Exception
    {
        public PoolableException(string message)
            :base(message)
        {
            
        }

        public PoolableException(string message,Exception innerException)
            : base(message,innerException)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPoolable"></typeparam>
    internal class DefaultObjectPool<TPoolable> : AObjectPool<TPoolable>, IDefaultObjectPool
        where TPoolable:APoolable<TPoolable>
    {
        private readonly ConstructorInfo _tpoolableConstructorInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override TPoolable CreateObject()
        {
            return (TPoolable)_tpoolableConstructorInfo.Invoke(new object[] {this});

            //return new TPoolable();
        }

        private static readonly DefaultObjectPool<TPoolable> _singleton = new DefaultObjectPool<TPoolable>();
        
        /// <summary>
        /// 
        /// </summary>
        private DefaultObjectPool()
        {
            Type t = typeof(TPoolable);

            _tpoolableConstructorInfo = t.GetConstructor(
                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
#if !COMPACT_FRAMEWORK
                CallingConventions.Any,
#endif
                new[] { typeof(AObjectPool<TPoolable>) },
                null
                );

            if (_tpoolableConstructorInfo == null)
                throw new InvalidOperationException("Unable to find "+t+"(IDefaultObjectPool ...) constructor");

        } 

        /// <summary>
        /// 
        /// </summary>
        public static DefaultObjectPool<TPoolable> Singleton
        {
            get { return _singleton; }
        }

#if COMPACT_FRAMEWORK
        private const int MinimalReductionThreshold = 100;
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnedObject"></param>
        public override void Return(TPoolable returnedObject)
        {
#if COMPACT_FRAMEWORK
            if (Count > MinimalReductionThreshold &&
                Count > ASafeThreadBase.ThreadCount)
                // ignore the return, let the GC to discard the object
                return;
#endif

            base.Return(returnedObject);
        }

        private string _cachedToString;
        public override string ToString()
        {
            return
                _cachedToString
                ?? (_cachedToString = "APoolable<" + typeof(TPoolable).FullName + ">");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// 
        /// </summary>
        void Return();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allowReturnCancellation"></param>
        bool Return(bool allowReturnCancellation);

        /// <summary>
        /// 
        /// </summary>
        bool IsInPool { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public abstract class APoolable<T> : IPoolable where T : APoolable<T>
    {
        protected readonly IObjectPool ParentObjectPool;

        private const int MarkInPool = 1;
        private const int MarkNotInPool = 0;

        // starting value should be 1, as there's validation condition following create
        private int _inPool = MarkInPool;
        /// <summary>
        /// 
        /// </summary>
        protected bool InPool { get { return _inPool == MarkInPool; }}

        /// <summary>
        /// just to recognize the instantiation is via Pooling mechanism
        /// </summary>
        /// <param name="objectPool"></param>
        protected APoolable(IObjectPool objectPool)
        {
            ParentObjectPool = objectPool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static T Get()
        {
            // ReSharper disable once AssignNullToNotNullAttribute - INTENTIONAL
            return Get(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initializationLambda"></param>
        /// <returns></returns>
        public static T Get([NotNull] Action<T> initializationLambda)
        {
            var instance = DefaultObjectPool<T>.Singleton.Get();

            instance.BeforeGet();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse - INTENTIONAL
            if (initializationLambda != null)
                try
                {
                    initializationLambda(instance);
                }
                catch
                {
                    DefaultObjectPool<T>.Singleton.Return(instance);
                    throw;
                }

            
// ReSharper disable once UnusedVariable
            int previousInPool = 
                // marks the instance being out of pool
                Interlocked.Exchange(ref instance._inPool, MarkNotInPool);

#if DEBUG
            

            if (previousInPool == MarkNotInPool)
                DebugHelper.TryBreak("APoolable<"+typeof(T)+">. Get object in pool with wrong marking", instance);
#endif

            return instance;
        }

        protected virtual void BeforeGet()
        {
            
        }

        /// <summary>
        /// oriented to provide finalization of the objects, that should not be reused in original form after next get from the pool
        /// 
        /// raising an exception OR returning false from the method WILL STOP PROCESS OF THE RETURN 
        /// of the instance to the pool if the allowCancellation in Return is set to true
        /// 
        /// 
        /// </summary>
        protected virtual bool FinalizeBeforeReturn()
        {
            return true;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        public static void Return(ref T instance)
        {
            if (ReferenceEquals(instance, null))
                throw new ArgumentNullException("instance");

            instance.Return();

            instance = null;
        }



        /// <summary>
        /// return process can be stopped either by raising an exception in FinalizeBeforeReturn or returning false from it
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Return(bool allowReturnCancellation)
        {
            try
            {
                // can raise exception to prevent the return
                if (!FinalizeBeforeReturn() &&
                    allowReturnCancellation)
                    return false;
            }
            catch
            {
                if (allowReturnCancellation)
                    throw;
            }

            int previousInPool = Interlocked.Exchange(ref _inPool, MarkInPool);

            if (previousInPool == MarkInPool)
            {
#if DEBUG
                DebugHelper.TryBreak("Instance of type " + typeof (T) + " is already in pool");
#endif
                return true;
            }

            try
            {
                DefaultObjectPool<T>.Singleton.Return(this as T);
            }
            catch
            {
                Interlocked.Exchange(ref _inPool, MarkNotInPool);
                throw;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInPool { get { return _inPool == MarkInPool; } }

        /// <summary>
        /// 
        /// </summary>
        public void Return()
        {
            Return(true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static int CountOfPooled
        {
            get { return DefaultObjectPool<T>.Singleton.Count; }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public static class IPoolableExtensions
    {
        public static bool TryReturn(this IPoolable poolable,bool allowReturnCancellation)
        {
            if (ReferenceEquals(poolable, null))
                return false;

            try
            {
                if (!poolable.IsInPool)
                    return poolable.Return(allowReturnCancellation);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryReturn(this IPoolable poolable)
        {
            return TryReturn(poolable, true);
        }

        
    }
}
