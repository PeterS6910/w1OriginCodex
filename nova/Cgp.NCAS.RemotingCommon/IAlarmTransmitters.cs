using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IAlarmTransmitters : IBaseOrmTable<AlarmTransmitter>
    {
        ICollection<AlarmTransmitterShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error);

        ICollection<IModifyObject> ListModifyObjects(out Exception error);

        void AlarmTransmittersLookup(Guid clientId);

        void CreateLookupedAlarmTransmitters(ICollection<string> ipAddresses, int? idStructuredSubSite);

        OnlineState GetOnlineState(string ipAddress);
    }
}
