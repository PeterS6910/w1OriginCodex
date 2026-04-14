using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IMultiDoors : IBaseOrmTable<MultiDoor>
    {
        ICollection<MultiDoorShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error);

        ICollection<IModifyObject> ListModifyObjects(Guid? ccuId, out Exception error);

        ICollection<IModifyObject> ListNotUsedCardReadersModifyObjects(
            Guid? multiDoorId,
            Guid? ccuId,
            out Exception error);

        bool IsCardReaderUsedInDoorElement(Guid cardReaderId, Guid? multiDoorId);
        bool IsCardReaderUsedInMultiDoor(Guid cardReaderId);
        Guid? GetParentCcuId(Guid multiDoorEnvironmentId);
        
        void CreateDoorsForMultiDoor(
            Guid multiDoorId,
            string doorLocalization,
            int doorsCount);

        void CreateDoorsForElevator(
            Guid multiDoorId,
            string floorLocalization,
            int lowestDoorIndex,
            int doorsCount,
            bool assignToFloors,
            bool createFloorIfNotExist);
    }
}
