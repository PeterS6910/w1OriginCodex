using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IDoorEnvironments : IBaseOrmTable<DoorEnvironment>
    {
        ICollection<DoorEnvironmentShort> GetShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        bool IsInputInDoorEnvironments(Guid inputGuid);
        bool IsOutputInDoorEnvironmentsActuators(Output output);
        ICollection<Input> GetInputsNotInDoorEnvironments(Guid doorEnvironmentGuid);
        IList<Output> GetAllCCUOutputsNotInDoorEnvironmentsActuators(Guid doorEnvironmentGuid);
        IList<Output> GetNotUsedOutputs(Guid doorEnvironmentGuid);
        bool HasCardReaderDoorEnvironment(Guid idCardReader);
        ICollection<CardReader> GetCardReadersNotInDoorEnvironments(Guid doorEnvironmentGuid);
        bool? DoorEnvironmentAccessGranted(DoorEnvironment doorEnvironment);
        DoorEnvironmentState GetDoorEnvironmentState(Guid doorEnvironmentId);
        bool UnconfigureDoorEnvironments(DoorEnvironment doorEnvironment);
        AOrmObject GetInputDoorEnvironment(Guid inputGuid);
        AOrmObject GetOutputDoorEnvironment(Guid outputGuid);
        DoorEnvironment CreateNewForDCU();
        bool HasAccessToAccessGranted(Guid doorEnvironmentId);
        bool AllowToConfigureDoorEnvironment();

        //ICollection<DCUShort> SvDcuSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
        IList<IModifyObject> ListModifyObjectsOnlyFromCCUs(out Exception error);

        bool UsedSensorInAnotherDoorEnvironment(Guid idInput, Guid idDoorEnvironment);
        bool UsedActuatorInAnotherDoorEnvironment(Guid idOutput, Guid idDoorEnvironment);
        bool UsedCardReaderInAnotherDoorEnvironment(Guid idCardReader, Guid idDoorEnvironment);
        bool UsedPushButtonInAnotherDoorEnvironment(Guid idInput, Guid idDoorEnvironment);
        bool UsedSensorsInAnotherDoorEnvironment(Guid idOutput, Guid idDoorEnvironment);
        SecurityLevel? GetSecondCardReaderSecurityLevelUsedInDoorEnvironmentsWithCardReader(Guid cardReaderGuid);
    }

}
