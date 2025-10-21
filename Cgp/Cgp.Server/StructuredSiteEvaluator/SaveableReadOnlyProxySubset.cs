using System.Collections.Generic;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public class SaveableReadOnlyProxySubset<TElement> : IReadOnlySubSet<TElement>
    {
        private Subset<TElement> _initialSubset;
        private ReadOnlyProxySubset<TElement> _readOnlyProxySubset;

        public SaveableReadOnlyProxySubset()
        {
            _initialSubset = new Subset<TElement>();
        }

        public SaveableReadOnlyProxySubset(Subset<TElement> targetSubset)
        {
            _readOnlyProxySubset = new ReadOnlyProxySubset<TElement>(targetSubset);
        }

        public bool Contains(TElement element)
        {
            return
                _initialSubset != null
                    ? _initialSubset.Contains(element)
                    : _readOnlyProxySubset.Contains(element);
        }

        public IEnumerable<TElement> Elements
        {
            get
            {
                return
                    _initialSubset != null
                        ? _initialSubset.Elements
                        : _readOnlyProxySubset.Elements;
            }
        }

        public int Count
        {
            get
            {
                return
                    _initialSubset != null
                        ? _initialSubset.Count
                        : _readOnlyProxySubset.Count;
            }
        }

        public void OnSaved(Subset<TElement> targetSubset)
        {
            if (_initialSubset == null)
            {
                return;
            }

            _initialSubset = null;
            _readOnlyProxySubset = 
                new ReadOnlyProxySubset<TElement>(targetSubset);
        }
    }
}