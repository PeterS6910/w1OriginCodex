//#define DEBUG_PROCESSING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System.Linq;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class SingleQueueIdentifier
    {
        internal SingleQueueIdentifier()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SingleQueue";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PQProcessingState
    {
        /// <summary>
        /// 
        /// </summary>
        Instantiated = 0,
        /// <summary>
        /// 
        /// </summary>
        Enqueued = 1,
        /// <summary>
        /// 
        /// </summary>
        Processing = 2,
        /// <summary>
        /// 
        /// </summary>
        Processed = 3,
        /// <summary>
        /// 
        /// </summary>
        Disposed = 4
    }

    /// <summary>
    /// non-generic ancestor for ItemCarrier&lt;T&gt; class3
    /// </summary>
    public abstract class AItemCarrier : ADisposable
    {
        protected static readonly Threads.EventWaitHandlePool<ManualResetEvent> _mrePool =
            new Threads.EventWaitHandlePool<ManualResetEvent>();

        /// <summary>
        /// marking the item before processing
        /// </summary>
        public const int INVALID_PROCESSING_ID = -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemCarrier<T> : AItemCarrier
    {
        // no necessarity to make getters/setters , when data fields only used in ProcessingQueue

        private readonly int _processingId = INVALID_PROCESSING_ID;
        internal readonly T _item;
        internal readonly uint _priority;

        private volatile ManualResetEvent _finishedMutex = null;
        private readonly object _finishedMutexLock = new object();
        // just a marking, to not return the mutex if in waiting process
        // int type, because of Interlocked
        private int _finishedMutexInWaiting = 0;

        internal volatile PQProcessingState _processingState = PQProcessingState.Instantiated;
        private readonly object _processingStateLock = new object();

        /// <summary>
        /// 
        /// </summary>
        public PQProcessingState ProcessingState
        {
            get
            {
                lock(_processingStateLock)
                    return _processingState;
            }
            internal set
            {
                lock (_processingStateLock)
                {
                    // only incrementing state values are allowed
                    if (value > _processingState)
                    {
                        PQProcessingState originalValue = _processingState;
                       
                        _processingState = value;

                        // better to release, when state is already stored
                        if (_finishedMutex != null)
                            // double locking schema due fact, that another Thread
                            // can evaluate the TryReturnMutex right now by Wait call
                            lock (_finishedMutexLock)
                            {
                                if (_finishedMutex != null)
                                {
                                    if (originalValue == PQProcessingState.Processing &&
                                          value == PQProcessingState.Processed)
                                        _finishedMutex.Set();
                                }
                            }
                    }
                    else
                    {
                        DebugHelper.NOP(value);
                    }
                }
            }
        }

// ReSharper disable once StaticFieldInGenericType
        

        /// <summary>
        /// this carrier should be only instantiated within IwQuick
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <param name="processingId"></param>
        /// <param name="allocateMutex"></param>
        internal ItemCarrier(T item, uint priority, int processingId, bool allocateMutex)
        {
            _item = item;
            _priority = priority;
            _processingId = processingId;

            if (allocateMutex)
            {
                // no locking needed here, it's constructor
                _finishedMutex = _mrePool.Get();
                _finishedMutex.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public uint Priority
        {
            get
            {
                return _priority;
            }
        }

        /// <summary>
        /// unique id the item gets on instantiation
        /// </summary>
        public int ProcessingId
        {
            get
            {
                return _processingId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Item
        {
            get
            {
                return _item;
            }
        }



        //public override bool Equals(T obj)
        //{
        //    return obj.Equals(_item);
        //    //return _item.Equals(obj);
        //}

        private void TryReturnMutex()
        {
            if (_finishedMutex != null)
            {
                lock (_finishedMutexLock)
                {
                    if (_finishedMutex != null &&
                        _finishedMutexInWaiting == 0) // condition preventing to return the mutex if 
                                                      // some wait is in progress OR is entering the wait
                    {
                        _mrePool.Return(_finishedMutex);
                        _finishedMutex = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExplicitDispose"></param>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            ProcessingState = PQProcessingState.Disposed;

            TryReturnMutex();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Wait(int timeout)
        {
            bool succeededWithoutTimeout = false;
            //bool tryReturnMutex = false;
            bool mutexNotUsable = false;

            if (_finishedMutex != null)
                lock (_finishedMutexLock)
                {
                    if (_finishedMutex != null)
                        _finishedMutexInWaiting++; // no need to do Interlocked, cause all
                                                   // _finishedMutexInWaiting operations are in lock (_finishedMutexLock)
                    else
                        mutexNotUsable = true;
                }
            else
                mutexNotUsable = true;

            if (mutexNotUsable)
            {
                var psSnapshot = ProcessingState;
                if (psSnapshot == PQProcessingState.Processed ||
                    psSnapshot == PQProcessingState.Disposed)
                    return true;
                
                // means _finishedMutex is null, but not due returning after waiting
                throw new InvalidOperationException("Item was not prepared for waiting on Enqueue call");
            }

            try
            {
                _finishedMutex.WaitOne(0, false);

                if (timeout > 0)
                    succeededWithoutTimeout = _finishedMutex.WaitOne(timeout, false);
                else
                    succeededWithoutTimeout = _finishedMutex.WaitOne();
            }
            // to catch even thread aborts
            // so the _finishedMutexInWaiting counter would not stuck in inconsistent state
            catch
            {

            }
            finally
            {
                lock (_finishedMutexLock)
                {
                    if (_finishedMutexInWaiting > 0)
                        _finishedMutexInWaiting--;
                    else
                        DebugHelper.NOP();

                    if (succeededWithoutTimeout)
                    {
                        var psSnapshot = ProcessingState;
                        if (psSnapshot == PQProcessingState.Processed ||
                            psSnapshot == PQProcessingState.Disposed)
                            TryReturnMutex();
                    }
                }
            }

            return succeededWithoutTimeout;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Wait()
        {
            Wait(-1);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PQMode
    {
        /// <summary>
        /// means the processing queue will process the item on top of queue no-matter 
        /// how long the ItemProcessing takes
        /// </summary>
        Synchronous,
        /// <summary>
        /// means the processing queue will process the item on top of queue within ItemProcessing
        /// until EndItemProcessing is called, 
        /// if EndItemProcessing is not called within AsynchronousTimeout, ItemProcessingTimedOut is called
        /// </summary>
        Asynchronous
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PQPickQueueMode
    {
        /// <summary>
        /// pick each other sub-queue in the queue list
        /// </summary>
        RoundRobin,
        /// <summary>
        /// picking always the same sub-queue in the queue list, 
        /// until that one empty
        /// </summary>
        FlushQueueFirst,
        /// <summary>
        /// picking random sub-subqueue
        /// </summary>
        Random
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AProcessingQueue : ADisposable
    {


        /// <summary>
        /// summarizes and locks up, if all queues are empty
        /// </summary>
        private readonly ManualResetEvent _nonEmptyMutex = new ManualResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        public static readonly object SINGLE_QUEUE_IDENTIFIER = new SingleQueueIdentifier();

        protected const int DEFAULT_QUEUE_ID = int.MinValue;
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool LockDueQueuesEmpty()
        {
            return _nonEmptyMutex.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool UnlockDueQueuesNotEmpty()
        {
            return _nonEmptyMutex.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual void WaitForQueuesNotBeingEmpty()
        {
            _nonEmptyMutex.WaitOne(-1, false);
        }

        private const uint DEFAULT_IMPLICIT_PRIORITY = 0;

        private volatile uint _defaultPriority = DEFAULT_IMPLICIT_PRIORITY;

        /// <summary>
        /// priority used for future Enqueue calls without priority definition
        /// </summary>
        public uint DefaultPriority
        {
            get { return _defaultPriority; }
            set { _defaultPriority = value; }
        }

        private volatile bool _declineEnqueueing = false;
        /// <summary>
        /// if true, all enqueue calls will actually enqueue nothing;
        /// USE WITH CAUTION
        /// </summary>
        public bool DeclineEnqueueing
        {
            get { return _declineEnqueueing; }
            set { _declineEnqueueing = value; }
        }

        private readonly object _processingIdCounterSync = new object();

        /// <summary>
        /// valid area limited to &lt;0,int.MaxValue&gt; by GetNextProcessingId() call
        /// </summary>
        private volatile int _processingIdCounter = 0;

        /// <summary>
        /// finds next processing ID for item synchronously
        /// </summary>
        /// <returns></returns>
        protected internal int GetNextProcessingId()
        {
            int processingId;

            lock (_processingIdCounterSync)
            {
                if (_processingIdCounter < int.MaxValue)
                {
                    processingId = _processingIdCounter++;
                }
                else
                {
                    processingId = int.MaxValue;
                    _processingIdCounter = 0;
                }
            }

            return processingId;
        }

    }

    /// <summary>
    /// class with one or set of queues and processing thread that is able to call ItemProcessing event over the items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProcessingQueue<T> : AProcessingQueue
    {
        /// <summary>
        /// list of parallel sub-queues
        /// </summary>
        private volatile SyncDictionary<object, LevelSortedQueue<ItemCarrier<T>>> _queues = null;

        private object[] _queueKeys = null;

        // snapshot of _queueKeys for the outside world
        private volatile object[] _queueKeysSnapshot = null;

        private readonly object _queuesSyncRoot = new object();

        private volatile int _postProcessingDelay = 0;

        /// <summary>
        /// Delay between two processings. By default 0.
        /// </summary>
        public int PostProcessingDelay
        {
            get { return _postProcessingDelay; }
            set
            {
                if (value > 0)
                    _postProcessingDelay = value;
                else
                    _postProcessingDelay = 0;
            }
        }

        private volatile int _postSuccessDelay = 0;

        /// <summary>
        /// Delay between two processings if previous processing didn't end-up with timeout. By default 0.
        /// </summary>
        public int PostSuccessDelay
        {
            get { return _postSuccessDelay; }
            set
            {
                if (value > 0)
                    _postSuccessDelay = value;
                else
                    _postSuccessDelay = 0;
            }
        }

        private int _idleGeneratingDelay = 5;

        /// <summary>
        /// defines , how often will be the IDLE item generated when IdleItemGenerated == true
        /// it also defines the interval, how often will queue look for new items to be processed when IdleItemGenerated == true
        /// </summary>
        public int IdleGeneratingDelay
        {
            get { return _idleGeneratingDelay; }
            set
            {
                if (value > 0)
                    _idleGeneratingDelay = value;
                else
                    _idleGeneratingDelay = 0;
            }
        }

        private volatile int _asynchronousTimeout = 100;

        /// <summary>
        /// asynchronous timeout between the begin of itemprocessing and EndItemProcessing call
        /// 
        /// if 0 or negative, it means, that timeout is infinite
        /// </summary>
        public int AsynchronousTimeout
        {
            get { return _asynchronousTimeout; }
            set
            {
                if (value > 0)
                    _asynchronousTimeout = value;
                else
                    _asynchronousTimeout = 0;
            }
        }



        /*
        private bool _processingBeginsAsynchronously = false;
        public bool ProcessingBeginsAsynchronously
        {
            get { return _processingBeginsAsynchronously; }
            set
            {
                _processingBeginsAsynchronously = value;
            }
        }*/

// ReSharper disable once StaticFieldInGenericType

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <param name="expectedWaiting"></param>
        /// <returns></returns>
        private ItemCarrier<T> PrepareItemCarrier(T item, uint priority, bool expectedWaiting)
        {
            return new ItemCarrier<T>(
                item,
                priority,
                GetNextProcessingId(),
                expectedWaiting);
        }

        /// <summary>
        /// mechanism prepared for INTERNAL sub-queue indexing
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        private LevelSortedQueue<ItemCarrier<T>> GetQueueById(int queueId,out object queueKey)
        {
            lock (_queuesSyncRoot)
            {
                if (_queues.Count > 1)
                {

                    queueKey = _queueKeys[queueId];
                    LevelSortedQueue<ItemCarrier<T>> queue;

                    _queues.TryGetValue(queueKey, out queue);
                    return queue;
                }

                queueKey = _queueKeys.FirstOrDefault();
                return _queues.Values.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        private LevelSortedQueue<ItemCarrier<T>> GetQueueByKey(object queueKey)
        {
            lock (_queuesSyncRoot)
            {
                if (!ReferenceEquals(queueKey, null))
                {
                    LevelSortedQueue<ItemCarrier<T>> queue;
                    if (_queues.TryGetValue(queueKey, out queue))
                    {
                        //queueId = QueueKey2QueueId(queueKey);
                        return queue;
                    }
                }

                //queueId = -1;
                return null;



                // THIS CONDITIONS SHOULD NOT APPLY
                // THE GetQueueByKey should be called only when queueKey is relevant
                //// this would return 0 or 1
                ////queueId = _queues.Count;

                //return _queues.Values.FirstOrDefault();
            }
        }

        private struct PQEnqueueFlagsParsed
        {
            internal readonly bool enqueueOnlyIfEmpty;
            internal readonly bool onTop;
            internal readonly bool validateCallFromItemProcessing;
            internal readonly bool expectedWaiting;
            internal readonly bool prohibitCallFromItemProcessing;

            internal PQEnqueueFlagsParsed(PQEnqueueFlags flags)
            {
                enqueueOnlyIfEmpty = ((flags & PQEnqueueFlags.EnqueueOnlyIfEmpty) == PQEnqueueFlags.EnqueueOnlyIfEmpty);
                
                onTop = ((flags & PQEnqueueFlags.OnTop) == PQEnqueueFlags.OnTop);
                
                validateCallFromItemProcessing = ((flags & PQEnqueueFlags.ValidateCallFromItemProcessing) ==
                                                   PQEnqueueFlags.ValidateCallFromItemProcessing);
                expectedWaiting = ((flags & PQEnqueueFlags.ExpectedWaitingToFinish) ==
                                      PQEnqueueFlags.ExpectedWaitingToFinish);
                prohibitCallFromItemProcessing = ((flags & PQEnqueueFlags.ProhibitCallFromItemProcessing) ==
                                                   PQEnqueueFlags.ProhibitCallFromItemProcessing);
            }
        }
             

    #region Single Enqueue methods

        /*
        /// <summary>
        /// enqueues single item
        /// </summary>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, first queue will be used</param>
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
        public ItemCarrier<T> Enqueue(
            int queueId, 
            T item, 
            PQEnqueueFlags flags,
            uint priority)
        {
            return EnqueueSingle(null, queueId, item, flags, priority);
        }*/

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
        public ItemCarrier<T> EnqueueByKey(
            object queueKey, 
            T item, 
            PQEnqueueFlags flags,
            uint priority)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            return EnqueueSingle(queueKey, DEFAULT_QUEUE_ID, item, flags, priority);
        }

        /// <summary>
        /// enqueues single item
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, first queue will be used</param>
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
        private ItemCarrier<T> EnqueueSingle(
            object queueKey,
            int queueId, 
            T item, 
            PQEnqueueFlags flags,
            uint priority)

        {
            if (AimedToBeDisposed || DeclineEnqueueing)
                return null; //ItemCarrier<T>.INVALID_PROCESSING_ID;

            if (ReferenceEquals(queueKey, null))
            {
                if (queueId < 0)
                    queueId = 0;

                Validator.CheckIntegerRange(queueId, 0, CountOfQueues);
            }

            ItemCarrier<T> ic;
            var f = new PQEnqueueFlagsParsed(flags);

            uint enqueuePriority = DefaultPriority;
            if ((flags & PQEnqueueFlags.WithPriority) == PQEnqueueFlags.WithPriority)
                enqueuePriority = priority;


            if (f.validateCallFromItemProcessing ||
                f.prohibitCallFromItemProcessing)
            {
                if (// id of the thread calling the Enqueue
                    Thread.CurrentThread.ManagedThreadId ==
                    _processingThreadIdSnapshot) // this id snapshot can cover the situation when custom thread pool is used
                {
                    if (f.prohibitCallFromItemProcessing)
                        throw new InvalidOperationException("Invalid Enqueue operation within ItemProcessing as EnqueueFlags.ProhibitCallFromItemProcessing used");

                    LevelSortedQueue<ItemCarrier<T>> queue;
                    if (ReferenceEquals(queueKey, null))
                        queue = GetQueueById(queueId, out queueKey);
                    else
                        queue = GetQueueByKey(queueKey);

                    if (queue == null)
                        return null;

                    // means the enqueue is called within the ItemProcessing
                    if (f.enqueueOnlyIfEmpty)
                    {
                        if (queue.Count > 0)
                            return null; //ItemCarrier<T>.INVALID_PROCESSING_ID;
                    }

                    ic = PrepareItemCarrier(item,enqueuePriority, f.expectedWaiting);

                    PerformSingleItemProcessing(queueKey, ic);

                    return ic;//ic._processingId;
                }
            }

            lock (_queuesSyncRoot)
            {
                LevelSortedQueue<ItemCarrier<T>> queue;
                if (ReferenceEquals(queueKey, null))
                    queue = GetQueueById(queueId, out queueKey);
                else
                    queue = GetQueueByKey(queueKey);

                if (queue == null)
                    return null;
                
                if (f.enqueueOnlyIfEmpty && queue.Count > 0)
                    return null;//ItemCarrier<T>.INVALID_PROCESSING_ID;

                ApplyLimitConstraints(queue, f.onTop);

                ic = PrepareItemCarrier(item, enqueuePriority, f.expectedWaiting);

                queue.Enqueue(ic, enqueuePriority, f.onTop);

                ic.ProcessingState = PQProcessingState.Enqueued;

                UnlockDueQueuesNotEmpty();
            }

            EnsureProcessingThread();

            return ic;//ic._processingId;
        }

        /*
        /// <summary>
        /// enqueues single item
        /// </summary>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, first queue will be used</param>
        /// <param name="flags"></param>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item ; 
        /// or null if enqueue failed due disposal process ; 
        /// or null if enqueueing is declined ; 
        /// or null if queue was not empty and enqueuOnlyIfEmpty flag used</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        /// <exception cref="InvalidOperationException">if EnqueueFlags.ProhibitCallFromItemProcessing used, and the Enqueue was called within the ItemProcessing</exception>
        public ItemCarrier<T> Enqueue(int queueId, T item, PQEnqueueFlags flags)
        {
            return EnqueueSingle(null, queueId, item, flags, DefaultPriority);
        }*/

        /*
        /// <summary>
        /// enqueues single item by priority from the end of the queue
        /// </summary>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, first queue will be used</param>
        /// <param name="item">item to be queued</param>
        /// <param name="priority">priority according which the item will be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        public ItemCarrier<T> EnqueueP(int queueId, T item, uint priority)
        {

            return EnqueueSingle(null, queueId, item, PQEnqueueFlags.WithPriority, priority);

            /*
            if (AimedToBeDisposed || DeclineEnqueueing)
                return null;//ItemCarrier<T>.INVALID_PROCESSING_ID;

            if (queueId < 0)
                queueId = 0;

            Validator.CheckIntegerRange(queueId, 0, _queues.Length);

            ItemCarrier<T> ic;

            lock (_queuesSyncRoot)
            {
                LinkedList<ItemCarrier<T>> queue = _queues[queueId];

                if (_limitQueueCount > 0 && queue.Count >= _limitQueueCount)
                    throw new ResourceExhaustedException("The queue is limited by count " + _limitQueueCount);

                LinkedListNode<ItemCarrier<T>> node;
                node = queue.First;

                ic = PrepareItemCarrier(item, priority);

                // empty list 
                if (node == null)
                    queue.AddLast(ic);
                else
                {
// ReSharper disable once PossibleNullReferenceException
                    while (node.Value._priority <= priority)
                    {
                        if (node != queue.Last)
                            node = node.Next;
                        else
                            break;
                    }

                    if (node.Value._priority > priority)
                        queue.AddBefore(node, ic);
                    else
                        queue.AddAfter(node, ic);
                }

                UnlockDueQueuesNotEmpty();
            }

            EnsureProcessingThread();

            return ic;//ic._processingId;*/
        //}*/

        /*       

        /// <summary>
        /// enqueues single item by priority from the beginning of the queue
        /// </summary>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, first queue will be used</param>
        /// <param name="item">item to be queued</param>
        /// <param name="priority">priority according which the item will be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        public ItemCarrier<T> EnqueueTopP(int queueId, T item, uint priority)
        {
            return EnqueueSingle(null, queueId, item, PQEnqueueFlags.WithPriority | PQEnqueueFlags.OnTop, priority);
            
            /*if (AimedToBeDisposed || DeclineEnqueueing)
                return null;//ItemCarrier<T>.INVALID_PROCESSING_ID;

            if (queueId < 0)
                queueId = 0;

            Validator.CheckIntegerRange(queueId, 0, _queues.Length);

            ItemCarrier<T> ic;

            lock (_queuesSyncRoot)
            {
                LinkedList<ItemCarrier<T>> queue = _queues[queueId];

                if (_limitQueueCount > 0 && queue.Count >= _limitQueueCount)
                    throw new ResourceExhaustedException("The queue is limited by count " + _limitQueueCount);

                LinkedListNode<ItemCarrier<T>> node;
                node = queue.First;

                ic = PrepareItemCarrier(item, priority);

                // empty list 
                if (node == null)
                    queue.AddLast(ic);
                else
                {
// ReSharper disable once PossibleNullReferenceException
                    while (node.Value._priority < priority)
                    {
                        if (node != queue.Last)
                            node = node.Next;
                        else
                            break;
                    }

                    if (node.Value._priority >= priority)
                        queue.AddBefore(node, ic);
                    else
                        queue.AddAfter(node, ic);
                }

                UnlockDueQueuesNotEmpty();
            }

            EnsureProcessingThread();

            return ic;//ic._processingId;*/
        //}*/

        /// <summary>
        /// applies limit exception or limit shrinking if limit is non-zero positive count
        /// </summary>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        private void ApplyLimitConstraints(LevelSortedQueue<ItemCarrier<T>> queue, bool onTop)
        {
            if (queue == null)
                return;

            if (_limitQueueCount > 0)
            {
                if (!queue.ValidateLimitCount(_limitQueueCount, _shrinkOnLimitExceeded, onTop))
                    throw new ResourceExhaustedException("The queue is limited by count " + _limitQueueCount);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(object queueKey, T item)
        {         
            LevelSortedQueue<ItemCarrier<T>> queue;

            if (ReferenceEquals(queueKey, null))
                queue = GetQueueById(0, out queueKey);
            else
                queue = GetQueueByKey(queueKey);

            if (null != queue)
                return queue.Contains(
                    (itemInQueue, itemToCompareWith) => itemInQueue.Item.Equals(itemInQueue),
                    item
                    );

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Contains(null, item);
        }

        /// <summary>
        /// enqueues single item into FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        public ItemCarrier<T> Enqueue(T item)
        {
            return EnqueueSingle(null, DEFAULT_QUEUE_ID, item, PQEnqueueFlags.None, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public ItemCarrier<T> Enqueue(T item, PQEnqueueFlags flags)
        {
            return EnqueueSingle(null, DEFAULT_QUEUE_ID, item, flags, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flags"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public ItemCarrier<T> Enqueue(T item, PQEnqueueFlags flags, uint priority)
        {
            return EnqueueSingle(null, DEFAULT_QUEUE_ID, item, flags | PQEnqueueFlags.WithPriority, priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public  ItemCarrier<T> EnqueueOrExecute(T item)
        {
            return EnqueueSingle(null, DEFAULT_QUEUE_ID, item, PQEnqueueFlags.ValidateCallFromItemProcessing, 0);
        }

        /*
        /// <summary>
        /// enqueues single item into FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <param name="enqueueOnlyIfEmpty"></param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        public ItemCarrier<T> Enqueue(T item,bool enqueueOnlyIfEmpty)
        {
            return EnqueueSingle(
                null, 
                -1, 
                item, 
                (enqueueOnlyIfEmpty?PQEnqueueFlags.EnqueueOnlyIfEmpty:PQEnqueueFlags.None),
                0);
        }*/

        /*
        /// <summary>
        /// enqueues single item by priority from the end of the FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <param name="priority">priority according which the item will be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        public ItemCarrier<T> EnqueueP(T item, uint priority)
        {
            return EnqueueSingle(null, -1, item, PQEnqueueFlags.WithPriority, priority);
        }*/

        
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public ItemCarrier<T> EnqueueP(T item, uint priority, PQEnqueueFlags flags)
        {
            return EnqueueSingle(null, -1, item, flags | PQEnqueueFlags.WithPriority, priority);
        }*/

        /*
        /// <summary>
        /// enqueues single item by priority from the beginning of the FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <param name="priority">priority according which the item will be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        public ItemCarrier<T> EnqueueTopP(T item, uint priority)
        {
            return EnqueueSingle(null, -1, item, PQEnqueueFlags.WithPriority | PQEnqueueFlags.OnTop, priority);
        }
         */

        /// <summary>
        /// enqueues single item on top of FIRST queue
        /// </summary>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        public ItemCarrier<T> EnqueueTop(T item)
        {
            return EnqueueSingle(null, DEFAULT_QUEUE_ID, item, PQEnqueueFlags.OnTop,0);
        }

        /*
        /// <summary>
        /// enqueues single item into FIRST queue
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        public ItemCarrier<T> Enqueue(int queueId, T item)
        {
            return EnqueueSingle(null, queueId, item, PQEnqueueFlags.None,0);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        public ItemCarrier<T> EnqueueByKey(object queueKey,T item)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            return EnqueueSingle(queueKey, DEFAULT_QUEUE_ID, item, PQEnqueueFlags.None, 0);
        }

        /*
        /// <summary>
        /// enqueues single item on top of queue
        /// </summary>
        /// <param name="queueId">defines queueId from 0 to maximum queue count, if lower than 0, FIRST queue will be used</param>
        /// <param name="item">item to be queued</param>
        /// <returns>processing ID of the item, or null if enqueue failed due disposal process</returns>
        /// <exception cref="IndexOutOfRangeException">if the specified queueId is out sub-queues range present</exception>
        /// <exception cref="ResourceExhaustedException">if the queue count is over LimitQueueCount and 
        /// ShrinkOnLimitExceeded is not applied</exception>
        public ItemCarrier<T> EnqueueTop(int queueId, T item)
        {
            return EnqueueSingle(null, queueId, item, PQEnqueueFlags.OnTop,0);
        }*/
        #endregion

        #region Multi Enqueue methods

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="onTop"></param>
        /// <param name="validateCallFromItemProcessing"></param>
        /// <param name="items"></param>
        public void Enqueue(
            int queueId, 
            bool onTop,
            bool validateCallFromItemProcessing, 
            params T[] items)
        {
            if (AimedToBeDisposed || _declineEnqueueing)
                return;

            if (queueId < 0)
                queueId = 0;

            Validator.CheckNull(items);
            Validator.CheckIntegerRange(queueId, 0, _queues.Length);

            if (validateCallFromItemProcessing)
            {
                if (_processingThread != null &&
                    // id of the thread calling the Enqueue
                    Thread.CurrentThread.ManagedThreadId ==
                    _processingThread.Thread.ManagedThreadId)
                {
                    // means the enqueue is called within the ItemProcessing

                    if (onTop)
                        Array.Reverse(items);

                    foreach (T item in items)
                    {
                        ItemCarrier<T> ic = new ItemCarrier<T>(item, DEFAULT_PRIORITY, GetNextProcessingId());

                        PerformSingleItemProcessing(ic);
                    }

                    return;
                }

            }

            lock (_queuesSyncRoot)
            {
                LinkedList<ItemCarrier<T>> queue = _queues[queueId];

                ApplyLimitConstraints(queue, onTop);

                foreach (var item in items)
                {
                    ItemCarrier<T> ic = new ItemCarrier<T>(item, DEFAULT_PRIORITY, GetNextProcessingId());

                    if (onTop)
                        queue.AddFirst(ic);
                    else
                        queue.AddLast(ic);
                }

                _nonEmptyMutex.Set();
            }

            EnsureProcessingThread(); 
            
        }*/

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="onTop"></param>
        /// <param name="items"></param>
        public void Enqueue(int queueId, bool onTop, params T[] items)
        {
            EnqueueMulti(null, queueId, onTop ? PQEnqueueFlags.OnTop : PQEnqueueFlags.None, items);
        }*/

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="flags"></param>
        /// <param name="items"></param>
        public void Enqueue(int queueId, 
            PQEnqueueFlags flags, 
            ICollection<T> items)
        {
            EnqueueMulti(null, queueId, flags, items);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="flags"></param>
        /// <param name="items"></param>
        public void EnqueueByKey(
            object queueKey, 
            PQEnqueueFlags flags, 
            ICollection<T> items)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            EnqueueMulti(queueKey, DEFAULT_QUEUE_ID, flags, items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="queueId"></param>
        /// <param name="flags"></param>
        /// <param name="items"></param>
        private void EnqueueMulti(
            object queueKey,
            int queueId, 
            PQEnqueueFlags flags, 
            ICollection<T> items)
        {
            if (AimedToBeDisposed || DeclineEnqueueing)
                return;

            if (items == null || items.Count == 0)
                return;

            if (ReferenceEquals(queueKey, null))
            {

                if (queueId < 0)
                    queueId = 0;

                Validator.CheckIntegerRange(queueId, 0, CountOfQueues);
            }

            var f = new PQEnqueueFlagsParsed(flags);

            if (f.validateCallFromItemProcessing)
            {                
                if (// id of the thread calling the Enqueue
                    Thread.CurrentThread.ManagedThreadId ==
                    _processingThreadIdSnapshot) // this id snapshot can cover the situation when custom thread pool is used
                {
                    // means the enqueue is called within the ItemProcessing

                    if (f.onTop)
                    {
                        if (!(items is T[]))
                        {
                            T[] tmp = new T[items.Count];
                            items.CopyTo(tmp, 0);
                            Array.Reverse(tmp);
                            items = tmp;
                        }
                        else
                            Array.Reverse((T[])items);
                    }

                    if (ReferenceEquals(queueKey,null))
                        GetQueueById(queueId, out queueKey);
                    else if (null == GetQueueByKey(queueKey))
                        return;

                    foreach (var item in items)
                    {
                        ItemCarrier<T> ic = PrepareItemCarrier(item, DefaultPriority, f.expectedWaiting);

                        PerformSingleItemProcessing(queueKey, ic);
                    }

                    return;
                }

            }

            lock (_queuesSyncRoot)
            {
                LevelSortedQueue<ItemCarrier<T>> queue;

                if (ReferenceEquals(queueKey, null))
                    queue = GetQueueById(queueId, out queueKey);
                else
                    queue = GetQueueByKey(queueKey);

                if (null == queue)
                    return;

                if (f.enqueueOnlyIfEmpty && queue.Count > 0)
                    return;

                ApplyLimitConstraints(queue, f.onTop);

                foreach (T item in items)
                {
                    ItemCarrier<T> ic = PrepareItemCarrier(item, DefaultPriority, f.expectedWaiting);

                    queue.Enqueue(ic, DefaultPriority, f.onTop);
                    /*if (onTop)
                        queue.AddFirst(ic);
                    else
                        queue.AddLast(ic);*/

                    ic.ProcessingState = PQProcessingState.Enqueued;
                }

                UnlockDueQueuesNotEmpty();
            }

            EnsureProcessingThread(); 
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="onTop"></param>
        /// <param name="items"></param>
        public void Enqueue(int queueId, bool onTop, ICollection<T> items)
        {
            EnqueueMulti(null, queueId, onTop ? PQEnqueueFlags.OnTop : PQEnqueueFlags.None, items);
        }*/

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueId"></param>
        /// <param name="items"></param>
        public void Enqueue(int queueId, params T[] items)
        {
            EnqueueMulti(null, queueId, PQEnqueueFlags.None, items);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Enqueue(ICollection<T> items)
        {
            EnqueueMulti(null, DEFAULT_QUEUE_ID, PQEnqueueFlags.None, items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Enqueue(params T[] items)
        {
            EnqueueMulti(null, DEFAULT_QUEUE_ID, PQEnqueueFlags.None, items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void EnqueueOrExecute(params T[] items)
        {
            EnqueueMulti(null, DEFAULT_QUEUE_ID, PQEnqueueFlags.ValidateCallFromItemProcessing, items);
        }
        #endregion

        /// <summary>
        /// main thread responsible for dequeuing and processing of the items
        /// </summary>
        private volatile Threads.SafeThread _processingThread = null;

        private volatile int _processingThreadIdSnapshot = -1;

        private readonly PQMode _mode = PQMode.Synchronous;
        private readonly PQPickQueueMode _queuePickMode = PQPickQueueMode.RoundRobin;
        private readonly bool _useThreadPool;
        private volatile object _threadAccessSync = new object();

        private volatile ThreadPriority _threadPriority;
        /// <summary>
        /// ThreadPriority assigned to MainProcessingThread loop
        /// </summary>
        public ThreadPriority ProcessingThreadPriority
        {
            get
            {
                try
                {
                    lock (_threadAccessSync)
                    {
                        if (null != _processingThread)
                            return _processingThread.Thread.Priority;
                        
                        return _threadPriority;
                    }
                }
                catch(NullReferenceException) 
                {
                    return ThreadPriority.Normal;
                }
            }
            set
            {
                try
                {
                    lock (_threadAccessSync)
                    {
                        _threadPriority = value;
                        if (null != _processingThread)
                            _processingThread.Thread.Priority = _threadPriority;
                    }
                }
                catch(NullReferenceException)
                {
                }
            }
        }

// ReSharper disable once NotAccessedField.Local
        private readonly string _threadNamePrefix = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="queueKeys">if less than zero, or zero , it'll create single queue</param>
        /// <param name="queuePickMode">relevant for processing queue with more than one queue</param>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority">priority of the processing thread</param>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(
            PQMode mode, 
            ICollection queueKeys, 
            PQPickQueueMode queuePickMode, 
// ReSharper disable once UnusedParameter.Local
            bool useThreadPool, 
            ThreadPriority threadPriority, 
            string threadNamePrefix)
        {
            _mode = mode;

            int numberOfQueues = 1;
            if (queueKeys != null && queueKeys.Count > 0)
            {
                int i = 0;
                foreach(var qk in queueKeys)
                {
                    if (qk == null)
                        throw new ArgumentNullException("queueKeys", "Queue key on index " + i + " is null");

                    i++;
                }

                numberOfQueues = queueKeys.Count;
            }
            else
                queueKeys = new object[]{SINGLE_QUEUE_IDENTIFIER};

            _queuePickMode = queuePickMode;

            // TODO : thread pool is producing troubles in Async mode, manually disabled
            _useThreadPool = false; //useThreadPool;

            _threadPriority = threadPriority;

            #region bound structures
            _queues =
                new SyncDictionary<object, LevelSortedQueue<ItemCarrier<T>>>(numberOfQueues);

            // no synchronization here, constructor
            _queueKeys = new object[queueKeys.Count];
            int k = 0;
            foreach (var qk in queueKeys)
            {
                _queueKeys[k++] = qk;
            }
            _queueKeysSnapshot = (object[])_queueKeys.Clone();

            foreach(object queueKey in queueKeys)
            {
                _queues[queueKey] = new LevelSortedQueue<ItemCarrier<T>>(DefaultPriority);
            }
            #endregion

            if (!string.IsNullOrEmpty(threadNamePrefix))
                _threadNamePrefix = threadNamePrefix;
            else
                _threadNamePrefix = null;

            EnsureProcessingThread();
        }

        /* 
         * UNNECESSARY OVERLOAD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="queueKeys"></param>
        /// <param name="queuePickMode"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        public ProcessingQueue(PQMode mode, object[] queueKeys, PQPickQueueMode queuePickMode, bool useThreadPool, ThreadPriority threadPriority)
            :this(mode, queueKeys, queuePickMode, useThreadPool, threadPriority, null)
        {
        }
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="queueKeys"></param>
        /// <param name="queuePickMode"></param>
        public ProcessingQueue(PQMode mode, ICollection queueKeys, PQPickQueueMode queuePickMode)
            :this(mode,queueKeys,queuePickMode,false,ThreadPriority.Normal,null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        public ProcessingQueue(PQMode mode, bool useThreadPool,ThreadPriority threadPriority)
            : this(mode, null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(PQMode mode, bool useThreadPool, ThreadPriority threadPriority,string threadNamePrefix)
            : this(mode, null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, threadNamePrefix)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public ProcessingQueue(PQMode mode)
            : this(mode, null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, null)
        {
        }

        /// <summary>
        /// most common used Synchronous Single sub-queue processing queue with ThreadPriority.Normal thread
        /// </summary>
        public ProcessingQueue()
            : this(PQMode.Synchronous, null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(string threadNamePrefix)
            : this(PQMode.Synchronous, null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, threadNamePrefix)
        {

        }

        

        /// <summary>
        /// starts the main processing thread
        /// </summary>
        private void EnsureProcessingThread()
        {
            if (null == _processingThread) // double lock checking, to allow validation of the thread on every Enqueue call without necessity to lock every time
                lock (_threadAccessSync)
                {
                    if (null == _processingThread)
                    {
                        Threads.SafeThread processingThread;

#if COMPACT_FRAMEWORK
                        string name = "ProcessingQueue<" + typeof (T) + ">";
                        if (_threadNamePrefix != null)
                            name = _threadNamePrefix + StringConstants.SPACE + name;

                        processingThread = new Threads.SafeThread(
                            MainProcessingThread,
                            _threadPriority, 
                            _useThreadPool,
                            name
                            );
#else


                        processingThread = new Threads.SafeThread(
                            MainProcessingThread,
                            _threadPriority, 
                            _useThreadPool);
#endif
                        processingThread.Start();

                        if (!_useThreadPool)
                            _processingThreadIdSnapshot = processingThread.Thread.ManagedThreadId;

                        _processingThread = processingThread;
                    }
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        public bool EnsureQueue(object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            lock (_queuesSyncRoot)
            {
#if DEBUG
                if (_queueKeys.Length != _queues.Count)
                {
                    // should not happen
                    Debugger.Break();
                }
#endif

                LevelSortedQueue<ItemCarrier<T>> q;
                return _queues.GetOrAddValue(
                    queueKey,
                    out q,
                    key =>
                    {
                        var queue = new LevelSortedQueue<ItemCarrier<T>>(DefaultPriority);
                        RegisterQueueKey(queueKey);
                        return queue;
                    });
            }
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        public void AddQueue(object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            if (_queues.ContainsKey(queueKey))
                throw new AlreadyExistsException(queueKey);

            lock (_queuesSyncRoot)
            {
#if DEBUG
                if (_queueKeys.Length != _queues.Count)
                {
                    // should not happen
                    Debugger.Break();
                }
#endif

                var queue = new LevelSortedQueue<ItemCarrier<T>>(DefaultPriority);
                _queues[queueKey] = queue;
                RegisterQueueKey(queueKey);
            }
        }

        private void RegisterQueueKey(object queueKey)
        {

            // NOT GOOD IDEA, THIS'LL CHANGE ORDER OF THE QUEUES
            //_queueKeys = _queues.KeysSnapshot;

            Array.Resize(ref _queueKeys, _queueKeys.Length + 1);
            // at the last postion
            _queueKeys[_queueKeys.Length - 1] = queueKey;

            _queueKeysSnapshot = (object[]) _queueKeys.Clone();
        }

        private int QueueKey2QueueId(object queueKey)
        {
            int found = -1;
            for (int i = 0; i < _queueKeys.Length; i++)
            {
                if (ReferenceEquals(_queueKeys[i], queueKey) ||
                    _queueKeys[i].Equals(queueKey))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        public bool RemoveQueue(object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            lock (_queuesSyncRoot)
            {
#if DEBUG
                if (_queueKeys.Length != _queues.Count)
                {
                    // should not happen
                    Debugger.Break();
                }
#endif

                LevelSortedQueue<ItemCarrier<T>> queue;
                if (_queues.Remove(queueKey, out queue))
                {
                    queue.Clear();

                    int found = QueueKey2QueueId(queueKey);
                    
                    if (found >= 0)
                    {
                        // shrinking the array at position found
                        for (int j = found; j < _queueKeys.Length - 1; j++)
                        {
                            _queueKeys[j] = _queueKeys[j + 1];
                        }

                        Array.Resize(ref _queueKeys, _queueKeys.Length - 1);
                    }
                    else
                        // should not happen
                        Debugger.Break();

                    _queueKeysSnapshot = (object[])_queueKeys.Clone();

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event DType2Void<T> ItemProcessing;
        /// <summary>
        /// 
        /// </summary>
        public event D2Type2Void<object,T> MultiQueueItemProcessing;
        /// <summary>
        /// 
        /// </summary>
        public event D3Type2Void<T, bool, object> ItemProcessingResult;
        /// <summary>
        /// 
        /// </summary>
        public event D2Type2Void<T, Exception> ItemProcessingError;
        /// <summary>
        /// 
        /// </summary>
        public event DType2Void<T> ItemProcessingTimedOut;
        /// <summary>
        /// 
        /// </summary>
        public event DType2Type<T, object> IdleItemToBeGenerated;

        private bool _idleItemsGenerated = false;
        /// <summary>
        /// defines whether peeking thread will try to wait when queue(s) are empty
        /// or will try to generate idle items and process those
        /// </summary>
        public bool IdleItemsGenerated
        {
            get
            {
                return _idleItemsGenerated;
            }
            set
            {
                if (value && _idleGeneratingDelay < 1)
                {
                    throw new ArgumentException("If IDLE generation is turned on, the IdleGeneratingDelay property should be set to non-zero to avoid CPU process burst");
                }

                _idleItemsGenerated = value;
            }
        }

        private volatile bool _idleItemsPreventNull = false;
        /// <summary>
        /// if true, does not allow null values to be returned from IdleItemsToBeGenerated delegate
        /// </summary>
        public bool IdleItemsPreventNull
        {
            get { return _idleItemsPreventNull; }
            set { _idleItemsPreventNull = value; }
        }
        

        private volatile object _endItemProcessingLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool EndItemProcessing(bool success, object state)
        {
            T itemRef = default(T);

            return EndItemProcessing(success, state, true, ref itemRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="state"></param>
        /// <param name="releaseAfterResult"></param>
        /// <param name="itemOutput"></param>
        /// <returns>true, if end of processing started before timeout occured</returns>
        public bool EndItemProcessing(bool success, object state,bool releaseAfterResult,ref T itemOutput)
        {
            if (AimedToBeDisposed)
            {
                return false;
            }

            bool itemProcessingAlreadyTimedOut;

            //Stopwatch sw = Stopwatch.StartNew();

            lock (_endItemProcessingLock)
            {
                _asyncMutexEndItemProcessingStarted.Set();
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //sw.Start();

                // if waitone with zero is true, it was not blocking before, so it was released by timeout
                itemProcessingAlreadyTimedOut = _asyncMutexFinal.WaitOne(0, false);
                
                if (AimedToBeDisposed)
                    return false;

                if (!itemProcessingAlreadyTimedOut)
                {
                    bool assigned = false;
                    T item = default(T);

                    if (_actualItemCarrier != null)
                        lock (_actualItemCarrierSync)
                        {
                            if (_actualItemCarrier != null)
                            {
                                item = _actualItemCarrier._item;
                                assigned = true;

                                //ItemCarrier<T> icTmp = _actualItemCarrier;

                                try
                                {
                                    _actualItemCarrier.ProcessingState = PQProcessingState.Processed;
                                }
                                catch
                                {
                                }
                                _actualItemCarrier = null;
#if DEBUG
                                /*if (typeof(T).Name.Contains("NodeDataObject"))
                                {
                                    Console.WriteLine("WARN: AIC null EndItemProc("+this.GetHashCode()+"/"+icTmp._processingId+") ");
                                }*/
#endif
                            }
                        }

                    if (assigned)
                    {
                        /*
                        DateTime time = DateTime.Now;
                        string tmpstr = item == null ? "NULL" : item.ToString();
                        lock (GeneralLock._sync)
                        {
                            System.Diagnostics.Debug.WriteLine("END ITEM (" + time.ToString("mm.ss.fffffff") + ") " + tmpstr + "; " + this.GetHashCode().ToString());
                        }
                        */

                        if (!releaseAfterResult)
                            _asyncMutexFinal.Set();

                        itemOutput = item;

                        if (null != ItemProcessingResult)
                            try
                            {
                                ItemProcessingResult(item, success, state);
                            }
                            catch
                            {
                            }

                        if (releaseAfterResult)
                            _asyncMutexFinal.Set();
                    }

                }
                /*
            else
            {
                DateTime time = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("Already timed out(" + time.ToString("mm.ss.ffffff") + "); " + this.GetHashCode());
            }
            */
            }

            
            /*long took = sw.ElapsedMilliseconds;

            if (took > 50)
            {
                took *= 10;
            }*/

            

            return !itemProcessingAlreadyTimedOut;
        }

        /// <summary>
        /// implies object == null
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public bool EndItemProcessing(bool success)
        {
            T itemRef = default(T);
            return EndItemProcessing(success, null,true,ref itemRef);
        }

        /// <summary>
        /// implies success == true
        /// </summary>
        /// <returns></returns>
        public bool EndItemProcessing()
        {
            T itemRef = default(T);
            return EndItemProcessing(true, null, true, ref itemRef);
        }

        /// <summary>
        /// points to item carrier, that is currently in processing
        /// </summary>
        /// 
        private volatile ItemCarrier<T> _actualItemCarrier = null;

        /// <summary>
        /// ensures synchronized access for _actualItemCarrier via locks
        /// </summary>
        private volatile object _actualItemCarrierSync = new object();
        
        /// <summary>
        /// returns the item that is currently in processing, or default(T) if there is no item in processing
        /// </summary>
        public T ActualItem
        {
            get
            {
                T item = default(T);

                if (_actualItemCarrier != null)
                    lock (_actualItemCarrierSync)
                    {
                        if (_actualItemCarrier != null)
                            item = _actualItemCarrier._item;
                    }
                
                return item;
            }
        }

        /// <summary>
        /// more precise method to get ActualItem, because in some cases default(T) can be used as normal value;
        /// in this method the returned bool defines, if the ActualItem is defined or not
        /// </summary>
        /// <param name="itemOutput"></param>
        /// <returns></returns>
        public bool TryGetActualItem(ref T itemOutput)
        {
            if (_actualItemCarrier == null)
                return false;
            
            lock (_actualItemCarrierSync)
            {
                if (_actualItemCarrier != null)
                {
                    itemOutput = _actualItemCarrier._item;
                    return true;
                }
                
                return false;
            }
        }

        /// <summary>
        /// single item processing
        /// </summary>
        /// <returns>true, if ItemProcessing passed without raising exception ; 
        /// false in cases of Exception or ItemProcessing is not bound, thus null</returns>
        /// <param name="queueKey"></param>
        /// <param name="itemCarrier"></param>
        private bool PerformSingleItemProcessing(object queueKey, ItemCarrier<T> itemCarrier)
        {
            bool result = false;

            if (null != ItemProcessing || null != MultiQueueItemProcessing)
            {
                try
                {
                    if (!AimedToBeDisposed)
                    {
                        lock (_actualItemCarrierSync)
                        {
                            _actualItemCarrier = itemCarrier;
                            itemCarrier.ProcessingState = PQProcessingState.Processing;
                        }

                        // not expecting to call both handlers
                        if (null != ItemProcessing)
                            ItemProcessing(itemCarrier._item);

                        if ( null != MultiQueueItemProcessing)
                            MultiQueueItemProcessing(queueKey, itemCarrier._item);

                        result = true;
                    }
                }
                catch (ThreadAbortException tae)
                {
                    if (tae.ExceptionState != Threads.InternalThreadAbortKey.Singleton)
                    {
                        
#if !COMPACT_FRAMEWORK
                        try { Thread.ResetAbort(); }
                        catch { }
                        Debug.Assert(false,"Somebody is trying to stop peeking thread from inside of the processing");
#endif
                    }
                }
                catch (Exception e)
                {
                    if (null != ItemProcessingError)
                        try { ItemProcessingError(itemCarrier._item, e); }
                        catch(Exception error)
                        {
                            Sys.HandledExceptionAdapter.Examine(error);
                        }
                }

                if (_mode == PQMode.Synchronous)
                    lock (_actualItemCarrierSync)
                    {
                        try
                        {
                            _actualItemCarrier.ProcessingState = PQProcessingState.Processed;
                        }
                        catch
                        {
                            
                        }
                        _actualItemCarrier = null;

                    }
            }

            return result;
        }

        private readonly ManualResetEvent _asyncMutexFinal = new ManualResetEvent(true);
        private readonly ManualResetEvent _asyncMutexEndItemProcessingStarted = new ManualResetEvent(true);

        private volatile bool _noTimeoutOnProcessingFailure = false;
        /// <summary>
        /// 
        /// </summary>
        public bool NoTimeoutOnProcessingFailure
        {
            get
            {
                return _noTimeoutOnProcessingFailure;
            }
            set
            {
                _noTimeoutOnProcessingFailure = value;
            }
        }

        /// <summary>
        /// processes dequeued item and its timeouts if async mode
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="itemCarrier"></param>
        private void ProcessItemAndTimeouts(object queueKey,ItemCarrier<T> itemCarrier)
        {
            int postDelay = _postProcessingDelay;

            try
            {
                switch (_mode)
                {
                    case PQMode.Asynchronous:
                        _asyncMutexEndItemProcessingStarted.Reset();
                        _asyncMutexFinal.Reset();
                        
                        // main processing
                        bool itemProcessingResult = PerformSingleItemProcessing(queueKey,itemCarrier);

                        // if _asynchronousTimeout is set to 0 or negative
                        // it means it's not used
                        if (_asynchronousTimeout > 0 &&
                            // this condition will block starting async timeout in cases ItemProcessing
                            // stopped by any exception and NoTimeoutOnProcessingFailure is set to true
                            ((_noTimeoutOnProcessingFailure && itemProcessingResult) || !_noTimeoutOnProcessingFailure))
                        {
                            try
                            {
                                bool asyncMutexFinalResult = _asyncMutexFinal.WaitOne(_asynchronousTimeout, false);

                                if (AimedToBeDisposed)
                                    // don't block here too long, the request queue is in process to be disposed
                                    return;


                                if (!asyncMutexFinalResult)
                                {  
                                    lock (_endItemProcessingLock)
                                    {
                                        // THIS BRANCH MEANS TIMEOUT
                                        _asyncMutexFinal.Set();
                                        // set this ASAP, so potential EndItemProcessing would know about it
                                        // if the mutex is already set by started EndItemProcessing does not really matter, as the further mutex checking will verify it

                                        bool endItemProcessingAlreadyStarted = _asyncMutexEndItemProcessingStarted.WaitOne(0, false);

                                        if (AimedToBeDisposed)
                                            // don't block here too long, the request queue is in process to be disposed
                                            return;

                                        if (!endItemProcessingAlreadyStarted)
                                        {
                                            // this means timeout, as EndItemProcessing did NOT YET released the _asyncMutexFinal
                                            // and also that EndItemProcessing DID NOT STARTED YET

                                            /*
                                            if (_actualItem != null)
                                                System.Diagnostics.Debug.WriteLine("TIMEOUT: " + _actualItem.ToString() + "; " + this.GetHashCode().ToString());
                                            else
                                                System.Diagnostics.Debug.WriteLine("ITEM TIMEOUT but NULL; " + this.GetHashCode().ToString());
                                            */

                                            lock (_actualItemCarrierSync)
                                            {
                                                try
                                                {
                                                    _actualItemCarrier.ProcessingState = PQProcessingState.Processed;
                                                }
                                                catch
                                                {
                                                }
                                                _actualItemCarrier = null;
                                            }

                                            // if waiting for processing end timed out
                                            if (null != ItemProcessingTimedOut)
                                                try { ItemProcessingTimedOut(itemCarrier._item); }
                                                catch { }
                                        }
#if DEBUG_PROCESSING
                                        else
                                            Console.WriteLine(
                                                Thread.CurrentThread.ManagedThreadId.ToString("X8") + StringConstants.SPACE +
                                                DateTime.Now.ToString("hh:mm:ss.fff") +
                                                " End item processing did not started yet");

#endif
                                    }
                                }
                                else
                                {
                                    // if NO timeout occurs and post-success delay is defined
                                    if (_postSuccessDelay > 0 && _postSuccessDelay > postDelay)
                                        postDelay = _postSuccessDelay;
                                }
                            }
                            catch (ThreadAbortException)
                            {
                            }
                        }
                        else
                        {
                            // wait infinitely
                            _asyncMutexFinal.WaitOne();

                            if (AimedToBeDisposed)
                                // don't block here too long, the request queue is in process to be disposed
                                return;
                        }

                        break;
                    case PQMode.Synchronous:

                        PerformSingleItemProcessing(queueKey,itemCarrier);

                        break;
                }

                if (postDelay > 0)
                    Thread.Sleep(postDelay);
            }
            catch
            {
            }

        }

        private const int MINIMAL_DISPOSAL_TIMEOUT = 100;
        private const int DEFAULT_DISPOSAL_TIMEOUT = 2000;

        private int _disposalTimeout = DEFAULT_DISPOSAL_TIMEOUT;
        /// <summary>
        /// defines amount of time (by default 2000ms) that Dispose/InternalDispose will wait for MainProcessingThread to finish/Join,
        /// if timeout not fullfiled, MainProcessingThread is aborted
        /// </summary>
        public int DisposalTimeout
        {
            get
            {
                return _disposalTimeout;
            }
            set
            {
                if (value > MINIMAL_DISPOSAL_TIMEOUT)
                    _disposalTimeout = value;
                else
                    _disposalTimeout = MINIMAL_DISPOSAL_TIMEOUT;
            }
        }
        

        private int _lastQueueId;
        private Random _queueRandomizer = null;

        /// <summary>
        /// 
        /// </summary>
        private void QueuePickingIntialization()
        {
            switch (_queuePickMode)
            {
                case PQPickQueueMode.Random:
                    if (null == _queueRandomizer)
                        _queueRandomizer = new Random((int)DateTime.Now.Ticks);
                    break;
                case PQPickQueueMode.FlushQueueFirst:
                    _lastQueueId = 0;
                    break;
                //case ProcessingQueue<T>.QueuePickMode.RoundRobin:
                default:
                    _lastQueueId = -1;
                    break;
            }
        }

        /// <summary>
        /// picks a queue according to QueuePickMode and their count of items,
        /// also outs the index of the queue ; 
        /// 
        /// MUST BE LOCKED OUTSIDE by lock(_queueSyncRoot)
        /// </summary>
        /// <returns></returns>
        private LevelSortedQueue<ItemCarrier<T>> PickQueueByMode(
            out int queueId,
            out object queueKey)
        {
            LevelSortedQueue<ItemCarrier<T>> queue;
            
            if (_queues.Count > 1)
            {
                // queue decision
                switch (_queuePickMode)
                {
                    case PQPickQueueMode.FlushQueueFirst:
                    {
                            if (_lastQueueId >= _queueKeys.Length)
                                _lastQueueId = 0;

                            queueKey = _queueKeys[_lastQueueId];

                            if (_queues[queueKey].Count == 0)
                            {
                                if (_lastQueueId < _queueKeys.Length - 1)
                                    queueId = ++_lastQueueId;
                                else
                                    queueId = _lastQueueId = 0;
                            }
                            else
                                queueId = _lastQueueId;
                        }
                        break;
                    case PQPickQueueMode.Random:
                        queueId = _queueRandomizer.Next(_queueKeys.Length);
                        queueKey = _queueKeys[queueId];
                        break;
                        //case QueuePickMode.RoundRobin:
                    default:
                        {
                            if (_lastQueueId < _queueKeys.Length - 1)
                                queueId = ++_lastQueueId;
                            else
                                queueId = _lastQueueId = 0;

                            queueKey = _queueKeys[queueId];
                        }
                        break;
                }

                queue = _queues[queueKey];
            }
            else
            {
                queueKey = _queueKeys.FirstOrDefault();
                queueId = 0;
                queue = _queues.Values.FirstOrDefault();
            }

            return queue;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void WaitForQueuesNotBeingEmpty()
        {
            _queues.WaitWhileEmpty();

            base.WaitForQueuesNotBeingEmpty();
        }

        /// <summary>
        /// main loop of the ProcessingQueue
        /// </summary>
        private void MainProcessingThread()
        {
            try
            {
                QueuePickingIntialization();

                while (!AimedToBeDisposed)
                {
                    //Debug.WriteLine("PQ Cycling : " + _idleItemsGenerated);

                    if (null == IdleItemToBeGenerated || !_idleItemsGenerated)
                    {
                        // if no IDLE handling is defined, try to wait here infinitely
                        //_nonEmptyMutex.WaitOne(-1, false);
                        WaitForQueuesNotBeingEmpty();

                        if (AimedToBeDisposed)
                        {
                            //Console.WriteLine("Breaking ...");
                            break;
                        }
                    }
                   

                    ItemCarrier<T> itemCarrier;
                    bool dequeued;

                    object queueKey;

                    lock (_queuesSyncRoot)
                    {
                        // queueId shall not be used until necessary
                        int queueId;

                        var queue = PickQueueByMode(out queueId,out queueKey);
                        if (queue == null) {
                            LockDueQueuesEmpty();
                            continue;
                        }

                        dequeued = queue.Dequeue(out itemCarrier);

                        if (!dequeued)
                        {
                            bool lockEmptyMutex;

                            if (CountOfQueues <= 1)
                            {
                                // locking of the processing, until some new item will be added to this single queue
                                lockEmptyMutex = true;
                            }
                            else
                            {
                                lockEmptyMutex = true;
                                foreach(var queueToCompare in _queues.Values)
                                {
                                    if (ReferenceEquals(queueToCompare, queue))
                                        // do not compare queue count with it's own count)
                                        continue;

                                    if (queueToCompare != null && queueToCompare.Count > 0)
                                    {
                                        lockEmptyMutex = false;
                                        // if there is at least one other non-empty queue, do not lock the processing
                                        break;
                                    }
                                }
                            }

                            if (lockEmptyMutex)
                            {
                                if (!AimedToBeDisposed)
                                {
                                    LockDueQueuesEmpty();
                                    //Debug.WriteLine("NEM Reset");
                                }
                            }

                            // ALLOW IDLE PROCESSING
                            //continue;
                        }
                    }

                    if (dequeued)
                    {
                        ProcessItemAndTimeouts(queueKey, itemCarrier);
                    }
                    else
                    {
                        if (null != IdleItemToBeGenerated && _idleItemsGenerated)
                        {
                            try
                            {   
                                T generatedItem = IdleItemToBeGenerated(queueKey);

                                if (!ReferenceEquals(generatedItem,null) ||
                                    !_idleItemsPreventNull)
                                {
                                    itemCarrier = PrepareItemCarrier(generatedItem, DefaultPriority, false);
                                    
                                    ProcessItemAndTimeouts(queueKey, itemCarrier);
                                }

                                // special kind of post-delay
                                if (_idleGeneratingDelay > 0)
                                    Thread.Sleep(_idleGeneratingDelay);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
#if DEBUG_PROCESSING
                Console.WriteLine(
                    Thread.CurrentThread.ManagedThreadId.ToString("X8") + StringConstants.SPACE +
                    DateTime.Now.ToString("hh:mm:ss.fff")+
                    " Ending PeekingThread FORCEFULY " + typeof(T)+ " idleGeneration=" +_idleItemsGenerated);
#endif
            }
            catch
            {
            }
            finally
            {    
                // DO NOT LOCK HERE, simple sync done by volatile declaration
                _processingThread = null; // this must be in finally section, as also ThreadAbortException can occur
                _processingThreadIdSnapshot = -1;
            }

#if DEBUG_PROCESSING
            Console.WriteLine(
                Thread.CurrentThread.ManagedThreadId.ToString("X8") + StringConstants.SPACE +
                DateTime.Now.ToString("hh:mm:ss.fff") +
                " Ending PeekingThread gracefuly " + typeof(T) + " idleGeneration=" + _idleItemsGenerated);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (isExplicitDispose &&
                !_useThreadPool &&
                _processingThreadIdSnapshot == Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("Unable to call Dispose within ItemProcessing");

            try
            {
                // turning AimedToBeDisposed to true, that precedes this call ensures,
                // that all mutexes will look into this bool first
                
                _idleItemsGenerated = false;

                // the queues should be cleared before the mutex about emptyness is released
                InternalClear(false);

                // release mutexes, that might be pending the process
                try
                {
                    bool success = UnlockDueQueuesNotEmpty();

                    DebugHelper.NOP(success);
                }
                catch
                {
                }

                // Debug.WriteLine("Disposal NEM "+typeof(T)+" Set");

                if (_mode == PQMode.Asynchronous)
                {
                    try
                    {
                        _asyncMutexFinal.Set();
                    }
                    catch
                    {
                    }

                    try
                    {
                        _asyncMutexEndItemProcessingStarted.Set();
                    }
                    catch
                    {
                    }
                }

                if (null != _processingThread)
                    lock (_threadAccessSync)
                    {
                        if (null != _processingThread)
                        {
#if !COMPACT_FRAMEWORK
                            Threads.SafeThreadStopResolution stsr = _processingThread.Stop(_disposalTimeout);
#else
                            _processingThread.Stop(_disposalTimeout);
#endif

#if DEBUG && !COMPACT_FRAMEWORK
                            if (stsr == Threads.SafeThreadStopResolution.StoppedForcefuly ||
                                stsr == Threads.SafeThreadStopResolution.DidNotStopYet ||
                                stsr == Threads.SafeThreadStopResolution.StopFailed ||
                                stsr == Threads.SafeThreadStopResolution.NotStarted ||
                                stsr == Threads.SafeThreadStopResolution.AbortNotAllowed)
                            {
                                string tmp = "Stopping processing queue : " + stsr + " : " + typeof(T);
                                Debug.WriteLine(tmp);

                                if (_disposalTimeout >= DEFAULT_DISPOSAL_TIMEOUT &&
                                    (stsr == Threads.SafeThreadStopResolution.StoppedForcefuly ||
                                    stsr == Threads.SafeThreadStopResolution.DidNotStopYet ||
                                    stsr == Threads.SafeThreadStopResolution.AbortNotAllowed ||
                                    stsr == Threads.SafeThreadStopResolution.StopFailed
                                    ))
                                    Debug.Assert(false, "Possibly a deadlock between item processing and call of Dispose\n" +
                                        tmp);
                            }
#endif
                        }
                    }
                
               
            }
            catch(ThreadAbortException)
            {
            }
        }

        /// <summary>
        /// count of all items in all queues
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                lock (_queuesSyncRoot)
                {
                    foreach (var q in _queues.Values)
                        count += q.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CountOfQueues
        {
            get
            {
                lock (_queuesSyncRoot)
                {
                    return _queues.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object[] QueueKeys
        {
            get { return _queueKeysSnapshot; }
        }

        /// <summary>
        /// returns count for specific queue
        /// </summary>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        public int GetQueueItemCount(object queueKey)
        {

            LevelSortedQueue<ItemCarrier<T>> queue;
            
            lock (_queuesSyncRoot)
            {
                // let all exceptions be thrown out
                queue = _queues[queueKey];
            }

            return queue.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        public bool IsQueuePresent(object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            return _queues.ContainsKey(queueKey);
        }

        /*
        /// <summary>
        /// returns the actual queue
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">if the id is out of the array of queues</exception>
        public IEnumerable<ItemCarrier<T>> this[int queueId]
        {
            get
            {
                Validator.CheckIntegerRange(queueId, 0, CountOfQueues);
                object queueKey;
                LevelSortedQueue<ItemCarrier<T>> queue = GetQueueById(queueId,out queueKey);
                if (null == queue)
                    return null;

                return queue.QueueSnapshot;
            }
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lockEmptyMutex">usually set to false when disposing RequestQueue</param>
        private void InternalClear(bool lockEmptyMutex)
        {
            lock (_queuesSyncRoot)
            {
                foreach (var q in _queues.Values)
                    q.Clear();

                if (lockEmptyMutex)
                    try
                    {
                        LockDueQueuesEmpty();
                    }
                    catch { }
            }
        }

        /// <summary>
        /// erases all items in all queues
        /// </summary>
        public void Clear()
        {
            InternalClear(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        public void Clear(object queueKey)
        {
            var queue = _queues[queueKey];
            if (queue != null)
                // synchronized by itself
                queue.Clear();
        }

        private volatile int _limitQueueCount = 0;
        /// <summary>
        /// if zero, no limit is applied
        /// </summary>
        public int LimitQueueCount
        {
            get
            {
                return _limitQueueCount;
            }
            set
            {
                if (value <= 0)
                    _limitQueueCount = 0;
                else
                    _limitQueueCount = value;
            }
        }

        private volatile bool _shrinkOnLimitExceeded = false;
        /// <summary>
        /// if true, and limit is exceeded, queue will be shrinked from one side or another, depending on Enqueue or EnqueueTop call
        /// 
        /// if Enqueue used and queue is to be shrinked, the first member will be withdrawn
        /// 
        /// if EnqueueTop used and queue is to be shrinked, the last member will be withdrawn
        /// </summary>
        public bool ShrinkOnLimitExceeded
        {
            get { return _shrinkOnLimitExceeded; }
            set { _shrinkOnLimitExceeded = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemInQueue"></param>
        /// <param name="itemToCompareTo"></param>
        public delegate bool DCompareItem(T itemInQueue, T itemToCompareTo);

        /*
        /// <summary>
        /// implementation yet unsynchronized
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="valueToCompareTo"></param>
        /// <returns></returns>
        ICollection<LinkedListNode<ItemCarrier<T>>> Find(DCompareItem comparer, T valueToCompareTo)
        {
            if (null == comparer)
                return null;

            LinkedList<LinkedListNode<ItemCarrier<T>>> result = new LinkedList<LinkedListNode<ItemCarrier<T>>>();

            foreach (var q in _queues) // no need to sync here, the array should not change over time
            {
                LinkedListNode<ItemCarrier<T>> node = q.First;
                //LinkedListNode<ItemCarrier<T>> previousNode = null;

                while (node != null)
                {
                    try
                    {
                        if (comparer(node.Value._item, valueToCompareTo))
                            result.AddLast(node);
                    }
                    catch
                    {
                    }


                    node = node.Next;
                }
            }

            return result;
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemInQueue"></param>
        /// <param name="itemToCompareWith"></param>
        /// <param name="remove"></param>
        public delegate void DCompareAndRemoveItem(T itemInQueue,object itemToCompareWith,out bool remove);

        /// <summary>
        /// removes the item of the queue/s by the condition implemented by user's comparer delegate
        /// </summary>
        /// <param name="comparingLambda"></param>
        /// <param name="itemToCompareWith">any state object passed into the comparer delegate for the comparing process</param>
        /// <returns>count of items removed</returns>
        public int RemoveByComparing(Func<T,object,bool> comparingLambda,object itemToCompareWith)
        {
            if (null == comparingLambda)
                return 0;

            int countRemoved = 0;

            var queueSnapshot = _queues.ValuesSnapshot;

            foreach (var q in queueSnapshot)
            {
                if (q == null)
                    continue;

                try
                {
                    countRemoved +=
                        q.Remove(
                            (item, comparingObject) =>
                            {
                                try
                                {
                                    return comparingLambda(item.Item, itemToCompareWith);
                                }
                                catch
                                {
                                    return false;
                                }
                            }

                            , itemToCompareWith);
                }
                catch
                {
                    
                }

                /*LinkedListNode<ItemCarrier<T>> node;
                LinkedListNode<ItemCarrier<T>> previousNode = null;

                lock (_queuesSyncRoot)
                {
                    node = q.First;

                    if (node != null)
                        previousNode = node.Previous;
                }

                while (node != null)
                {
                    try
                    {
                        bool remove;
                        comparer(node.Value._item, itemToCompareWith,out remove);

                        if (remove)
                        {
                            
                            lock (_queuesSyncRoot)
                            {
                                q.Remove(node);
                            }

                            countRemoved++;
                        }
                    }
                    catch
                    {
                    }
                     //   result.AddLast(node);

                    lock (_queuesSyncRoot)
                    {
                        object parentList = null;

                        try
                        {
                            parentList = node.List;
                        }
                        catch
                        {
                        }

                        if (parentList != null)
                        {
                            previousNode = node;
                            node = node.Next;
                        }
                        else
                        {
                            // this validates the situation if the current node was being removed during this loop
                            
                            if (previousNode == null)
                                // restart searching
                                node = q.First;
                            else
                            {
                                parentList = null;

                                try
                                {
                                    parentList = previousNode.List;
                                }
                                catch
                                {
                                }

                                if (parentList != null)
                                    node = previousNode.Next;
                                else
                                {
                                    // restarts searching, as even the previousNode seems to be removedAlready
                                    previousNode = null;
                                    node = q.First;
                                }
                            }
                        }
                    }
            }*/
            }

            return countRemoved;
        }

    }

    /// <summary>
    /// flags for enqueuing the items
    /// </summary>
    [Flags]
    public enum PQEnqueueFlags
    {
        /// <summary>
        /// usually used as implicit parameter without flags
        /// </summary>
        None = 0x0000,
        /// <summary>enqueuing the items at the top of the queue</summary>
        OnTop = 0x0001,
        /// <summary>
        /// 
        /// </summary>
        WithPriority = 0x0002,
        /// <summary>
        /// enqueuing will only proceed if particular queue is empty
        /// </summary>
        EnqueueOnlyIfEmpty = 0x0008,
        /// <summary>
        /// 
        /// </summary>
        ExpectedWaitingToFinish = 0x0010,
        /// <summary>
        /// 
        /// </summary>
        ValidateCallFromItemProcessing = 0x0100,
        ///
        ProhibitCallFromItemProcessing = 0x0200
    }
}
