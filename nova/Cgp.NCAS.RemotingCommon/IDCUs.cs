using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IDCUs : IBaseOrmTable<DCU>
    {
        OnlineState GetOnlineStates(DCU dcu);
        string GetPhysicalAddress(DCU dcu);
        string GetFirmwareVersion(Guid guidDcu);
        State GetInputsSabotageState(Guid dcuId);
        bool CheckLogicalAddress(DCU dcu);
        int GetDCUUpgradePercentage(Guid guidCCU, byte logicalAddressDCU);

        ICollection<DCUShort> SvDcuSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        OnlineState GetOnlineStates(Guid idDcu);

        IList<DcuListObj> GetListObj(out Exception error);

        IList<IModifyObject> ListModifyObjects(out Exception error);

        bool Reset(Guid ccuGuid, byte dcuLogicalAddress);

        ICollection<IModifyObject> GetObjectsToRename(Guid idDcu);
    }
}
