using System;
using System.Collections.Generic;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IAccessZones : IBaseOrmTable<AccessZone>
    {
        ICollection<AccessZone> GetAccessZonesByPerson(Guid idPerson, out Exception error);
    }
}
