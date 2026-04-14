using System;
using System.Collections.Generic;
using Contal.IwQuick.CrossPlatform.Data;

namespace Contal.IwQuick.CrossPlatform
{
    /// <summary>
    /// Encapsulates periodic batch processing: incoming requests are stored into queue
    /// and processed in batches. The size of batch is determined by the number of 
    /// request stored during a period (of predefined length) since the last idle-time.
    /// </summary>
    /// <typeparam name="TRequest">The type of the requests</typeparam>
    public class BatchWorker<TRequest> : ABatchWorkerBase<TRequest, RequestCollectionAdapter<TRequest>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchExecutor">executor to be called on batch of items, as effect of timeout expiration after last Add call</param>
        /// <param name="timeout"></param>
        /// <param name="limitCountInBatch"></param>
        /// <exception cref="ArgumentNullException">if batchAction is null</exception>
        public BatchWorker(
            IBatchExecutor<TRequest> batchExecutor,
            long timeout,
            int limitCountInBatch)
            : base(
                batchExecutor,
                timeout,
                limitCountInBatch)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchExecutor">executor to be called on batch of items, as effect of timeout expiration after last Add call</param>
        /// <param name="timeout"></param>
        ///  <exception cref="ArgumentNullException">if batchAction is null</exception>
        public BatchWorker(
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
        /// <param name="newRequests"></param>
        public void Add(IEnumerable<TRequest> newRequests)
        {
            lock (RequestCollection)
            {
                foreach (var newRequest in newRequests)
                {
                    RequestCollection.AddLastRequest(newRequest);
                }

                FinishAdd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRequests"></param>
        public void Add(params TRequest[] newRequests)
        {
            Add((IEnumerable<TRequest>) newRequests);
        }
    }
}