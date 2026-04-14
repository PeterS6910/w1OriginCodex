using System;
using System.Collections.Generic;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.CrossPlatform
{
    public interface IBatchExecutor<TRequest>
    {
        int Execute(ICollection<TRequest> requests);
    }

    public abstract class ABatchExecutor<TRequest, TParam> : IBatchExecutor<TRequest>
    {
        private readonly TParam _param;

        protected ABatchExecutor(TParam param)
        {
            _param = param;
        }

        public int Execute(ICollection<TRequest> requests)
        {
            int processedItemsCount = 0;

            if (!BeforeBatch(_param))
                return 0;

            foreach (var request in requests)
            {
                try
                {
                    ExecuteInternal(
                        request,
                        _param);
                }
                catch (Exception error)
                {
                    if (!OnError(
                        request,
                        error))
                    {
                        break;
                    }
                }

                ++processedItemsCount;
            }

            AfterBatch(_param);

            return processedItemsCount;
        }

        protected abstract bool OnError(TRequest request, Exception error);

        protected virtual bool BeforeBatch(TParam param)
        {
            return true;
        }

        protected abstract void ExecuteInternal(
            TRequest request,
            TParam param);

        protected virtual void AfterBatch(TParam param)
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest">The type of the requests</typeparam>
    /// /// <typeparam name="TRequestCollection">The type of the request collection</typeparam>
    public abstract class ABatchWorkerBase<TRequest, TRequestCollection>
        where TRequestCollection : class, IRequestCollection<TRequest>, new()
    {
        protected TRequestCollection RequestCollection;

        private readonly LinkedList<TRequest> _oneBatchRequests =
            new LinkedList<TRequest>();

        private readonly IBatchExecutor<TRequest> _batchExecutor;

        private bool _batchRequestsProcessing;

        private volatile Action _postProcessingActions;

        private const long DefaultTimeout = 5000;
        private const long MinimalTimeout = 100;

        private long _timeout = DefaultTimeout;

        protected const int NoLimitCountInBatch = -1;
        private volatile int _limitCountInBatch = NoLimitCountInBatch;

        /// <summary>
        /// Gets the number of requests in the batch-worker's queue
        /// </summary>
        public int Count
        {
            get
            {
                lock (RequestCollection)
                    return RequestCollection.Count;
            }
        }

        /// <summary>
        /// always applies in the next round of addition
        /// </summary>
        public long Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value >= MinimalTimeout
                    ? value
                    : DefaultTimeout;
            }
        }

        /// <summary>
        /// always applies in the next round of evaluation
        /// </summary>
        public int LimitCountInBatch
        {
            get { return _limitCountInBatch; }
            set
            {
                _limitCountInBatch = value > 0
                    ? value
                    : NoLimitCountInBatch;
            }
        }

        private ITimer _currentTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchExecutor"></param>
        /// <param name="timeout"></param>
        /// <param name="limitCountInBatch"></param>
        protected ABatchWorkerBase(
            IBatchExecutor<TRequest> batchExecutor,
            long timeout,
            int limitCountInBatch)
        {
            RequestCollection = new TRequestCollection();

            _batchExecutor = batchExecutor;
            Timeout = timeout;
            LimitCountInBatch = limitCountInBatch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchExecutor"></param>
        /// <param name="timeout"></param>
        protected ABatchWorkerBase(
            IBatchExecutor<TRequest> batchExecutor,
            long timeout)
            : this(
                batchExecutor,
                timeout,
                NoLimitCountInBatch)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected void FinishAdd()
        {
            if (ReferenceEquals(_currentTimeout, null))
            {
                _currentTimeout =
                    TimerManager.Static.StartTimeout(
                        _timeout,
                        false,
                        PerformBatchAction);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerCarrier"></param>
        /// <returns></returns>
        private bool PerformBatchAction(TimerCarrier timerCarrier)
        {
            bool result = false;

            try
            {
                lock (RequestCollection)
                {
                    for (int i = _oneBatchRequests.Count;
                        _limitCountInBatch == NoLimitCountInBatch || i < _limitCountInBatch;
                        i++)
                    {
                        if (RequestCollection.Count == 0)
                            break;

                        _oneBatchRequests.AddLast(RequestCollection.GetAndRemoveFirstRequest());
                    }

                    _batchRequestsProcessing = true;
                }

                var processedBatchRequestsCount = _batchExecutor.Execute(_oneBatchRequests);

                lock (RequestCollection)
                {
                    if (_postProcessingActions != null)
                    {
                        try
                        {
                            _postProcessingActions();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                        _postProcessingActions = null;
                    }

                    _batchRequestsProcessing = false;

                    for (int i = 0; i < processedBatchRequestsCount; i++)
                    {
                        _oneBatchRequests.RemoveFirst();
                    }

                    if (RequestCollection.Count == 0
                        && _oneBatchRequests.Count == 0)
                    {
                        result = true;
                        _currentTimeout = null;
                    }
                }
            }
            catch (Exception err)
            {
                HandledExceptionAdapter.Examine(err);
                // catch the exception from _batchAction 
                // because TimerManager would try to re-run the timeout
            }

            return result;
        }

        /// <summary>
        /// Clears the batch-worker's request queue
        /// </summary>
        public void Clear()
        {
            Clear(null);
        }
        /// <summary>
        /// Clears the batch-worker's request queue and performs action in the lock
        /// </summary>
        /// <param name="action"></param>
        public void Clear(Action action)
        {
            lock (RequestCollection)
            {
                if (action != null)
                {
                    try
                    {
                        if (_batchRequestsProcessing)
                            _postProcessingActions += action;
                        else
                            action();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }

                RequestCollection.Clear();
            }
        }
    }
}