using System;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public class BinaryHeapSupportedRemoval<TBinaryHeapItem>
        : ABinaryHeap<TBinaryHeapItem>
        where TBinaryHeapItem : BaseBinarayHeapItem
                              , IComparable<TBinaryHeapItem>
    {
        protected override int Compare(TBinaryHeapItem firstItem, TBinaryHeapItem secondItem)
        {
            return firstItem.CompareTo(secondItem);
        }

        public void Add(TBinaryHeapItem item)
        {
            AddInternal(item);
        }

        public TBinaryHeapItem GetMinimum()
        {
            return GetMinimumInternal();
        }

        public void Remove(TBinaryHeapItem item)
        {
            RemoveInternal(item);
        }
    }
}
