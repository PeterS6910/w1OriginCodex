using System.Collections.Generic;

namespace Contal.IwQuick.Data
{
    public class LimitedQueue<TElement>
    {
        private readonly int _maxSize;

        public LimitedQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

        public bool Contains(TElement element)
        {
            lock (_elementsSet)
                return _elementsSet.Contains(element);
        }

        public void Add(IEnumerable<TElement> elements)
        {
            lock (_elementsSet)
            {
                foreach (TElement element in elements)
                {
                    if (_elementsSet.Contains(element))
                        continue;

                    _elementsSet.Add(element);
                    _elementsList.AddLast(element);
                }

                while (_elementsList.Count > _maxSize)
                {
                    TElement firstElement = _elementsList.First.Value;

                    _elementsList.RemoveFirst();
                    _elementsSet.Remove(firstElement);
                }
            }
        }

        private readonly HashSet<TElement> _elementsSet = 
            new HashSet<TElement>();

        private readonly LinkedList<TElement> _elementsList = 
            new LinkedList<TElement>();

        public void Clear()
        {
            lock (_elementsSet)
            {
                _elementsSet.Clear();
                _elementsList.Clear();
            }
        }
    }
}
