using System;
using System.Threading;

namespace Contal.IwQuick.Threads
{
    public interface IAutoResetEventAdapter
    {
        void Set();
        void Reset();
        void Wait();
        bool Wait(int timeout);

        void PrepareWait();
        void Release();

        void Dispose();
    }

    public struct OnDemandAutoResetEventAdapter : IAutoResetEventAdapter
    {
        private AutoResetEvent _event;
        private bool _initialState;

        public void Set()
        {
            if (_event != null)
                _event.Set();
            else
                _initialState = true;
        }

        public void Reset()
        {
            if (_event != null)
                _event.Reset();
            else
                _initialState = false;
        }

        public void Wait()
        {
            _event.WaitOne();
        }

        public bool Wait(int timeout)
        {
            return _event.WaitOne(timeout, false);
        }

        public void PrepareWait()
        {
            if (_event == null)
                _event = new AutoResetEvent(_initialState);
        }

        public void Release()
        {
        }

        public void Dispose()
        {
            if (_event != null)
                _event.Close();
        }
    }

    public struct CountedAutoResetEventAdapter : IAutoResetEventAdapter
    {
        private int _referenceCount;
        private bool _initialState;
        private AutoResetEvent _event;

        public void Set()
        {
            if (_event != null)
                _event.Set();
            else
                _initialState = true;
        }

        public void Reset()
        {
            if (_event != null)
                _event.Set();
            else
                _initialState = false;
        }

        public void Wait()
        {
            _event.WaitOne();
        }

        public bool Wait(int timeout)
        {
            return _event.WaitOne(timeout, false);
        }

        public void PrepareWait()
        {
            if (++_referenceCount == 1)
                _event = new AutoResetEvent(_initialState);
        }

        public void Release()
        {
            if (--_referenceCount == 0)
            {
                _event.Close();
                _event = null;
            }
        }

        public void Dispose()
        {
            if (_event != null)
                _event.Close();
        }
    }

    public class MonitorEx<TAutoResetEventAdapter> : ADisposable
        where TAutoResetEventAdapter : struct, IAutoResetEventAdapter
    {
        protected readonly object Lock = new object();

        protected int LockCount;

        private Thread _owner;

        private TAutoResetEventAdapter _enterFreeProvider = new TAutoResetEventAdapter();

        protected void ExitInternal()
        {
            _owner = null;

            _enterFreeProvider.Set();
        }

        protected bool InitialTryEnter(int initialLockCount)
        {
            var currentThread = Thread.CurrentThread;

            lock (Lock)
            {
                if (_owner == null)
                {
                    _owner = currentThread;
                    LockCount = initialLockCount;

                    _enterFreeProvider.Reset();

                    return true;
                }

                if (_owner.ManagedThreadId == currentThread.ManagedThreadId)
                {
                    ++LockCount;
                    return true;
                }

                _enterFreeProvider.PrepareWait();
            }

            return false;
        }

        protected void RepeatWaitAndTryEnter(int initialLockCount)
        {
            do
            {
                _enterFreeProvider.Wait();

                lock (Lock)
                    if (_owner == null)
                    {
                        _owner = Thread.CurrentThread;
                        LockCount = initialLockCount;

                        _enterFreeProvider.Release();
                        _enterFreeProvider.Reset();

                        return;
                    }
            } while (true);
        }

        public void Enter()
        {
            if (!InitialTryEnter(1))
                RepeatWaitAndTryEnter(1);
        }

        public bool TryEnter()
        {
            Thread currentThread = Thread.CurrentThread;

            lock (Lock)
            {
                if (_owner == null)
                {
                    _owner = currentThread;
                    LockCount = 1;

                    _enterFreeProvider.Reset();

                    return true;
                }

                if (_owner.ManagedThreadId == currentThread.ManagedThreadId)
                {
                    LockCount++;
                    return true;
                }

                return false;
            }
        }

        public bool TryEnter(int timeToWait)
        {
            if (InitialTryEnter(1))
                return true;

            do
            {
                var originalTickCount = Environment.TickCount;

                bool result = _enterFreeProvider.Wait(timeToWait);

                timeToWait -= unchecked(Environment.TickCount - originalTickCount);

                lock (Lock)
                {
                    if (_owner == null)
                    {
                        _owner = Thread.CurrentThread;
                        LockCount = 1;

                        _enterFreeProvider.Release();
                        _enterFreeProvider.Reset();

                        return true;
                    }

                    if (!result || timeToWait < 0)
                    {
                        _enterFreeProvider.Release();
                        return false;
                    }
                }

            } while (true);
        }

        public void Exit()
        {
            lock (Lock)
            {
                if (--LockCount > 0)
                    return;

                ExitInternal();
            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (isExplicitDispose)
                _enterFreeProvider.Dispose();
        }
    }
}
