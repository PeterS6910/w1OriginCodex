using System;
using System.Collections.Generic;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class RequestCollectionAdapter<TRequest>
        : IRequestCollection<TRequest>
    {
        private readonly LinkedList<TRequest> _requests;

        public RequestCollectionAdapter()
        {
            _requests = new LinkedList<TRequest>();
        }

        public void AddLastRequest(TRequest request)
        {
            _requests.AddLast(request);
        }

        public void AddFirstRequest(TRequest request)
        {
            _requests.AddFirst(request);
        }

        public int Count { get { return _requests.Count; } }

        public TRequest GetAndRemoveFirstRequest()
        {
            if (_requests.First == null)
                return default(TRequest);

            var request = _requests.First.Value;
            _requests.RemoveFirst();

            return request;
        }

        public void Clear()
        {
            _requests.Clear();
        }
    }

    public abstract class AThreadPoolQueue<TRequest>
        : AThreadPoolQueue<
            TRequest, 
            RequestCollectionAdapter<TRequest>> 
        where TRequest : class
    {
        protected AThreadPoolQueue(IThreadPool threadPool)
            : base(threadPool, new RequestCollectionAdapter<TRequest>())
        {
        }

        protected void EnqueueInternal(TRequest item)
        {
            RequestCollection.AddLastRequest(item);

            FinishEnqueue();
        }

        public void UnblockAndEnqueue(TRequest item)
        {
            lock (RequestCollection)
            {
                IsBlocked = false;

                EnqueueInternal(item);
            }
        }

        public bool Enqueue(TRequest item)
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return false;

                EnqueueInternal(item);

                return true;
            }
        }
    }

    public class ThreadPoolQueue<TRequest>
        : AThreadPoolQueue<TRequest> 
        where TRequest : class, IProcessingQueueRequest
    {
        public ThreadPoolQueue(IThreadPool threadPool) : base(threadPool)
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

    public class ThreadPoolQueue<TRequest, TParam>
        : AThreadPoolQueue<TRequest>
        where TRequest : class, IProcessingQueueRequest<TParam>
    {
        private readonly TParam _param;

        public ThreadPoolQueue(
            IThreadPool threadPool,
            TParam param)
            : base(threadPool)
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
