using System.Collections.Generic;
using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LevelSortedQueue<T> : ADisposable, ICollection<T> , IWaitCollection
    {
        private volatile uint _defaultPriority;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultPriority"></param>
        public LevelSortedQueue(uint defaultPriority)
        {
            DefaultPriority = defaultPriority;
        }

        private readonly LinkedList<T> _queue = new LinkedList<T>();
        private readonly object _queueSyncRoot = new object();

        private class LevelInfo<TItem>
        {
            /// <summary>
            /// numerically lower priority value is logically higher priority
            ///  </summary>
            /// <param name="priority"></param>
            internal LevelInfo(uint priority)
            {
                _priority = priority;
            }

            internal readonly uint _priority;
            internal LinkedListNode<TItem> _firstNode;
            internal LinkedListNode<TItem> _lastNode;
            internal uint _itemCount;
        }

        private readonly Dictionary<uint, LevelInfo<T>> _levels = new Dictionary<uint, LevelInfo<T>>(2);
        private volatile uint[] _levelsSorted;

        /// <summary>
        /// makes sorting of the priority from _levels.Keys
        /// </summary>
        private void RefreshLevelsSorted()
        {
            _levelsSorted = new uint[_levels.Count];

            if (_levels.Count > 0)
            {
                _levels.Keys.CopyTo(_levelsSorted, 0);

                Array.Sort(_levelsSorted);
            }
        }

        private void IntroduceLevel(T item, uint priority,[CanBeNull] LinkedListNode<T> beforeNode)
        {
            var li = new LevelInfo<T>(priority);

            if (beforeNode == null)
            {
                li._firstNode = li._lastNode =
                        _queue.AddLast(item);
            }
            else
                li._firstNode = li._lastNode =
                    _queue.AddBefore(beforeNode, item);

            _levels[priority] = li;
            RefreshLevelsSorted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        /// <param name="onTop"></param>
        public void Enqueue(T item, uint priority, bool onTop)
        {
            lock (_queueSyncRoot)
            {
                LevelInfo<T> li;
                if (_levels.TryGetValue(priority, out li))
                {
                    if (onTop)
                    {
                        li._firstNode = _queue.AddBefore(li._firstNode, item);
                    }
                    else
                    {
                        li._lastNode = _queue.AddAfter(li._lastNode, item);
                    }

                    li._itemCount++;
                }
                else
                {
                    if (_queue.Count == 0)
                    {
                        IntroduceLevel(item, priority, null);
                    }
                    else
                    {
                        if (null != _levelsSorted && _levelsSorted.Length > 0)
                        {
                            if (priority < _levelsSorted[0])
                            {
                                // just optimisation, quicker than via _levels.TryGetValue(_levelsSorted[i], out nextPriorityInfo)
                                IntroduceLevel(item, priority, _queue.First);
                            }
                            else
                            {
                                if (priority > _levelsSorted[_levelsSorted.Length - 1])
                                {
                                    // last value prio
                                    // onTop does not matter here, cause it's only one of it's kind (priority)
                                    IntroduceLevel(item, priority, null);
                                }
                                else
                                    // specified priority equals the highest priority in _levelSorted array
                                    for (int i = 0; i < _levelsSorted.Length; i++)
                                    {
                                        if (priority < _levelsSorted[i])
                                        {
                                            LevelInfo<T> nextPriorityInfo;
                                            if (_levels.TryGetValue(_levelsSorted[i], out nextPriorityInfo))
                                            {
                                                // onTop does not matter here, cause it's only one of it's kind (priority)
                                                IntroduceLevel(item, priority, nextPriorityInfo._firstNode);

                                            }
                                            else
                                                DebugHelper.TryBreak("LevelSortedQueue.Enqueue incosistency1");

                                            break;
                                        }
                                    }
                            }
                        }
                        else
                        {

                            DebugHelper.TryBreak("LevelSortedQueue.Enqueue inconsistency2");
                        }
                    }
                }

                CheckWaitingMutexes();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Dequeue(out T outItem)
        {
            lock (_queueSyncRoot)
            {
                if (_queue.Count == 0)
                {
                    outItem = default(T);
                    return false;
                }

                LinkedListNode<T> lln = _queue.First;
                LevelInfo<T> li;

                uint lowestPriorityValue = _levelsSorted[0];
                if (_levels.TryGetValue(lowestPriorityValue, out li))
                {
                    if (ReferenceEquals(lln, li._firstNode))
                    {
                        if (ReferenceEquals(lln, li._lastNode))
                        {
                            // removing the level from _levels because the dequeued instance is the last one for the level

                            _levels.Remove(li._priority);

                            RefreshLevelsSorted();
                        }
                        else
                        {
                            li._firstNode = li._firstNode.Next;

                            li._itemCount--;
                        }
                    }
                    else
                        DebugHelper.TryBreak("LevelSortedQueue.Dequeue inconsitency");
                }

                outItem = _queue.First.Value;
                _queue.RemoveFirst();

                CheckWaitingMutexes();

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outItem"></param>
        /// <returns></returns>
        public bool DequeueLast(out T outItem)
        {
            lock (_queueSyncRoot)
            {
                if (_queue.Count == 0)
                {
                    outItem = default(T);
                    return false;
                }

                LinkedListNode<T> lln = _queue.First;
                LevelInfo<T> li;

                uint highestPriorityValue = _levelsSorted[_levelsSorted.Length-1];
                if (_levels.TryGetValue(highestPriorityValue, out li))
                {
                    if (ReferenceEquals(lln, li._lastNode))
                    {
                        if (ReferenceEquals(lln, li._firstNode))
                        {
                            // removing the level from _levels because the dequeued instance is the last one for the level

                            _levels.Remove(li._priority);

                            RefreshLevelsSorted();
                        }
                        else
                        {
                            li._lastNode = li._lastNode.Previous;

                            li._itemCount--;
                        }
                    }
                    else
                        // inconsitency , should not happen
                        DebugHelper.TryBreak("LevelSortedQueue.DequeueLast inconsistency");
                }

                outItem = _queue.Last.Value;
                _queue.RemoveLast();

                CheckWaitingMutexes();

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExplicitDispose"></param>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            Clear();
        }


        #region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            Enqueue(item, DefaultPriority, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            lock (_queueSyncRoot)
            {
                // O(n)
                _queue.Clear();
                // O(n)
                _levels.Clear();
                _levelsSorted = null;

                CheckWaitingMutexes();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            lock (_queueSyncRoot)
            {
                return _queue.Contains(item);
            }
        }

        ///  <summary>
        /// 
        ///  </summary>
        ///  <param name="comparingLambda">SHOULD BE AS FAST AS POSSIBLE</param>
        /// <param name="objectToCompareWith"></param>
        /// <returns></returns>
        internal bool Contains([NotNull] Func<T,object,bool> comparingLambda,object objectToCompareWith)
        {
            lock (_queueSyncRoot)
            {
                foreach (var o in _queue)
                {
                    try
                    {
                        if (comparingLambda(o, objectToCompareWith))
                            return true;
                    }
                    catch
                    {
                        
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo([NotNull] T[] array, int arrayIndex)
        {
			if (ReferenceEquals(array,null))
				throw new ArgumentNullException("array");

            lock (_queueSyncRoot)
                _queue.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<T> QueueSnapshot
        {
            get
            {
                lock (_queueSyncRoot)
                {
                    T[] snapshotArray = new T[_queue.Count];
                    _queue.CopyTo(snapshotArray, 0);

                    return snapshotArray;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                lock (_queueSyncRoot)
                {
                    return _queue.Count;
                }
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public uint DefaultPriority
        {
            get { return _defaultPriority; }
            set { _defaultPriority = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            lock (_queueSyncRoot)
            {
                // object value is just because CF does not know hashset
                Dictionary<LinkedListNode<T>, object> nodesToRemove = null;

                // comparing part
                LinkedListNode<T> lln = _queue.First;
                while(lln != null)
                {
                    if (ReferenceEquals(item, lln.Value) ||
                        (!ReferenceEquals(item, null) && item.Equals(lln.Value)))
                    {
                        if (nodesToRemove == null)
                            nodesToRemove = new Dictionary<LinkedListNode<T>, object>(2);

                        nodesToRemove[lln] = lln;
                    }

                    lln = lln.Next;
                }

                // execution part
                bool nodesRemoved = RemoveNodes(nodesToRemove) > 0;
                CheckWaitingMutexes();
                return nodesRemoved;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemInQueue"></param>
        /// <param name="itemToCompareWith"></param>
        /// <param name="remove"></param>
        public delegate void DCompareAndRemoveLambda(T itemInQueue, object itemToCompareWith, out bool remove);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparingLambda"></param>
        /// <param name="itemToCompareWith"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public int Remove([NotNull] Func<T,object,bool> comparingLambda, object itemToCompareWith)
        {
            if (ReferenceEquals(comparingLambda, null))
                throw new ArgumentNullException("comparingLambda");

            lock (_queueSyncRoot)
            {
                // object value is just because CF does not know hashset
                Dictionary<LinkedListNode<T>, object> nodesToRemove = null;

                // comparing part
                LinkedListNode<T> lln = _queue.First;
                while (lln != null)
                {
                    bool toRemove = false;
                    try
                    {
                        toRemove = comparingLambda(lln.Value, itemToCompareWith);
                    }
                    catch
                    {
                        
                    }

                    if (toRemove)
                    {
                        if (nodesToRemove == null)
                            nodesToRemove = new Dictionary<LinkedListNode<T>, object>(2);

                        nodesToRemove[lln] = lln;
                    }

                    lln = lln.Next;
                }

                // execution part
                int nodesRemoved = RemoveNodes(nodesToRemove);

                CheckWaitingMutexes();

                return nodesRemoved;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodesToRemove"></param>
        private int RemoveNodes([CanBeNull] IDictionary<LinkedListNode<T>, object> nodesToRemove)
        {
            if (nodesToRemove == null)
                return 0;

            bool refreshLevelsSorted = false;
            foreach (var li in _levels.Values)
            {
                if (li._firstNode == null ||
                    li._lastNode == null)
                {
                    DebugHelper.TryBreak("LevelSortedQueue.RemoveNodes inconsistency");
                    continue;
                }

                bool first = nodesToRemove.ContainsKey(li._firstNode);
                bool last = nodesToRemove.ContainsKey(li._lastNode);


                if (first && last)
                {
                    _levels.Remove(li._priority);

                    // do not refresh right now, waste of time
                    refreshLevelsSorted = true;
                }
                else if (first)
                {
                    li._firstNode = li._firstNode.Next;
                    li._itemCount--;
                }
                else if (last)
                {
                    li._lastNode = li._lastNode.Previous;
                    li._itemCount--;
                }
            }

            if (refreshLevelsSorted)
                RefreshLevelsSorted();

            foreach (var k in nodesToRemove.Keys)
            {
                _queue.Remove(k);
            }

            return nodesToRemove.Count;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return QueueSnapshot.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return QueueSnapshot.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public bool IsLevelPresent(uint priority)
        {
            lock (_queueSyncRoot)
                return _levels.ContainsKey(priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public uint GetLevelItemCount(uint priority)
        {
            lock (_queueSyncRoot)
            {
                LevelInfo<T> li;
                if (_levels.TryGetValue(priority, out li))
                    return li._itemCount;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="shrinkIfExceeded"></param>
        /// <param name="shrinkFromTop"></param>
        /// <returns>true, if limit constraint not violated</returns>
        public bool ValidateLimitCount(int limit, bool shrinkIfExceeded, bool shrinkFromTop)
        {
            if (limit <= 0)
                return true; // faked true

            lock (_queueSyncRoot)
            {
                if (_queue.Count >= limit)
                {
                    if (shrinkIfExceeded)
                    {
                        T dummyItem;

                        if (shrinkFromTop)
                            DequeueLast(out dummyItem);
                        else
                        {
                            Dequeue(out dummyItem);
                        }
                    }
                    else
                        // limit constraint is violated
                        return false;
                }
            }

            return true;
        }

        private volatile ManualResetEvent _mreWaitWhileEmpty = null;
        private volatile ManualResetEvent _mreWaitUntilEmpty = null;

        /// <summary>
        /// should be locked outside by lock(_queueSyncRoot)
        /// </summary>
        private void CheckWaitingMutexes()
        {
            if (null != _mreWaitWhileEmpty)
            {
                if (_queue.Count > 0)
                    _mreWaitWhileEmpty.Set();
                else
                    _mreWaitWhileEmpty.Reset();
            }

            if (null != _mreWaitUntilEmpty)
            {
                if (_queue.Count == 0)
                    _mreWaitUntilEmpty.Set();
                else
                    _mreWaitUntilEmpty.Reset();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool WaitWhileEmpty(int timeout)
        {
            lock (_queueSyncRoot)
            {
                if (_mreWaitWhileEmpty == null)
                    _mreWaitWhileEmpty = new ManualResetEvent(_queue.Count > 0);
            }

            return _mreWaitWhileEmpty.WaitOne(timeout, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitWhileEmpty()
        {
            WaitWhileEmpty(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitUntilEmpty(int timeout)
        {
            lock (_queueSyncRoot)
            {
                if (_mreWaitUntilEmpty == null)
                    _mreWaitUntilEmpty = new ManualResetEvent(_queue.Count == 0);
            }

            return _mreWaitUntilEmpty.WaitOne(timeout, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitUntilEmpty()
        {
            WaitUntilEmpty(-1);
        }
    }
}
