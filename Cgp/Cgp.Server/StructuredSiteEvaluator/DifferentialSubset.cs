using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public interface IDifferentialSubset<TElement> : ISubset<TElement>
    {
        IEnumerable<TElement> AddedElements { get; }
        IEnumerable<TElement> RemovedElements { get; }

        void Update();
        bool IsModified { get; }
    }

    public class DifferentialSubset<TElement> : IDifferentialSubset<TElement>
    {
        private readonly Subset<TElement> _target;

        private HashSet<TElement> _addedElements = new HashSet<TElement>();
        private HashSet<TElement> _removedElements = new HashSet<TElement>();

        public DifferentialSubset(Subset<TElement> target)
        {
            _target = target;
        }

        public bool Add(TElement element)
        {
            if (_removedElements != null)
                if (_removedElements.Remove(element))
                    return true;

            if (_target.Contains(element))
                return false;

            if (_addedElements == null)
                _addedElements = new HashSet<TElement>();

            return _addedElements.Add(element);
        }

        public bool Remove(TElement element)
        {
            if (_addedElements != null)
                if (_addedElements.Remove(element))
                    return true;

            if (!_target.Contains(element))
                return false;

            if (_removedElements == null)
                _removedElements = new HashSet<TElement>();

            return _removedElements.Add(element);
        }

        public bool Contains(TElement element)
        {
            if (_removedElements != null)
                if (_removedElements.Contains(element))
                    return false;

            if (_addedElements != null)
                if (_addedElements.Contains(element))
                    return true;

            return _target.Contains(element);
        }

        public IEnumerable<TElement> Elements
        {
            get
            {
                return
                    _target.Elements
                        .Where(element => !_removedElements.Contains(element))
                        .Concat(_addedElements);
            }
        }

        public int Count
        {
            get { return _target.Count + _addedElements.Count - _removedElements.Count; }
        }

        public IEnumerable<TElement> AddedElements
        {
            get { return _addedElements; }
        }

        public IEnumerable<TElement> RemovedElements
        {
            get { return _removedElements; }
        }

        public void Update()
        {
            ICollection<TElement> unnecessaryElements =
                new LinkedList<TElement>(
                    _addedElements
                        .Where(element => _target.Contains(element)));

            foreach (var unnecessaryElement in unnecessaryElements)
                _addedElements.Remove(unnecessaryElement);

            unnecessaryElements.Clear();

            foreach (var removedElement in _removedElements)
                if (!_target.Contains(removedElement))
                    unnecessaryElements.Add(removedElement);

            foreach (var unnecessaryElement in unnecessaryElements)
                _removedElements.Remove(unnecessaryElement);
        }

        public bool IsModified
        {
            get { return _addedElements.Count > 0 || _removedElements.Count > 0; }
        }

        public void Reset()
        {
            _addedElements.Clear();
            _removedElements.Clear();
        }
    }
}