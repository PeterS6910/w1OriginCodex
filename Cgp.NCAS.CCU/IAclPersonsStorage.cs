using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAclPersonsStorage
    {
        IEnumerable<Guid> GetAclIdsForPerson(Guid idPerson);
    }
}
