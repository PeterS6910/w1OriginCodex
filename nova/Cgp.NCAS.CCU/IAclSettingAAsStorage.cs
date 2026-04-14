using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.DB;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAclSettingAAsStorage
    {
        IEnumerable<ACLSettingAA> GetAclSettingAAs(Guid idPerson, Guid idAlamrArea);
        IEnumerable<ACLSettingAA> GetAclSettingAAs(Guid idPerson);
    }
}
