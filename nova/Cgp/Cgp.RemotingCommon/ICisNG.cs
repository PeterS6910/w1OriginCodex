using System;
using System.Collections.Generic;

using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICisNGs
    {
        bool Insert(Contal.Cgp.Server.Beans.CisNG obj);
        bool Delete(CisNG obj);
        System.Collections.Generic.ICollection<CisNG> List();
        System.Collections.Generic.ICollection<CisNG> SelectByCriteria(IList<FilterSettings> filterSettings);
        bool Update(CisNG obj);
        CisNG CreateCisNG();
    }
}
