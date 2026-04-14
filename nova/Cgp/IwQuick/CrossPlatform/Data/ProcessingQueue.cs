//#define DEBUG_PROCESSING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;
using System.Linq;

#if !COMPACT_FRAMEWORK
using System.Diagnostics;
#endif

namespace Contal.IwQuick.Data
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemCarrier<T> : APoolable<ItemCarrier<T>>, IDisposable
    {
        private int _processingId;
        internal T _item;
        internal uint _priority;

        // has to be volatile due double-locking and non-locking operations
        private volatile ManualResetEvent _finishedMutex = null;

        private readonly object _finishedMutexLock = new object();
        // just a marking, to not return the mutex if in waiting process
        // int type, because of Interlocked
        private int _finishedMutexInWaiting;

        private const int MutexNotUsedYet = -1;
        private const int MutexWaitingFinished = 0;

        private int _processingState;
        //private readonly object _processingStateLock = new object();

        protected override void BeforeGet()
        {
            _processingState = (int) PQProcessingState.Instantiated;
            _finishedMutexInWaiting = MutexNotUsedYet;
        }

        /// <summary>
        /// 
        /// </summary>
        public PQProcessingState ProcessingState
        {
            get
            {
                return (PQProcessingState)_processingState;
            }
            
        }

        internal void SetProcessingState(PQProcessingState newState)
        {

            var originalValue = Interlocked.Exchange(ref _processingState, (int)newState);

#if DEBUG
            if ((int) newState <= originalValue)
                DebugHelper.TryBreak("ItemCarrier.SetProcessingState only incrementing state");
#endif
            if (originalValue != (int) newState)
            {
                // better to release, when state is already stored
                if (_finishedMutex != null)
                    // double locking schema due fact, that another Thread
                    // can evaluate the TryReturnMutex right now by Wait call
                    lock (_finishedMutexLock)
                    {
                        if (_finishedMutex != null)
                        {
                            if (newState == PQProcessingState.Processed) 
                                _finishedMutex.Set();
                        }
                    }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectPool"></param>
        protected ItemCarrier(IObjectPool objectPool) : base(objectPool)
        {
            
        }

        /// <summary>
        /// this carrier should be only instantiated within IwQuick
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <param name="processingId"></param>
        /// <param name="allocateMutex"></param>
        internal static ItemCarrier<T> Get(T item, uint priority, int processingId, bool allocateMutex)
        {
            var itemCarrier = Get();

            itemCarrier._item = item;
            itemCarrier._priority = priority;
            itemCarrier._processingId = processingId;

            if (allocateMutex)
            {
                itemCarrier._finishedMutex = EventWaitHandlePool<ManualResetEvent>.Singleton.Get();
                itemCarrier._finishedMutex.Reset();
            }

            return itemCarrier;
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

        /// <summary>
        /// 
        /// </summary>
        private bool TryReturnMutex()
        {
            if (_finishedMutex != null)
            {
                lock (_finishedMutexLock)
                {
                    if (_finishedMutex != null &&
                        _finishedMutexInWaiting == MutexWaitingFinished) // condition preventing to return the mutex if 
                                                      // some wait is in progress OR is entering the wait
                    {
                        EventWaitHandlePool<ManualResetEvent>.Singleton.Return(_finishedMutex);
                        _finishedMutex = null;
                        return true;
                    }
                    
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool AbleToBeReturned()
        {
            return _finishedMutex == null ||
                   _finishedMutexInWaiting == MutexWaitingFinished;
        }

        protected override bool FinalizeBeforeReturn()
        {
            if (!TryReturnMutex())
                return false;
                //throw new PoolableException("Returning mutex while ItemCarrier<"+typeof(T)+"> prepared for waiting or used for waiting is not allowed") ;

            SetProcessingState(PQProcessingState.Disposed);

            _item = default (T);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Wait(int timeout)
        {
            var succeededWithoutTimeout = false;
            //bool tryReturnMutex = false;
            var mutexNotAvailable = false;

            lock (_finishedMutexLock)
            {
                // comparison and incrementing has to be in one lock due TryReturnMutex calls
                if (_finishedMutex != null)
                {
                    if (_finishedMutexInWaiting < MutexWaitingFinished)
                        _finishedMutexInWaiting = MutexWaitingFinished+1;
                    else
                        // no need to do Interlocked, cause all
                        // _finishedMutexInWaiting operations are in lock (_finishedMutexLock)
                        _finishedMutexInWaiting++; 
                }
                else
                    mutexNotAvailable = true;
            }
            

            if (mutexNotAvailable)
            {
                var psSnapshot = ProcessingState;
                if (psSnapshot == PQProcessingState.Processed ||
                    psSnapshot == PQProcessingState.Disposed)
                    // means the mutex has already been returned by the change of state
                    return true;
                
                // means _finishedMutex is null BUT not due returning after waiting
                throw new InvalidOperationException("Item was not prepared for waiting on Enqueue call");
            }

            try
            {
                _finishedMutex.WaitOne(0, false);

// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (timeout > 0)
                    succeededWithoutTimeout = _finishedMutex.WaitOne(timeout, false);
                else
                    succeededWithoutTimeout = _finishedMutex.WaitOne();
            }
            // to catch even thread aborts
            // so the _finishedMutexInWaiting counter would not stuck in inconsistent state
            catch(Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
            finally
            {
                lock (_finishedMutexLock)
                {
                    if (_finishedMutexInWaiting > MutexWaitingFinished)
                        _finishedMutexInWaiting--;
#if DEBUG
                    else
                    {
                        DebugHelper.TryBreak("ItemCarrier _finishedMutexInWaiting should not go below 0");
                    }
#endif

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

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (AbleToBeReturned())
                this.TryReturn();
        }

        ~ItemCarrier()
        {
            // DONT try to return the mutex here, it might be in inconsistent state
            try
            {
                if (_finishedMutex != null)
                    _finishedMutex.Close();
            }
            catch
            {
                
            }
        }

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
    public abstract class AProcessingQueue : ADisposable, IProcessingQueue
    {

        /// <summary>
        /// marking the item before processing
        /// </summary>
        public const int InvalidProcessingId = -1;

        /// <summary>
        /// summarizes and locks up, if all queues are empty
        /// </summary>
        private readonly ManualResetEvent _nonEmptyMutex = new ManualResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        public static readonly object SINGLE_QUEUE_IDENTIFIER = new SingleQueueIdentifier();

        protected const int DefaultQueueId = int.MinValue;
        
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
            var disposalCheck = _nonEmptyMutex.WaitOne(0, false);
            DebugHelper.Keep(disposalCheck);

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

        private const uint DefaultImplicitPriority = 0;

        private volatile uint _defaultPriority = DefaultImplicitPriority;

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

        protected volatile int _limitQueueCount = 0;
        protected volatile bool _shrinkOnLimitExceeded = false;

        /// <summary>
        /// if zero, no limit is applied
        /// </summary>
        public int LimitQueueCount
        {
            get
            {
                return _limitQueueCount;
            }
            set { _limitQueueCount = value <= 0 ? 0 : value; }
        }

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
        public abstract int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool CountIsZero { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract void Clear();

        protected volatile int _postSuccessDelay = 0;

        /// <summary>
        /// Delay between two processings if previous processing didn't end-up with timeout. By default 0.
        /// </summary>
        public int PostSuccessDelay
        {
            get { return _postSuccessDelay; }
            set { _postSuccessDelay = value > 0 ? value : 0; }
        }

        #region Processing id generating

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
        #endregion

        private static readonly LinkedList<WeakReference> _pqRegister = new LinkedList<WeakReference>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processingQueue"></param>
        /// <param name="toRegister"></param>
        protected void RegisterOrNot([NotNull] AProcessingQueue processingQueue, bool toRegister)
        {
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(processingQueue, null))
// ReSharper disable once HeuristicUnreachableCode
                return;
            try
            {
                if (toRegister)
                {
                    // not called often, small inoptimality in favor to structure's memory footprint
                    lock (_pqRegister)
                    {
                        if (!_pqRegister.Any(pq => ReferenceEquals(pq.Target, processingQueue)))
                            _pqRegister.AddLast(new WeakReference(processingQueue));
                    }
                }
                else
                {
                    lock (_pqRegister)
                    {
                        var node = _pqRegister.First;
                        while (node != null)
                        {
                            var toRemove = false;
                            if (ReferenceEquals(node.Value.Target, null))
                                toRemove = true;
                            else
                            {
                                if (ReferenceEquals(node.Value.Target, processingQueue))
                                    toRemove = true;
                            }

                            if (toRemove)
                            {
                                var tmp = node;
                                node = node.Next;
                                _pqRegister.Remove(tmp);
                            }
                            else
                                node = node.Next;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.TryBreak();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AProcessingQueue> MakeReport()
        {
            var result =  new LinkedList<AProcessingQueue>();

            lock (_pqRegister)
            {
                foreach (var weakReference in _pqRegister)
                {
                    if (weakReference != null &&
                        weakReference.Target != null)
                    {
                        result.AddLast((AProcessingQueue) weakReference.Target);
                    }
                }
            }

            return result;
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            RegisterOrNot(this,false);
        }

        protected string _toStringCached;

        
    }

    /// <summary>
    /// class with one or set of queues and processing thread that is able to call ItemProcessing event over the items
    /// 
    /// the processing queue will process the item on top of queue no-matter 
    /// how long the ItemProcessing takes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProcessingQueue<T> : AProcessingQueue, ISynchronousProcessingQueue<T>, IMultiProcessingQueue<T>
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
            set { _postProcessingDelay = value > 0 ? value : 0; }
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
            return ItemCarrier<T>.Get(
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
        private LevelSortedQueue<ItemCarrier<T>> GetQueueById(int queueId, out object queueKey)
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
        private LevelSortedQueue<ItemCarrier<T>> GetQueueByKey([NotNull] object queueKey)
        {
            lock (_queuesSyncRoot)
            {

                LevelSortedQueue<ItemCarrier<T>> queue;
                if (_queues.TryGetValue(queueKey, out queue))
                {
                    //queueId = QueueKey2QueueId(queueKey);
                    return queue;
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
            [NotNull] object queueKey,
            T item,
            PQEnqueueFlags flags,
            uint priority)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            return EnqueueSingle(queueKey, DefaultQueueId, item, flags, priority);
        }

        /// <summary>
        /// enqueues single item
        /// </summary>
        /// <param name="queueKey">if null, means the queueId identifies the queue</param>
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
            [CanBeNull] object queueKey,
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

            ValidateItemProcessingEventsAssignment();

            ItemCarrier<T> ic;
            var f = new PQEnqueueFlagsParsed(flags);

            var enqueuePriority = DefaultPriority;
            if ((flags & PQEnqueueFlags.WithPriority) == PQEnqueueFlags.WithPriority)
                enqueuePriority = priority;


            if (f.validateCallFromItemProcessing ||
                f.prohibitCallFromItemProcessing)
            {
                if ( // id of the thread calling the Enqueue
                    Thread.CurrentThread.ManagedThreadId ==
                    _processingThreadIdSnapshot)
                    // this id snapshot can cover the situation when custom thread pool is used
                {
                    if (f.prohibitCallFromItemProcessing)
                        throw new InvalidOperationException(
                            "Invalid Enqueue operation within ItemProcessing as EnqueueFlags.ProhibitCallFromItemProcessing used");

                    LevelSortedQueue<ItemCarrier<T>> queue;
// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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

                    ic = PrepareItemCarrier(item, enqueuePriority, f.expectedWaiting);

                    ProcessSingleItem(queueKey, ic,0);

                    return ic; //ic._processingId;
                }
            }

            lock (_queuesSyncRoot)
            {
                LevelSortedQueue<ItemCarrier<T>> queue;
// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (ReferenceEquals(queueKey, null))
                    queue = GetQueueById(queueId, out queueKey);
                else
                    queue = GetQueueByKey(queueKey);

                if (queue == null)
                    return null;

                if (f.enqueueOnlyIfEmpty && queue.Count > 0)
                    return null; //ItemCarrier<T>.INVALID_PROCESSING_ID;

                ApplyLimitConstraints(queue, f.onTop);

                ic = PrepareItemCarrier(item, enqueuePriority, f.expectedWaiting);

                queue.Enqueue(ic, enqueuePriority, f.onTop);

                ic.SetProcessingState(PQProcessingState.Enqueued);

                UnlockDueQueuesNotEmpty();
            }

            return ic; //ic._processingId;
        }

        private void ValidateItemProcessingEventsAssignment()
        {
            if (_itemProcessingDelegate == null && _multiQueueItemProcessingDelegate == null)
                throw new InvalidOperationException(
                    "Either ItemProcessing or MultiQueueItemProcessing method should be assigned");
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
        private void ApplyLimitConstraints([NotNull] LevelSortedQueue<ItemCarrier<T>> queue, bool onTop)
        {
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

// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Func<T, bool> predicate)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            object queueKey;
            var queue = GetQueueById(0, out queueKey);

             
            if (null != queue)
                return queue.Contains(
                    (itemInQueue, itemToCompareWith) => predicate(itemInQueue.Item),
                    null
                    );

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains([NotNull] object queueKey,Func<T, bool> predicate)
        {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            var queue = GetQueueByKey(queueKey);


            if (null != queue)
                return queue.Contains(
                    (itemInQueue, itemToCompareWith) => predicate(itemInQueue.Item),
                    null
                    );

            return false;
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
            return EnqueueSingle(null, DefaultQueueId, item, PQEnqueueFlags.None, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public ItemCarrier<T> Enqueue(T item, PQEnqueueFlags flags)
        {
            return EnqueueSingle(null, DefaultQueueId, item, flags, 0);
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
            return EnqueueSingle(null, DefaultQueueId, item, flags | PQEnqueueFlags.WithPriority, priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ItemCarrier<T> EnqueueOrExecute(T item)
        {
            return EnqueueSingle(null, DefaultQueueId, item, PQEnqueueFlags.ValidateCallFromItemProcessing, 0);
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
            return EnqueueSingle(null, DefaultQueueId, item, PQEnqueueFlags.OnTop, 0);
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
        public ItemCarrier<T> EnqueueByKey([NotNull] object queueKey, T item)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            return EnqueueSingle(queueKey, DefaultQueueId, item, PQEnqueueFlags.None, 0);
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

            Validator.CheckForNull(items);
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

                        ProcessSingleItem(ic);
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
        /// <exception cref="ArgumentNullException"></exception>
        public void EnqueueByKey(
            [NotNull] object queueKey,
            PQEnqueueFlags flags,
            ICollection<T> items)
        {
            if (ReferenceEquals(null, queueKey))
                throw new ArgumentNullException("queueKey");

            EnqueueMulti(queueKey, DefaultQueueId, flags, items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey">if null, queue is identified by queueId</param>
        /// <param name="queueId"></param>
        /// <param name="flags"></param>
        /// <param name="items"></param>
        private void EnqueueMulti(
            [CanBeNull] object queueKey,
            int queueId,
            PQEnqueueFlags flags,
            [CanBeNull] ICollection<T> items)
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

            ValidateItemProcessingEventsAssignment();

            var f = new PQEnqueueFlagsParsed(flags);

            if (f.validateCallFromItemProcessing)
            {
                if ( // id of the thread calling the Enqueue
                    Thread.CurrentThread.ManagedThreadId ==
                    _processingThreadIdSnapshot)
                    // this id snapshot can cover the situation when custom thread pool is used
                {
                    // means the enqueue is called within the ItemProcessing

                    if (f.onTop)
                    {
                        if (!(items is T[]))
                        {
                            var tmp = new T[items.Count];
                            items.CopyTo(tmp, 0);
                            Array.Reverse(tmp);
                            items = tmp;
                        }
                        else
                            Array.Reverse((T[]) items);
                    }

                    if (ReferenceEquals(queueKey, null))
                        GetQueueById(queueId, out queueKey);
                    else if (null == GetQueueByKey(queueKey))
                        return;

                    foreach (var item in items)
                    {
                        var ic = PrepareItemCarrier(item, DefaultPriority, f.expectedWaiting);

                        ProcessSingleItem(queueKey, ic,0);
                    }

                    return;
                }

            }

            lock (_queuesSyncRoot)
            {
                LevelSortedQueue<ItemCarrier<T>> queue;

// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (ReferenceEquals(queueKey, null))
                    queue = GetQueueById(queueId, out queueKey);
                else
                    queue = GetQueueByKey(queueKey);

                if (null == queue)
                    return;

                if (f.enqueueOnlyIfEmpty && queue.Count > 0)
                    return;

                ApplyLimitConstraints(queue, f.onTop);

                foreach (var item in items)
                {
                    var ic = PrepareItemCarrier(item, DefaultPriority, f.expectedWaiting);

                    queue.Enqueue(ic, DefaultPriority, f.onTop);
                    /*if (onTop)
                        queue.AddFirst(ic);
                    else
                        queue.AddLast(ic);*/

                    ic.SetProcessingState(PQProcessingState.Enqueued);
                }

                UnlockDueQueuesNotEmpty();
            }

            
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
            EnqueueMulti(null, DefaultQueueId, PQEnqueueFlags.None, items);
        }

 /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void Enqueue(params T[] items)
        {
            EnqueueMulti(null, DefaultQueueId, PQEnqueueFlags.None, items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public void EnqueueOrExecute(params T[] items)
        {
            EnqueueMulti(null, DefaultQueueId, PQEnqueueFlags.ValidateCallFromItemProcessing, items);
        }

        #endregion

        /// <summary>
        /// main thread responsible for dequeuing and processing of the items
        /// </summary>
        private volatile SafeThread _processingThread = null;

        private volatile int _processingThreadIdSnapshot = -1;

        
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
                catch (NullReferenceException)
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
                catch (NullReferenceException)
                {
                }
            }
        }

// ReSharper disable once NotAccessedField.Local
        private readonly string _threadNamePrefix = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKeys">if less than zero, or zero , it'll create single queue</param>
        /// <param name="queuePickMode">relevant for processing queue with more than one queue</param>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority">priority of the processing thread</param>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(
            [CanBeNull] IEnumerable queueKeys,
            PQPickQueueMode queuePickMode,
// ReSharper disable once UnusedParameter.Local
            bool useThreadPool,
            ThreadPriority threadPriority,
            string threadNamePrefix)
        {
            var numberOfQueues = 0;
            if (queueKeys != null)
            {
                var i = 0;
// ReSharper disable once PossibleMultipleEnumeration
                foreach (var qk in queueKeys)
                {
                    if (ReferenceEquals(qk, null))
                        throw new ArgumentNullException("queueKeys", "Queue key on index " + i + " is null");

                    i++;
                }

                numberOfQueues = i;
            }

            RegisterOrNot(this,true);

            if (numberOfQueues == 0)
            {
                queueKeys = new[] {SINGLE_QUEUE_IDENTIFIER};
                numberOfQueues = 1;
            }

            _queuePickMode = queuePickMode;

            // TODO : thread pool is producing troubles in Async mode, manually disabled
            _useThreadPool = false; //useThreadPool;

            _threadPriority = threadPriority;

            #region bound structures

            _queues =
                new SyncDictionary<object, LevelSortedQueue<ItemCarrier<T>>>(numberOfQueues);

            // no synchronization here, constructor
            _queueKeys = new object[numberOfQueues];

            var k = 0;
// ReSharper disable once PossibleMultipleEnumeration
// ReSharper disable once PossibleNullReferenceException - not relevant due {{{ if (numberOfQueues == 0)}}}
            foreach (var qk in queueKeys)
            {
                _queueKeys[k++] = qk;
                _queues[qk] = new LevelSortedQueue<ItemCarrier<T>>(DefaultPriority);
            }
            _queueKeysSnapshot = (object[]) _queueKeys.Clone();


            #endregion

            _threadNamePrefix = !string.IsNullOrEmpty(threadNamePrefix) ? threadNamePrefix : null;

            
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
        /// <param name="queueKeys"></param>
        /// <param name="queuePickMode"></param>
        public ProcessingQueue(IEnumerable queueKeys, PQPickQueueMode queuePickMode)
            : this(queueKeys, queuePickMode, false, ThreadPriority.Normal, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        public ProcessingQueue(bool useThreadPool, ThreadPriority threadPriority)
            : this(null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="useThreadPool"></param>
        /// <param name="threadPriority"></param>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(bool useThreadPool, ThreadPriority threadPriority, string threadNamePrefix)
            : this(null, PQPickQueueMode.RoundRobin, useThreadPool, threadPriority, threadNamePrefix)
        {
        }

        /// <summary>
        /// most common used Synchronous Single sub-queue processing queue with ThreadPriority.Normal thread
        /// </summary>
        public ProcessingQueue()
            : this(null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadNamePrefix"></param>
        public ProcessingQueue(string threadNamePrefix)
            : this(null, PQPickQueueMode.RoundRobin, false, ThreadPriority.Normal, threadNamePrefix)
        {

        }



        /// <summary>
        /// starts the main processing thread
        /// </summary>
        private void EnsureProcessingThread()
        {
            if (null == _processingThread)
                // double lock checking, to allow validation of the thread on every Enqueue call without necessity to lock every time
                lock (_threadAccessSync)
                {
                    if (null == _processingThread)
                    {
// ReSharper disable once JoinDeclarationAndInitializer
                        SafeThread processingThread;

#if COMPACT_FRAMEWORK
                        var name = GeneratePqThreadName();


                        processingThread = new SafeThread(
                            MainProcessingThread,
                            _threadPriority, 
                            _useThreadPool,
                            name
                            );
#else


                        processingThread = new SafeThread(
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

// ReSharper disable once UnusedMember.Local
        private string GeneratePqThreadName()
        {
            var name = ToString();
            if (_threadNamePrefix != null)
            {
                name = _threadNamePrefix + StringConstants.SPACE + name;
            }

			try {
            Delegate d = null;

            if (_itemProcessingDelegate != null)
            {
                d = _itemProcessingDelegate;
            }
            else
            {
                if (_multiQueueItemProcessingDelegate != null)
                    d = _multiQueueItemProcessingDelegate;
            }

            if (d != null)
                name += " " + d.GetTargetDescription();
			}
			catch{
			}
            return name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool EnsureQueue([NotNull] object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            lock (_queuesSyncRoot)
            {
#if DEBUG
                if (_queueKeys.Length != _queues.Count)
                {
                    DebugHelper.TryBreak("ProcessingQueue _queueKeys and _queues count is unsynchronized");
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
                    },
                    null);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <exception cref="AlreadyExistsException"></exception>
        /// <exception cref="ArgumentNullException">if queueKey is null</exception>
        public void AddQueue([NotNull] object queueKey)
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
                    DebugHelper.TryBreak("ProcessingQueue _queueKeys and _queues count is unsynchronized");
                }
#endif

                var queue = new LevelSortedQueue<ItemCarrier<T>>(DefaultPriority);
                _queues[queueKey] = queue;
                RegisterQueueKey(queueKey);
            }
        }

        private void RegisterQueueKey([NotNull] object queueKey)
        {

            // NOT GOOD IDEA, THIS'LL CHANGE ORDER OF THE QUEUES
            //_queueKeys = _queues.KeysSnapshot;

            Array.Resize(ref _queueKeys, _queueKeys.Length + 1);
            // at the last postion
            _queueKeys[_queueKeys.Length - 1] = queueKey;

            _queueKeysSnapshot = (object[]) _queueKeys.Clone();
        }

        private int QueueKey2QueueId([NotNull] object queueKey)
        {
            var found = -1;
            for (var i = 0; i < _queueKeys.Length; i++)
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
        /// <exception cref="ArgumentNullException"></exception>
        public bool RemoveQueue([NotNull] object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            lock (_queuesSyncRoot)
            {
#if DEBUG
                if (_queueKeys.Length != _queues.Count)
                {
                    DebugHelper.TryBreak("ProcessingQueue _queueKeys and _queues count is unsynchronized");
                }
#endif

                LevelSortedQueue<ItemCarrier<T>> queue;
                if (_queues.Remove(queueKey, out queue) && queue != null)
                {
                    queue.Clear();

                    var found = QueueKey2QueueId(queueKey);

                    if (found >= 0)
                    {
                        // shrinking the array at position found
                        for (var j = found; j < _queueKeys.Length - 1; j++)
                        {
                            _queueKeys[j] = _queueKeys[j + 1];
                        }

                        Array.Resize(ref _queueKeys, _queueKeys.Length - 1);
                    }
                    else
                        DebugHelper.TryBreak("ProcessingQueue when queueKey not found", queueKey);

                    _queueKeysSnapshot = (object[]) _queueKeys.Clone();

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Action<T> _itemProcessingDelegate;
        


        /// <summary>
        /// not threadsafe, advised to assign/remove delegates from the same thread
        /// </summary>
        public event Action<T> ItemProcessing
        {
            add
            {
                if (_itemProcessingDelegate == null ||
                    !_itemProcessingDelegate.GetInvocationList().Contains(value))
                    _itemProcessingDelegate += value;
                else
                {
                    return;
                }

                EnsureProcessingThread();
            }
            remove
            {
// ReSharper disable once DelegateSubtraction
                _itemProcessingDelegate -= value;
            }

        }

        private Action<object, T> _multiQueueItemProcessingDelegate;

        /// <summary>
        /// not threadsafe, advised to assign/remove delegates from the same thread
        /// </summary>
        public event Action<object, T> MultiQueueItemProcessing
        {
            add
            {
                if (_multiQueueItemProcessingDelegate == null ||
                    !_multiQueueItemProcessingDelegate.GetInvocationList().Contains(value))
                    _multiQueueItemProcessingDelegate += value;
                else
                {
                    return;
                }

                
                EnsureProcessingThread();
            }
// ReSharper disable once DelegateSubtraction
            remove { _multiQueueItemProcessingDelegate -= value; }
        }

#pragma warning disable
        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        public event Action<T, bool, object> ItemProcessingResult;
#pragma warning restore
        
        /// <summary>
        /// 
        /// </summary>
        [PublicAPI]
        public event Action<T, Exception> ItemProcessingError;

        


        #region Idle generating

        private Func<object, T> _idleItemToBeGeneratedDelegate;
        /// <summary>
        /// not threadsafe, advised to assign/remove delegates from the same thread
        /// </summary>
        public event Func<object, T> IdleItemToBeGenerated
        {
            add
            {
                if (_idleItemToBeGeneratedDelegate == null ||
                    !_idleItemToBeGeneratedDelegate.GetInvocationList().Contains(value))
                    _idleItemToBeGeneratedDelegate += value;
                else
                    return;
                
                EnsureProcessingThread();
            }
// ReSharper disable once DelegateSubtraction
            remove { _idleItemToBeGeneratedDelegate -= value; }
        }

        private volatile bool _idleItemsGenerated = false;

        /// <summary>
        /// defines whether peeking thread will try to wait when queue(s) are empty
        /// or will try to generate idle items and process those
        /// </summary>
        public bool IdleItemsGenerated
        {
            get { return _idleItemsGenerated; }
            set
            {
                if (value && _idlePostSuccessDelay < 1)
                {
                    throw new ArgumentException(
                        "If IDLE generation is turned on, the IdleGeneratingDelay property should be set to non-zero to avoid CPU process burst");
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

        private volatile int _idlePostSuccessDelay = 10;

        /// <summary>
        /// defines , how often will be the IDLE item generated when IdleItemGenerated == true
        /// 
        /// delay applied only when IdleItemToBeGenerated returns NONnull
        /// and if there is no other item in the queue
        /// </summary>
        public int IdlePostSuccessDelay
        {
            get { return _idlePostSuccessDelay; }
            set { _idlePostSuccessDelay = value > 0 ? value : 0; }
        }

        private volatile int _idlePostFailureDelay = 100;

        /// <summary>
        /// delay applied only when IdleItemToBeGenerated returns null , or fails
        /// and if there is no other item in the queue
        /// </summary>
        public int IdlePostFailureDelay
        {
            get { return _idlePostFailureDelay; }
            set { _idlePostFailureDelay = value > 0 ? value : 0; }
        }


        #endregion

        

        /// <summary>
        /// points to item carrier, that is currently in processing
        /// </summary>
        /// 
        protected volatile ItemCarrier<T> _actualItemCarrier = null;

        /// <summary>
        /// ensures synchronized access for _actualItemCarrier via locks
        /// </summary>
        protected volatile object _actualItemCarrierSync = new object();

        /// <summary>
        /// returns the item that is currently in processing, or default(T) if there is no item in processing
        /// </summary>
        public T ActualItem
        {
            get
            {
                var item = default(T);

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
        /// <param name="postDelay"></param>
        protected virtual bool ProcessSingleItem(object queueKey, ItemCarrier<T> itemCarrier, int postDelay)
        {
            return ProcessSingleItem(queueKey, itemCarrier, true, postDelay);
        }

        /// <summary>
        /// single item processing
        /// </summary>
        /// <returns>true, if ItemProcessing passed without raising exception ; 
        /// false in cases of Exception or ItemProcessing is not bound, thus null</returns>
        /// <param name="queueKey"></param>
        /// <param name="itemCarrier"></param>
        /// <param name="setItemProcessingState"></param>
        /// <param name="postDelay"></param>
        protected bool ProcessSingleItem(object queueKey, ItemCarrier<T> itemCarrier, bool setItemProcessingState,int postDelay)
        {
            if (null == _itemProcessingDelegate 
                && null == _multiQueueItemProcessingDelegate) 
                return false;

            var result = false;
            try
            {
                if (!AimedToBeDisposed)
                {
                    lock (_actualItemCarrierSync)
                    {
                        _actualItemCarrier = itemCarrier;
                        itemCarrier.SetProcessingState(PQProcessingState.Processing);
                    }

                    // not expecting to call both handlers
                    _itemProcessingDelegate.Call(itemCarrier._item);

                    _multiQueueItemProcessingDelegate.Call(queueKey, itemCarrier._item);

                    result = true;
                }
            }
            catch (ThreadAbortException tae)
            {
                if (tae.ExceptionState != InternalThreadAbortKey.Singleton)
                {
                    HandledExceptionAdapter.Examine(tae);

#if !COMPACT_FRAMEWORK
                        try
                        {
                            Thread.ResetAbort();
                        }
                        catch
                        {
                        }

                        DebugHelper.TryBreak("Somebody is trying to stop peeking thread from inside of the processing",
                            typeof (T));
#endif
                }
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);

                if (null != ItemProcessingError &&
                    !AimedToBeDisposed)
                    // raising ItemProcessingError during PQ disposal process only produces more troubles
                    try
                    {
                        ItemProcessingError(itemCarrier._item, e);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }

            if (setItemProcessingState)
                lock (_actualItemCarrierSync)
                {
                    try
                    {
                        _actualItemCarrier.SetProcessingState(PQProcessingState.Processed);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                    _actualItemCarrier = null;

                }

            if (postDelay > 0)
                Thread.Sleep(postDelay);

            return result;
        }

        


        /// <summary>
        /// processes dequeued item and its timeouts if async mode
        /// </summary>
        /// <param name="queueKey"></param>
        /// <param name="itemCarrier"></param>
        private void ProcessItemAndTimeouts(object queueKey, ItemCarrier<T> itemCarrier)
        {
            var postDelay = _postProcessingDelay;

            try
            {
                ProcessSingleItem(queueKey, itemCarrier, postDelay);                
            }
            catch
            {
            }
            finally
            {
                itemCarrier.TryReturn();
            }

        }

        private const int MinimalDisposalTimeout = 100;
        private const int DefaultDisposalTimeout = 2000;

        private int _disposalTimeout = DefaultDisposalTimeout;

        /// <summary>
        /// defines amount of time (by default 2000ms) that Dispose/InternalDispose will wait for MainProcessingThread to finish/Join,
        /// if timeout not fullfiled, MainProcessingThread is aborted
        /// </summary>
        public int DisposalTimeout
        {
            get { return _disposalTimeout; }
            set { _disposalTimeout = value > MinimalDisposalTimeout ? value : MinimalDisposalTimeout; }
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
                        _queueRandomizer = new Random((int) DateTime.Now.Ticks);
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

                    if (null == _idleItemToBeGeneratedDelegate || !_idleItemsGenerated)
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

                        var queue = PickQueueByMode(out queueId, out queueKey);
                        if (queue == null)
                        {
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
                                foreach (var queueToCompare in _queues.Values)
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
                        if (null != _idleItemToBeGeneratedDelegate && _idleItemsGenerated)
                        {
                            var idlePostFailureDelay = _idlePostFailureDelay;

                            try
                            {
                                // making snapshots intentionally
                                var idlePostSuccessDelay = _idlePostSuccessDelay;


                                var generatedItem = _idleItemToBeGeneratedDelegate(queueKey);

                                var generatedItemIsNull = ReferenceEquals(generatedItem, null);


                                if (!generatedItemIsNull ||
                                    !_idleItemsPreventNull)
                                {
                                    itemCarrier = PrepareItemCarrier(generatedItem, DefaultPriority, false);

                                    ProcessItemAndTimeouts(queueKey, itemCarrier);
                                }


                                if (CountIsZero)
                                {
                                    if (generatedItemIsNull)
                                    {
                                        if (idlePostFailureDelay > 0)
                                            Thread.Sleep(idlePostFailureDelay);
                                    }
                                    else
                                        // special kind of post-delay
                                        if (idlePostSuccessDelay > 0)
                                            Thread.Sleep(idlePostSuccessDelay);
                                }

                            }
                            catch (Exception e)
                            {
                                HandledExceptionAdapter.Examine(e);

                                if (CountIsZero)
                                {
                                    if (idlePostFailureDelay > 0)
                                    {
                                        Thread.Sleep(idlePostFailureDelay);
                                    }
                                }
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
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
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

                // unregister all event delegates that might hold references to targets
                _itemProcessingDelegate = null;
                _multiQueueItemProcessingDelegate = null;
                _idleItemToBeGeneratedDelegate = null;

                ItemProcessingError = null;
                ItemProcessingResult = null;
                



                // the queues should be cleared before the mutex about emptyness is released
                InternalClear(false);

                //if (isExplicitDispose)
                {
                    // release mutexes, that might be pending the process
                    try
                    {
                        var success = UnlockDueQueuesNotEmpty();

                        DebugHelper.Keep(success);
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                    catch (Exception)
                    {
                        // avoid exception examination in dispose
                        // Sys.HandledExceptionAdapter.Examine(e);

                    }
                }

                // Debug.WriteLine("Disposal NEM "+typeof(T)+" Set");

                

                if (null != _processingThread)
                    lock (_threadAccessSync)
                    {
                        if (null != _processingThread)
                        {
#if !COMPACT_FRAMEWORK
// ReSharper disable UnusedVariable
                            var stsr = _processingThread.Stop(_disposalTimeout);
// ReSharper restore UnusedVariable
#else
                            _processingThread.Stop(_disposalTimeout);
#endif

#if DEBUG && !COMPACT_FRAMEWORK
                            if (stsr == SafeThreadStopResolution.StoppedForcefuly ||
                                stsr == SafeThreadStopResolution.DidNotStopYet ||
                                stsr == SafeThreadStopResolution.StopFailed ||
                                stsr == SafeThreadStopResolution.NotStarted ||
                                stsr == SafeThreadStopResolution.AbortNotAllowed)
                            {
                                var tmp = "Stopping processing queue : " + stsr + " : " + typeof (T);
                                Debug.WriteLine(tmp);

                                if (_disposalTimeout >= DefaultDisposalTimeout &&
                                    (stsr == SafeThreadStopResolution.StoppedForcefuly ||
                                     stsr == SafeThreadStopResolution.DidNotStopYet ||
                                     stsr == SafeThreadStopResolution.AbortNotAllowed ||
                                     stsr == SafeThreadStopResolution.StopFailed
                                        ))
                                    DebugHelper.TryBreak(
                                        "Possibly a deadlock between item processing and call of Dispose\n" + tmp);
                            }
#endif
                        }
                    }


            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                base.InternalDispose(isExplicitDispose);
            }
        }

        /// <summary>
        /// count of all items in all queues
        /// </summary>
        public override int Count
        {
            get
            {
                var count = 0;
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
        public override bool CountIsZero
        {
            get
            {
                lock (_queuesSyncRoot)
                {
                    foreach (var q in _queues.Values)
                        if (q.Count != 0)
                            return false;
                }

                return true;
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
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public int GetQueueItemCount([NotNull] object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

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
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public bool IsQueuePresent([NotNull] object queueKey)
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
                    catch (Exception e)
                    {
                        HandledExceptionAdapter.Examine(e);
                    }
            }
        }

        /// <summary>
        /// erases all items in all queues
        /// </summary>
        public override void Clear()
        {
            InternalClear(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueKey"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Clear([NotNull] object queueKey)
        {
            if (ReferenceEquals(queueKey, null))
                throw new ArgumentNullException("queueKey");

            var queue = _queues[queueKey];
            if (queue != null)
                // synchronized by itself
                queue.Clear();
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
        public delegate void DCompareAndRemoveItem(T itemInQueue, object itemToCompareWith, out bool remove);

        /// <summary>
        /// removes the item of the queue/s by the condition implemented by user's comparer delegate
        /// </summary>
        /// <param name="comparingLambda"></param>
        /// <returns>count of items removed</returns>
        public int RemoveWhere([NotNull] Func<T, bool> comparingLambda)
        {
            return RemoveWhere((objectFromQueue,stateNotUsed)=> comparingLambda(objectFromQueue), null);
        }

        /// <summary>
        /// removes the item of the queue/s by the condition implemented by user's comparer delegate
        /// </summary>
        /// <param name="comparingLambda"></param>
        /// <param name="state">any state object passed into the comparer delegate for the comparing process</param>
        /// <returns>count of items removed</returns>
        public int RemoveWhere([NotNull] Func<T, object, bool> comparingLambda, [CanBeNull] object state)
        {
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (null == comparingLambda)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
                return 0;
// ReSharper restore HeuristicUnreachableCode

            var countRemoved = 0;

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
                                    return comparingLambda(item.Item, state);
                                }
                                catch (Exception e)
                                {
                                    HandledExceptionAdapter.Examine(e);
                                    return false;
                                }
                            }

                            , state);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }

            }

            return countRemoved;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetOverallSnapshot()
        {
            IEnumerator<ItemCarrier<T>>[] enumerators = null;


            var i = 0;
            // done this way to invoke snapshots on all queues in one time,
            // rather than to invoke snapshots in the outside iteration process
            _queues.ForEach((key, value) =>
            {
                if (enumerators == null)
                    enumerators = new IEnumerator<ItemCarrier<T>>[_queues.Count];

                if (value != null)
                    enumerators[i] = value.GetEnumerator();

                i++;
            });

            if (null == enumerators)
                yield break;

            foreach (var queueSnapshotEnumerator in enumerators)
            {
                // can happen as in previous iteration the through queues, the queue can be null
                if (queueSnapshotEnumerator == null)
                    continue;

                bool iterationSuccessful;
                do
                {
                    iterationSuccessful = queueSnapshotEnumerator.MoveNext();
                    if (iterationSuccessful)
                        yield return queueSnapshotEnumerator.Current._item;
                    else
                        yield break;
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
                } while (iterationSuccessful);


            }

        }

        
        public override string ToString()
        {
            return _toStringCached ?? 
                (_toStringCached = "PQsync" + "<" + typeof (T).FullName + ">");
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
