using System;
using System.Threading;
using Contal.IwQuick.CrossPlatform.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.CrossPlatform.Threads
{
    public interface IThreadPool
    {
        void Execute(Action action);
    }

    public class ThreadPool
        : ADisposable
        , IThreadPool
    {
        private class PooledThread
            : ADisposable
        {
            private Thread _thread;
            private Action _threadAction;

            private readonly AObjectPoolWithSizeLimit<PooledThread> _threadObjectPool;
            private AutoResetEvent _isActionAssigned = new AutoResetEvent(true);

            public PooledThread(AObjectPoolWithSizeLimit<PooledThread> threadObjectPool)
            {
                _threadObjectPool = threadObjectPool;
            }

            private void InvokeAction()
            {
                try
                {
                    while (true)
                    {
                        _isActionAssigned.WaitOne();

                        if (AimedToBeDisposed)
                            break;

                        try
                        {
                            _threadAction();
                        }
                        catch (Exception error)
                        {
                            OnError(error);
                        }

                        if (!_threadObjectPool.Return(this))
                            break;
                    }
                }
                catch (Exception error)
                {
                    OnError(error);
                }
                finally
                {
                    _thread = null;
                    _threadAction = null;

                    _isActionAssigned.Close();
                    _isActionAssigned = null;
                }
            }

            protected void OnError(Exception error)
            {
            }

            public void BeforeReturn()
            {
                _threadAction = null;
            }

            public void Execute([NotNull] Action threadAction)
            {
                _threadAction = threadAction;

                if (_thread == null)
                {
                    _thread = new Thread(InvokeAction) { IsBackground = true };
                    _thread.Start();
                }
                else
                    _isActionAssigned.Set();
            }

            protected override void InternalDispose(bool isExplicitDispose)
            {
                if (_thread != null)
                {
                    _isActionAssigned.Set();
                    _thread.Join();
                }
            }
        }

        private class ThreadObjectPool : AObjectPoolWithSizeLimit<PooledThread>
        {
            public ThreadObjectPool(int sizeLimit)
                : base(sizeLimit)
            {
            }

            protected override PooledThread CreateObject()
            {
                return new PooledThread(this);
            }

            protected override void DisposeObject(PooledThread obj)
            {
                obj.Dispose();
            }

            protected override void BeforeReturnObject(PooledThread obj)
            {
                obj.BeforeReturn();
            }
        }

        private readonly ThreadObjectPool _threadObjectPool;

        public int PooledThreadsCount
        {
            get { return _threadObjectPool.SizeLimit; }
            set { _threadObjectPool.SizeLimit = value; }
        }

        public ThreadPool(int poolingThreadCount)
        {
            _threadObjectPool = new ThreadObjectPool(poolingThreadCount);
        }


        public void Execute(Action action)
        {
            _threadObjectPool.Get().Execute(action);
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            _threadObjectPool.Dispose();
        }
    }
}
