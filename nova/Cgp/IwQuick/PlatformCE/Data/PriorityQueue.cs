using System;
using System.Collections.Generic;

namespace Contal.IwQuick.Data
{
    public enum QueueItemPriority
    {
        Highest,
        Precedence,
        Normal
    }

    public class PriorityQueue<T> : LinkedList<T>, ICollection<T>
    {
        public LinkedListNode<T> _endOfPriorityPart = null;

        public PriorityQueue()
            :
            base()
        {

        }

        public void Enqueue(T item, QueueItemPriority priority)
        {
            switch (priority)
            {
                case QueueItemPriority.Highest:
                    AddFirst(item);

                    if (null == _endOfPriorityPart)
                        _endOfPriorityPart = First;

                    break;
                case QueueItemPriority.Precedence:
                    if (null == _endOfPriorityPart)
                    {
                        AddFirst(item);
                        _endOfPriorityPart = First;
                    }
                    else
                    {
                        AddAfter(_endOfPriorityPart, item);
                        _endOfPriorityPart = _endOfPriorityPart.Next;
                    }
                    break;
                case QueueItemPriority.Normal:
                    AddLast(item);
                    break;
            }
        }

        public void Enqueue(T item)
        {
            Enqueue(item, QueueItemPriority.Normal);
        }

        public T Dequeue()
        {
            if (0 == Count)
                throw new InvalidOperationException();

            T aItem = First.Value;

            if (null != _endOfPriorityPart)
            {
                if (First == _endOfPriorityPart)
                    _endOfPriorityPart = null;
            }

            RemoveFirst();
            return aItem;
        }

        new public void Clear()
        {
            base.Clear();
            _endOfPriorityPart = null;
        }

        new public void Remove(LinkedListNode<T> item)
        {
            if (null == item)
                throw new ArgumentNullException();

            bool bShift = false;
            if (item == _endOfPriorityPart)
            {
                bShift = true;
            }

            base.Remove(item);

            // do this lately in case of removal error
            if (bShift)
                _endOfPriorityPart = _endOfPriorityPart.Previous;
        }

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            Enqueue(item, QueueItemPriority.Normal);
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
