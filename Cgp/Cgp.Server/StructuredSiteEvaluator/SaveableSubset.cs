using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public class SaveableSubset<TElement> : IDifferentialSubset<TElement>
    {
        private Subset<TElement> _initialSubset;
        private DifferentialSubset<TElement> _differentialSubset;

        public SaveableSubset()
        {
            _initialSubset = new Subset<TElement>();
        }

        public SaveableSubset(Subset<TElement> targetSubset)
        {
            _differentialSubset = 
                new DifferentialSubset<TElement>(targetSubset);
        }

        public bool Add(TElement element)
        {
            return 
                _initialSubset != null
                    ? _initialSubset.Add(element) 
                    : _differentialSubset.Add(element);
        }

        public bool Remove(TElement element)
        {
            return
                _initialSubset != null
                    ? _initialSubset.Remove(element)
                    : _differentialSubset.Remove(element);
        }

        public bool Contains(TElement element)
        {
            return
                _initialSubset != null
                    ? _initialSubset.Contains(element)
                    : _differentialSubset.Contains(element);
        }

        public IEnumerable<TElement> Elements
        {
            get
            {
                return
                    _initialSubset != null
                        ? _initialSubset.Elements
                        : _differentialSubset.Elements;
            }
        }

        public int Count
        {
            get
            {
                return
                    _initialSubset != null
                        ? _initialSubset.Count
                        : _differentialSubset.Count;
            }
        }

        public bool IsNew
        {
            get { return _initialSubset != null; }
        }

        public void OnSaved(Subset<TElement> targetSubset)
        {
            if (_initialSubset == null)
            {
                _differentialSubset.Update();
                return;
            }

            _initialSubset = null;

            _differentialSubset = 
                new DifferentialSubset<TElement>(targetSubset);
        }

        public IEnumerable<TElement> AddedElements
        {
            get
            {
                return
                    _initialSubset != null
                        ? _initialSubset.Elements
                        : _differentialSubset.AddedElements;
            }
        }

        public IEnumerable<TElement> RemovedElements
        {
            get
            {
                return
                    _initialSubset != null
                        ? Enumerable.Empty<TElement>()
                        : _differentialSubset.RemovedElements;
            }
        }

        public void Update()
        {
            if (_initialSubset == null)
                _differentialSubset.Update();
        }

        public bool IsModified
        {
            get
            {
                return _initialSubset != null || _differentialSubset.IsModified;
            }
        }
    }
}