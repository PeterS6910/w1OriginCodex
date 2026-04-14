using System;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Parsing
#else
namespace Contal.IwQuick.Parsing
#endif
{
    public abstract class APhdParsingContext:IDisposable
    {
        protected abstract void InternalDispose();

        private object _disposalLock = new object();
        private bool _aimedToBeDisposed = false;
        public bool AimedToBeDisposed
        {
            get
            {
                lock (_disposalLock)
                {
                    return _aimedToBeDisposed;
                }
            }
        }

        private void InternalDisposeAtomicity()
        {
            lock (_disposalLock)
            {
                if (!_aimedToBeDisposed)
                    _aimedToBeDisposed = true;
                else
                    return;
            }

            try
            {
                InternalDispose();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            InternalDisposeAtomicity();
        }

        ~APhdParsingContext()
        {
            InternalDisposeAtomicity();
        }
    }
}
