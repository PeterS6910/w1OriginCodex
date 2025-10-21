using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IInputs : IBaseOrmTable<Input>
    {
        InputState GetActualStates(Input input);
        bool IsActivated(Input input);

        ICollection<InputShort> ShortSelectByCriteria(ICollection<FilterSettings> filterSettings, out Exception error);
        ICollection<InputShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings);
        InputState GetActualStatesByGuid(Guid idInput);

        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<IModifyObject> ListModifyObjectsFromCCU(Guid idCCU, out Exception error);
        IList<IModifyObject> ListModifyObjectsFromCCUNotUsedInDoorEnvironments(Guid idCCU, Guid idAlarmArea, out Exception error);
        IList<IModifyObject> ListModifyObjectsForOtputControlByObject(Guid idCCU, out Exception error);
        IList<IModifyObject> ListModifyObjectsForAlarmAreaActivationObject(Guid idAlarmArea, out Exception error);
        IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error, Guid guidImplicitCCU);
        bool ExistInputWithIndex(byte index, DCU dcu);
        bool ExistCcuInputWithIndex(byte index, CCU ccu);
        bool InputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(Guid idOutput, Guid idAddinInput);
        bool InputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(Guid idAlarmArea, Guid idAddindInput);
        Guid GetParentCCU(Guid idInput);

        ICollection<Input> SelectByCriteriaFullInputs(
            out Exception error,
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings);
    }
}
