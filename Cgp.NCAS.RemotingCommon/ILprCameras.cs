using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface ILprCameras : IBaseOrmTable<LprCamera>
    {
        ICollection<LprCameraShort> ShortSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception error);

        ICollection<LprCameraShort> ShortSelectByCriteria(
            out Exception error,
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings);

        void LprCamerasLookup(Guid clientId);

        void CreateLookupedLprCameras(
            ICollection<LookupedLprCamera> lookupedCameras,
            int? idStructuredSubSite);
    }
}
