using System;
using System.Collections.Generic;
using System.Threading;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProcessingQueue
    {
        /// <summary>
        /// priority used for future Enqueue calls without priority definition
        /// </summary>
        uint DefaultPriority { get; set; }

        /// <summary>
        /// if true, all enqueue calls will actually enqueue nothing ; 
        /// USE WITH CAUTION
        /// </summary>
        bool DeclineEnqueueing { get; set; }

        /// <summary>
        /// if zero, no limit is applied
        /// </summary>
        int LimitQueueCount { get; set; }

        /// <summary>
        /// if true, and limit is exceeded, queue will be shrinked from one side or another, depending on Enqueue or EnqueueTop call
        /// 
        /// if Enqueue used and queue is to be shrinked, the first member will be withdrawn
        /// 
        /// if EnqueueTop used and queue is to be shrinked, the last member will be withdrawn
        /// </summary>
        bool ShrinkOnLimitExceeded { get; set; }

        /// <summary>
        /// count of all items in all queues
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        bool CountIsZero { get; }

        /// <summary>
        /// erases all items in all queues
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISynchronousProcessingQueue<T> : IProcessingQueue
    {
        /// <summary>
        /// Delay between two processings. By default 0.
        /// </summary>
        int PostProcessingDelay { get; set; }

        /// <summary>
        /// Delay between two processings if previous processing didn't end-up with timeout. By default 0.
        /// </summary>
        int PostSuccessDelay { get; set; }

        /// <summary>
        /// delay applied only when IdleItemToBeGenerated returns null , or fails
        /// and if there is no other item in the queue
        /// </summary>
        int IdlePostFailureDelay { get; set; }

        /// <summary>
        /// ThreadPriority assigned to MainProcessingThread loop
        /// </summary>
        ThreadPriority ProcessingThreadPriority { get; set; }

        /// <summary>
        /// defines whether peeking thread will try to wait when queue(s) are empty
        /// or will try to generate idle items and process those
        /// </summary>
        bool IdleItemsGenerated { get; set; }

        /// <summary>
        /// if true, does not allow null values to be returned from IdleItemsToBeGenerated delegate
        /// </summary>
        bool IdleItemsPreventNull { get; set; }

        /// <summary>
        /// returns the item that is currently in processing, or default(T) if there is no item in processing
        /// </summary>
        T ActualItem { get; }

        /// <summary>
        /// defines amount of time (by default 2000ms) that Dispose/InternalDispose will wait for MainProcessingThread to finish/Join,
        /// if timeout not fullfiled, MainProcessingThread is aborted
        /// </summary>
        int DisposalTimeout { get; set; }

       

        /// <summary>
        /// defines , how often will be the IDLE item generated when IdleItemGenerated == true
        /// 
        /// delay applied only when IdleItemToBeGenerated returns NONnull
        /// and if there is no other item in the queue
        /// </summary>
        int IdlePostSuccessDelay { get; set; }


        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(object queueKey, T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(T item);

        /// <summary>
        /// enqueues single item into FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        ItemCarrier<T> Enqueue(T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        ItemCarrier<T> Enqueue(T item, PQEnqueueFlags flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flags"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        ItemCarrier<T> Enqueue(T item, PQEnqueueFlags flags, uint priority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        ItemCarrier<T> EnqueueOrExecute(T item);

        /// <summary>
        /// enqueues single item on top of FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        ItemCarrier<T> EnqueueTop(T item);

       

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        void Enqueue(ICollection<T> items);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        void Enqueue(params T[] items);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        void EnqueueOrExecute(params T[] items);

        /// <summary>
        /// 
        /// </summary>
        event Action<T> ItemProcessing;

        /// <summary>
        /// 
        /// </summary>
        event Action<T, bool, object> ItemProcessingResult;

        /// <summary>
        /// 
        /// </summary>
        event Action<T, Exception> ItemProcessingError;

        /// <summary>
        /// 
        /// </summary>
        event Func<object,T> IdleItemToBeGenerated;

        /// <summary>
        /// more precise method to get ActualItem, because in some cases default(T) can be used as normal value;
        /// in this method the returned bool defines, if the ActualItem is defined or not
        /// </summary>
        /// <param name="itemOutput"></param>
        /// <returns></returns>
        bool TryGetActualItem(ref T itemOutput);

        /// <summary>
        /// removes the item of the queue/s by the condition implemented by user's comparer delegate
        /// </summary>
        /// <param name="comparingLambda"></param>
        /// <param name="state">any state object passed into the comparer delegate for the comparing process</param>
        /// <returns>count of items removed</returns>
        int RemoveWhere(Func<T,object,bool> comparingLambda,object state);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultiProcessingQueue<T> : IProcessingQueue
    {
        /// <summary>
        /// enqueues single item
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="flags"></param>
        /// <param name="item">item to be queued</param>
        /// <param name="priority"></param>
        /// <returns>processing ID of the item ; 
        /// or null if enqueue failed due disposal process ; 
        /// or null if enqueueing is declined ; 
        /// or null if queue was not empty and enqueuOnlyIfEmpty flag used</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        /// <exception cref="InvalidOperationException">if EnqueueFlags.ProhibitCallFromItemProcessing used, and the Enqueue was called within the ItemProcessing</exception>
        ItemCarrier<T> EnqueueByKey(
            object queueKey,
            T item,
            PQEnqueueFlags flags,
            uint priority);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        ItemCarrier<T> EnqueueByKey(object queueKey, T item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="flags"></param>
        /// <param name="items"></param>
        void EnqueueByKey(
            object queueKey,
            PQEnqueueFlags flags,
            ICollection<T> items);

        /// <summary>
        /// 
        /// </summary>
        event Action<object, T> MultiQueueItemProcessing;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        bool EnsureQueue(object queueKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        void AddQueue(object queueKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        bool RemoveQueue(object queueKey);

        /// <summary>
        /// returns count for specific queue
        /// </summary>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        int GetQueueItemCount(object queueKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        bool IsQueuePresent(object queueKey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        void Clear(object queueKey);

        /// <summary>
        /// 
        /// </summary>
        int CountOfQueues { get; }

        /// <summary>
        /// 
        /// </summary>
        object[] QueueKeys { get; }
    }
}