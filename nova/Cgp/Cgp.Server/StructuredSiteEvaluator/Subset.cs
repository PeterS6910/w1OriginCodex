using System.Collections.Generic;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public class Subset<TElement> : ISubset<TElement>
    {
        private readonly HashSet<TElement> _underlyingHashSet;

        public Subset()
        {
            _underlyingHashSet = new HashSet<TElement>();
        }
        
        public Subset(IEnumerable<TElement> elements)
        {
            _underlyingHashSet = new HashSet<TElement>(elements);
        }

        public bool Add(TElement element)
        {
            return _underlyingHashSet.Add(element);
        }

        public bool Remove(TElement element)
        {
            return _underlyingHashSet.Remove(element);
        }

        public bool Contains(TElement element)
        {
            return _underlyingHashSet.Contains(element);
        }

        public IEnumerable<TElement> Elements
        {
            get { return _underlyingHashSet; }
        }

        public int Count
        {
            get { return _underlyingHashSet.Count; }
        }
    }
}