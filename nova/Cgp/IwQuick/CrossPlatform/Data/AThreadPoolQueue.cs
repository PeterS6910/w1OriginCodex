using System;
using System.Threading;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public interface IProcessingQueueRequest
    {
        void Execute();

        void OnError(Exception error);
    }

    public interface IProcessingQueueRequest<TParam>
    {
        void Execute(TParam param);

        void OnError(
            TParam param,
            Exception error);
    }

    public interface IRequestCollection<TRequest>
    {
        int Count { get; }

        TRequest GetAndRemoveFirstRequest();

        void Clear();
    }


    public abstract class AThreadPoolQueue<TRequest, TRequestCollection>
        : ADisposable
        where TRequest : class
        where TRequestCollection : class, IRequestCollection<TRequest>
    {
        protected abstract void Execute(TRequest request);
   
        protected TRequestCollection RequestCollection;

        private bool _executionStatus;

        private readonly IThreadPool _threadPool;

        private readonly ManualResetEvent _noProcessingItems = new ManualResetEvent(true);
        public bool IsBlocked { get; protected set; }

        protected AThreadPoolQueue(
            IThreadPool threadPool,
            TRequestCollection requestCollection)
        {
            _threadPool = threadPool;
            RequestCollection = requestCollection;
        }

        public int Count
        {
            get
            {
                lock (RequestCollection)
                    return RequestCollection.Count;
            }
        }

        protected void FinishEnqueue()
        {
            if (_executionStatus)
                return;

            _executionStatus = true;
            _noProcessingItems.Reset();

            _threadPool.Execute(ProcessItems);
        }

        private void ProcessItems()
        {
            do
            {
                TRequest itemToProcess = null;

                lock (RequestCollection)
                {
                    if (RequestCollection.Count > 0)
                        itemToProcess = RequestCollection.GetAndRemoveFirstRequest();

                    if (itemToProcess == null)
                    {
                        _executionStatus = false;
                        _noProcessingItems.Set();

                        break;
                    }
                }

                Execute(itemToProcess);
            } while (true);
        }

        public void ClearAndBlock()
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return;

                IsBlocked = true;
                RequestCollection.Clear();
            }
        }

        public void Unblock()
        {
            lock (RequestCollection)
                IsBlocked = false;
        }

        public void WaitUntilIdle()
        {
            _noProcessingItems.WaitOne();
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            ClearAndBlock();
            WaitUntilIdle();
        }
    }
}