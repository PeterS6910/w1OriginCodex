namespace Contal.IwQuick.CrossPlatform.Data
{
    public class PriorityBinaryHeap<T>
        : ABinaryHeap<PriorityBinaryHeap<T>.BinaryHeapItem>
    {
        public class BinaryHeapItem : BaseBinarayHeapItem
        {
            private readonly uint _id;
            private readonly uint _priority;

            public T Value { get; private set; }

            public BinaryHeapItem(
                uint id,
                uint priority,
                T value)
            {
                _id = id;
                _priority = priority;
                Value = value;
            }

            public int CompareTo(BinaryHeapItem other, uint lastHeapItemId)
            {
                var result = _priority.CompareTo(other._priority);

                if (result == 0)
                {
                    if (_id > lastHeapItemId
                        && other._id < lastHeapItemId)
                    {
                        return -1;
                    }

                    if (_id < lastHeapItemId
                        && other._id > lastHeapItemId)
                    {
                        return 1;
                    }

                    return _id.CompareTo(other._id);
                }

                return result;
            }
        }

        private readonly object _lastHeapItemIdLock = new object();
        private uint _lastHeapItemId;

        protected override int Compare(BinaryHeapItem firstItem, BinaryHeapItem secondItem)
        {
            return firstItem.CompareTo(secondItem, _lastHeapItemId);
        }

        public void Add(T value, uint priority)
        {
            uint heapItemId;
            lock (_lastHeapItemIdLock)
            {
                heapItemId = _lastHeapItemId;

                if (_lastHeapItemId == uint.MaxValue)
                    _lastHeapItemId = 0;
                else
                    _lastHeapItemId++;
            }

            AddInternal(
                new BinaryHeapItem(
                    heapItemId,
                    priority,
                    value));
        }

        public T GetAndRemoveMinimum()
        {
            var heapItem = GetMinimumInternal();

            if (heapItem == null)
                return default(T);

            RemoveInternal(heapItem);

            return heapItem.Value;
        }
    }
}
