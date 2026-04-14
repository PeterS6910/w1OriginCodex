using System;
using System.Threading;

#if !COMPACT_FRAMEWORK
using Contal.IwQuick.Data;
#else
using Contal.IwQuick.Data;
#endif


namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    [LwSerialize(1020)]
    [LwSerializeMode(LwSerializationMode.Selective)]
    public abstract class ADisposable:IDisposable
    {
        private volatile int _aimedToBeDisposed;
        private volatile int _disposed;

        /// <summary>
        /// 
        /// </summary>
        public bool Disposed
        {
            get
            {
                // DO NOT LOCK THIS , can lead to deadlocks when the value is necessary to be read throughout the disposing process
                return _disposed == 1;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool AimedToBeDisposed
        {
            get
            {
                // DO NOT LOCK THIS , can lead to deadlocks when the value is necessary to be read throughout the disposing process
                return _aimedToBeDisposed == 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExplicitDispose">if true, Dispose has been called explicitly from the code, not by GC finalizer</param>
        protected abstract void InternalDispose(bool isExplicitDispose);

        private void SynchronizedDispose(bool isExplicitDispose)
        {
#pragma warning disable 420
            if (Interlocked.Exchange(ref _aimedToBeDisposed, 1) == 0)
#pragma warning restore 420
                try
                {
                    InternalDispose(isExplicitDispose);
                }
                catch
                {
                }
                finally
                {
#pragma warning disable 420
                    Interlocked.Exchange(ref _disposed, 1);
#pragma warning restore 420
                }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            SynchronizedDispose(true);
        }

        /// <summary>
        /// use only within other InternalDispose calls
        /// </summary>
        /// <param name="isExplicitDispose"></param>
        public void Dispose(bool isExplicitDispose)
        {
            SynchronizedDispose(isExplicitDispose);
        }

        /// <summary>
        /// 
        /// </summary>
        ~ADisposable()
        {
            SynchronizedDispose(false);
        }
    }
}
