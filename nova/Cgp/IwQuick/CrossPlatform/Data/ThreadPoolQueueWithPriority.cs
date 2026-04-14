using System;

using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class RequestCollectionWithPriorityAdapter<TRequest>
        : IRequestCollection<TRequest>
    {
        private readonly PriorityBinaryHeap<TRequest> _requests;

        public RequestCollectionWithPriorityAdapter()
        {
            _requests = new PriorityBinaryHeap<TRequest>();
        }

        public void AddRequest(TRequest item, uint priority)
        {
            _requests.Add(
                item,
                priority);
        }

        public int Count
        {
            get { return _requests.Count; }
        }

        public TRequest GetAndRemoveFirstRequest()
        {
            return _requests.GetAndRemoveMinimum();
        }

        public void Clear()
        {
            _requests.Clear();
        }
    }

    public abstract class AThreadPoolQueueWithPriority<TRequest>
        : AThreadPoolQueue<TRequest, RequestCollectionWithPriorityAdapter<TRequest>>
        where TRequest : class
    {
        protected AThreadPoolQueueWithPriority(IThreadPool threadPool)
            : base(threadPool, new RequestCollectionWithPriorityAdapter<TRequest>())
        {
        }

        protected void EnqueueInternal(
            TRequest item,
            uint priority)
        {
            RequestCollection.AddRequest(
                item,
                priority);

            FinishEnqueue();
        }

        public void UnblockAndEnqueue(
            TRequest request,
            uint priority)
        {
            lock (RequestCollection)
            {
                IsBlocked = false;

                EnqueueInternal(
                    request,
                    priority);
            }
        }

        public void Enqueue(
            TRequest request,
            uint priority)
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return;

                EnqueueInternal(
                    request,
                    priority);
            }
        }
    }

    public class ThreadPoolQueueWithPriority<TRequest>
        : AThreadPoolQueueWithPriority<TRequest>
        where TRequest : class, IProcessingQueueRequest
    {
        public ThreadPoolQueueWithPriority(IThreadPool threadPool) : base(threadPool)
        {
        }

        protected override void Execute(TRequest request)
        {
            try
            {
                request.Execute();
            }
            catch (Exception ex)
            {
                request.OnError(ex);
            }
        }
    }


    public class ThreadPoolQueueWithPriority<TRequest, TParam> 
        : AThreadPoolQueueWithPriority<TRequest>
        where TRequest : class, IProcessingQueueRequest<TParam>
    {
        private readonly TParam _param;

        public ThreadPoolQueueWithPriority(IThreadPool threadPool, TParam param) : base(threadPool)
        {
            _param = param;
        }

        protected override void Execute(TRequest request)
        {
            try
            {
               request.Execute(_param);
            }
            catch (Exception error)
            {
                request.OnError(
                    _param,
                    error);
            }
        }
    }
}
