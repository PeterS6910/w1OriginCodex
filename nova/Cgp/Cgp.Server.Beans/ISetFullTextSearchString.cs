using System.Collections.Generic;

namespace Contal.Cgp.Server.Beans
{
    public interface ISetFullTextSearchString
    {
        string FullTextSearchString { set; }
        string Name { get; }
        string AlternateName { get; }
        IEnumerable<string> OtherFullTextSearchStrings { get; }
    }
}
