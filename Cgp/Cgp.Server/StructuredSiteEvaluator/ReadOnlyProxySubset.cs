using System.Collections.Generic;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public class ReadOnlyProxySubset<TElement> : IReadOnlySubSet<TElement>
    {
        private readonly ISubset<TElement> _targetSubset;

        public ReadOnlyProxySubset(ISubset<TElement> targetSubset)
        {
            _targetSubset = targetSubset;
        }

        public bool Contains(TElement element)
        {
            return _targetSubset.Contains(element);
        }

        public IEnumerable<TElement> Elements
        {
            get { return _targetSubset.Elements; }
        }

        public int Count
        {
            get { return _targetSubset.Count; }
        }
    }
}
