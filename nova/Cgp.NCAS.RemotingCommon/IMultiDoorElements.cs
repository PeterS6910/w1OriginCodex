using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IMultiDoorElements : IBaseOrmTable<MultiDoorElement>
    {
        ICollection<MultiDoorElementShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error);

        ICollection<IModifyObject> ListModifyObjects(out Exception error);

        ICollection<IModifyObject> ListModifyElevatorDoors(
            int floorNumber,
            IEnumerable<Guid> alreadyAddedEvelatorDoors,
            Guid? parentCcuId,
            out Exception error);

        ICollection<IModifyObject> ListNotUsedInputsModifyObjects(
            Guid? multiDoorElementId,
            Guid? ccuId,
            IEnumerable<Guid> usedInputs,
            out Exception error);

        bool IsInputUsedInDoorElement(Guid inputId, Guid? multiDoorElementId, IEnumerable<Guid> usedInputs);

        ICollection<IModifyObject> ListNotUsedOutputsModifyObjects(
            Guid? multiDoorElementId,
            Guid? ccuId,
            IEnumerable<Guid> usedOutputs,
            out Exception error);

        bool IsOutputUsedInDoorElement(Guid outputId, Guid? multiDoorElementId, IEnumerable<Guid> usedOutputs);
        DoorEnvironmentState GetMultiDoorElementState(Guid multiDoorElementId);
        ICollection<MultiDoorElement> GetMultiDoorElementsForCcu(Guid idCcu);
    }
}
