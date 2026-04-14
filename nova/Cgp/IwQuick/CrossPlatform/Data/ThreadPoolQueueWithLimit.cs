using System;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{

    public class RequestCollectionWithLimitAdapter<TRequest>
        : RequestCollectionAdapter<TRequest>
    {
        private readonly int _queueThreshold;

        public RequestCollectionWithLimitAdapter(int queueThreshold)
        {
            _queueThreshold = queueThreshold;
        }

        public new void AddLastRequest(TRequest request)
        {
            base.AddLastRequest(request);

            if (Count >= _queueThreshold)
                GetAndRemoveFirstRequest();
        }
    }
    

    public abstract class AThreadPoolQueueWithLimit<TRequest>
        : AThreadPoolQueue<TRequest,RequestCollectionWithLimitAdapter<TRequest>> 
        where TRequest : class
    {
        protected AThreadPoolQueueWithLimit(
            IThreadPool threadPool,
            int queueThreshold)
            : base(
                threadPool, new RequestCollectionWithLimitAdapter<TRequest>(queueThreshold))
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

    public class ThreadPoolQueueWithLimit<TRequest>
       : AThreadPoolQueueWithLimit<TRequest>
       where TRequest : class, IProcessingQueueRequest
    {
        public ThreadPoolQueueWithLimit(IThreadPool threadPool, int queueThreshold)
            : base(threadPool, queueThreshold)
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

    public class ThreadPoolQueueWithLimit<TRequest, TParam>
        : AThreadPoolQueueWithLimit<TRequest>
        where TRequest : class, IProcessingQueueRequest<TParam>
    {
        private readonly TParam _param;

        public ThreadPoolQueueWithLimit(IThreadPool threadPool,int queueThreshold, TParam param)
            : base(threadPool, queueThreshold)
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
