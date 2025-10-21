using System;
using Contal.IwQuick.CrossPlatform.Threads;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public abstract class AaSyncThreadPoolQueue<TRequest>
        : AAsyncThreadPoolQueue<
            TRequest,
            RequestCollectionAdapter<TRequest>>
        where TRequest : class
    {
        protected AaSyncThreadPoolQueue(IThreadPool threadPool)
            : base(threadPool, new RequestCollectionAdapter<TRequest>())
        {
        }

        protected void EnqueueInternal(TRequest request)
        {
            RequestCollection.AddLastRequest(request);

            FinishEnqueue();
        }

        protected void EnqueueTopInternal(TRequest request)
        {
            RequestCollection.AddFirstRequest(request);

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

        public void Enqueue(TRequest item)
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return;

                EnqueueInternal(item);
            }
        }

        public void EnqueueTop(TRequest item)
        {
            lock (RequestCollection)
            {
                if (IsBlocked)
                    return;

                EnqueueTopInternal(item);
            }
        }
        public int Count{get { return RequestCollection.Count; }}
    }


    public class AsyncThreadPoolQueue<TRequest>
        : AaSyncThreadPoolQueue<TRequest>
        where TRequest : class, IAsyncProcessingQueueRequest
    {
        public AsyncThreadPoolQueue(IThreadPool threadPool) : base(threadPool)
        {
        }

        protected override bool OnExecutionFinished(TRequest processedRequest, bool timedOut)
        {
            try
            {
                return processedRequest.OnExecutionFinished(timedOut);
            }
            catch (Exception error)
            {
                processedRequest.OnExecutionFinishedError(error);
                return true;
            }
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

    public interface IAsyncProcessingQueueRequest: IProcessingQueueRequest
    {
        bool OnExecutionFinished(bool timedOut);

        void OnExecutionFinishedError(Exception error);
    }

    public interface IAsyncProcessingQueueRequest<TParam> : IProcessingQueueRequest<TParam>
    {
        bool OnExecutionFinished(TParam param, bool timedOut);

        void OnExecutionFinishedError(
            TParam param, 
            Exception error);
    }

    public class AsyncThreadPoolQueue<TRequest, TParam>
        : AaSyncThreadPoolQueue<TRequest>
        where TRequest : class, IAsyncProcessingQueueRequest<TParam>
    {
        private readonly TParam _param;

        public AsyncThreadPoolQueue(
            IThreadPool threadPool,
            TParam param)
            : base(threadPool)
        {
            _param = param;
        }

        protected override bool OnExecutionFinished(TRequest processedRequest, bool timedOut)
        {
            try
            {
                return 
                    processedRequest.OnExecutionFinished(
                        _param, 
                        timedOut);
            }
            catch (Exception error)
            {
                processedRequest.OnExecutionFinishedError(
                    _param,
                    error);

                return true;
            }
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