namespace Contal.IwQuick.CrossPlatform.Data
{
    public abstract class ABinaryHeap<TBinaryHeapItem>
        where TBinaryHeapItem : BaseBinarayHeapItem
    {
        private const int DefaultArrayIncrementationLength = 100;

        private readonly ResizeableArray<TBinaryHeapItem> _items;

        public bool IsEmpty
        {
            get
            {
                lock (_items)
                    return _items.Count == 0;
            }
        }

        public int Count
        {
            get
            {
                lock (_items)
                    return _items.Count;
            }
        }

        protected ABinaryHeap()
        {
            _items = new ResizeableArray<TBinaryHeapItem>(DefaultArrayIncrementationLength);
        }

        protected void AddInternal(TBinaryHeapItem item)
        {
            lock (_items)
            {
                _items.AddLast(item);
                item.Index = _items.Count - 1;

                if (_items.Count == 1)
                    return;

                ReorganizeHeapUpwards(_items.Count - 1);
            }
        }

        private bool ReorganizeHeapUpwards(int startIndex)
        {
            var result = false;
            var index = startIndex;
            var parentIdex = GetParent(index);

            while (index > 0 && Compare(_items[parentIdex], _items[index]) > 0)
            {
                result = true;
                SwapItems(parentIdex, index);

                index = parentIdex;
                parentIdex = GetParent(index);
            }

            return result;
        }

        protected void RemoveInternal(TBinaryHeapItem item)
        {
            if (_items.Count <= item.Index)
                return;

            if (item.Index == _items.Count - 1)
            {
                _items.RemoveLast();
                return;
            }

            _items[item.Index] = _items[_items.Count - 1];
            _items[item.Index].Index = item.Index;
            _items.RemoveLast();

            if (!ReorganizeHeapUpwards(item.Index))
                ReorganizeHeapDownwards(item.Index);
        }

        protected abstract int Compare(TBinaryHeapItem firstItem, TBinaryHeapItem secondItem);

        protected TBinaryHeapItem GetMinimumInternal()
        {
            lock (_items)
            {
                if (_items.Count == 0)
                    return default(TBinaryHeapItem);

                return _items[0];
            }
        }

        private void SwapItems(int firstIndex, int secondIndex)
        {
            var item = _items[firstIndex];
            _items[firstIndex] = _items[secondIndex];
            _items[firstIndex].Index = firstIndex;
            _items[secondIndex] = item;
            item.Index = secondIndex;
        }

        private void ReorganizeHeapDownwards(int startIndex)
        {
            int index = startIndex;

            do
            {
                var leftChildIndex = GetLeftChild(index);
                var rightChildIndex = GetRightChild(index);

                var smallest = index;

                if (leftChildIndex < _items.Count && Compare(_items[smallest], _items[leftChildIndex]) > 0)
                    smallest = leftChildIndex;

                if (rightChildIndex < _items.Count && Compare(_items[smallest], _items[rightChildIndex]) > 0)
                    smallest = rightChildIndex;

                if (smallest == index)
                    return;

                SwapItems(smallest, index);

                index = smallest;
            } while (true);
        }

        private int GetParent(int index)
        {
            return (index - 1) / 2;
        }

        private int GetLeftChild(int index)
        {
            return (2 * index) + 1;
        }

        private int GetRightChild(int index)
        {
            return (2 * index) + 2;
        }

        public void Clear()
        {
            lock (_items)
                _items.Clear();
        }
    }
}
