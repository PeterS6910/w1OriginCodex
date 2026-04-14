using System;
using System.Threading;

namespace Contal.IwQuick.Data
{
    public interface IReleasable
    {
        void Reserve();
        void Release();
    }

    public abstract class ACountedReleasable : IReleasable
    {
        private int _numReservations;

        protected abstract void InternalRelease(bool explicitDispose);

        ~ACountedReleasable()
        {
            Interlocked.Exchange(
                ref _numReservations,
                0);

            InternalRelease(false);
        }

        public void Reserve()
        {
            Interlocked.Increment(ref _numReservations);
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref _numReservations) == 0)
                InternalRelease(true);
        }
    }
}
