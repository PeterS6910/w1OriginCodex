using System;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class BinaryHeap<TValue>
        : ABinaryHeap<BinaryHeap<TValue>.BinaryHeapItem>
        where TValue : IComparable<TValue>
    {
        public class BinaryHeapItem : BaseBinarayHeapItem
        {
            public TValue Value { get; private set; }

            public BinaryHeapItem(TValue value)
            {
                Value = value;
            }
        }

        protected override int Compare(BinaryHeapItem firstItem, BinaryHeapItem secondItem)
        {
            return firstItem.Value.CompareTo(secondItem.Value);
        }

        public void Add(TValue item)
        {
            AddInternal(new BinaryHeapItem(item));
        }

        public TValue GetAndRemoveMinimum()
        {
            var binaryHeapItem = GetMinimumInternal();

            if (binaryHeapItem == null)
                return default(TValue);

            RemoveInternal(binaryHeapItem);

            return binaryHeapItem.Value;
        }
    }
}
