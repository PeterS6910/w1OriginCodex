using System.Threading;

namespace Contal.IwQuick.Threads
{
    public class WaitableMonitor : MonitorEx<OnDemandAutoResetEventAdapter>
    {
        private int _waiterCount;
        private int _expectedWaiterCount;

        private readonly Semaphore _pulsed = new Semaphore();

        private readonly AutoResetEvent _woken =
            new AutoResetEvent(true);

        protected override void InternalDispose(bool isExplicitDispose)
        {
            base.InternalDispose(isExplicitDispose);

            if (isExplicitDispose)
            {
                _woken.Close();
                _pulsed.Dispose();
            }
        }

        public void Wait()
        {
            int oldLockCount;

            lock (Lock)
            {
                oldLockCount = LockCount;
                LockCount = 0;

                ++_waiterCount;

                ExitInternal();
            }

            _pulsed.WaitOne();

            if (Interlocked.Decrement(ref _waiterCount) == _expectedWaiterCount)
                _woken.Set();

            if (!InitialTryEnter(oldLockCount))
                RepeatWaitAndTryEnter(oldLockCount);
        }

        public void Pulse()
        {
            lock (Lock)
            {
                _expectedWaiterCount = _waiterCount - 1;

                _pulsed.Release(1);

                _woken.WaitOne();
            }
        }

        public void PulseAll()
        {
            lock (Lock)
            {
                _expectedWaiterCount = 0;

                _pulsed.Release(_waiterCount);

                _woken.WaitOne();
            }
        }
    }
}
