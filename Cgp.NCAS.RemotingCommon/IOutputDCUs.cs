using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IOutputs : IBaseOrmTable<Output>
    {
        Output GetByOutputNumber(DCU dcu, byte number);
        OutputState GetActualStates(Output output);
        OutputState GetRealStates(Output output);
        bool IsActivated(Output output);

        ICollection<OutputShort> ShortSelectByCriteria(ICollection<FilterSettings> filterSettings, out Exception error);
        ICollection<OutputShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings);
        OutputState GetActualStatesByGuid(Guid idOutput);
        OutputState GetRealStatesByGuid(Guid idOutput);

        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<IModifyObject> ListModifyObjectsFromCCU(Guid idCCU, out Exception error);
        IList<IModifyObject> ListModifyObjectsFromCCUNotUsedInDoorEnvironmentsAnaAlarmAreas(Guid idCCU, Guid idAlarmArea, out Exception error);
        IList<IModifyObject> ListModifyObjectsForAlarmAreaActivationObject(Guid idAlarmArea, out Exception error);
        IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjectsForOtputControlByObject(Guid idOutput, IList<FilterSettings> filterSettings, out Exception error);
        bool ExistOutputWithIndex(byte index, DCU dcu);
        bool ExistCcuOutputWithIndex(byte index, CCU ccu);
        IList<Output> FilterOutputsFromActivators(bool allOutputs, Guid ccuGuid, bool allowDoorEnvironment, Guid doorEnvironmentGuid);
        bool OutputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(Guid idOutput, Guid idAddindOutput);
        bool OutputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(Guid idAlarmArea, Guid idAddindOutput);
        Guid GetParentCCU(Guid idOutput);
    }
}
