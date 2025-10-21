using System.Collections.Generic;

namespace Contal.Cgp.Server.StructuredSiteEvaluator
{
    public interface IReadOnlySubSet<TElement>
    {
        bool Contains(TElement element);

        IEnumerable<TElement> Elements { get; }
        int Count { get; }
    }

    public interface ISubset<TElement> : IReadOnlySubSet<TElement>
    {
        bool Add(TElement element);
        bool Remove(TElement element);
    }
}