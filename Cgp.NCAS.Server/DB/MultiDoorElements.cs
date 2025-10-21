using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class MultiDoorElements :
        ANcasBaseOrmTable<MultiDoorElements, MultiDoorElement>,
        IMultiDoorElements
    {
        private MultiDoorElements()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<MultiDoorElement>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.MultiDoorElements), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.MultiDoorElements), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.MultiDoorElementsInsertDeletePerform), login);
        }

        protected override void LoadObjectsInRelationshipGetById(MultiDoorElement obj)
        {
            if (obj.MultiDoor != null)
                obj.MultiDoor =
                    MultiDoors.Singleton.GetById(obj.MultiDoor.IdMultiDoor);
        }

        protected override void LoadObjectsInRelationship(MultiDoorElement obj)
        {
            if (obj.MultiDoor != null)
                obj.MultiDoor =
                    MultiDoors.Singleton.GetById(obj.MultiDoor.IdMultiDoor);

            if (obj.Floor != null)
                obj.Floor = Floors.Singleton.GetById(obj.Floor.IdFloor);

            if (obj.BlockAlarmArea != null)
                obj.BlockAlarmArea = AlarmAreas.Singleton.GetById(obj.BlockAlarmArea.IdAlarmArea);

            if (obj.ElectricStrike != null)
                obj.ElectricStrike = Outputs.Singleton.GetById(obj.ElectricStrike.IdOutput);

            if (obj.ExtraElectricStrike != null)
                obj.ExtraElectricStrike = Outputs.Singleton.GetById(obj.ExtraElectricStrike.IdOutput);

            if (obj.SensorOpenDoor != null)
                obj.SensorOpenDoor = Inputs.Singleton.GetById(obj.SensorOpenDoor.IdInput);
        }

        public override bool CheckData(MultiDoorElement multiDoorElelemnt, out Exception error)
        {
            error = null;

            var multiDoorElementId = multiDoorElelemnt.IdMultiDoorElement != Guid.Empty
                ? (Guid?) multiDoorElelemnt.IdMultiDoorElement
                : null;

            if (multiDoorElelemnt.ElectricStrike != null)
            {
                if (IsOutputUsedInDoorElement(multiDoorElelemnt.ElectricStrike.IdOutput, multiDoorElementId, null))
                {
                    error = new IwQuick.SqlUniqueException(MultiDoorElement.ColumnElectricStrike);
                    return false;
                }
            }

            if (multiDoorElelemnt.ExtraElectricStrike != null)
            {
                if (IsOutputUsedInDoorElement(multiDoorElelemnt.ExtraElectricStrike.IdOutput, multiDoorElementId, null))
                {
                    error = new IwQuick.SqlUniqueException(MultiDoorElement.ColumnExtraElectricStrike);
                    return false;
                }
            }

            if (multiDoorElelemnt.SensorOpenDoor != null)
            {
                if (IsInputUsedInDoorElement(multiDoorElelemnt.SensorOpenDoor.IdInput, multiDoorElementId, null))
                {
                    error = new IwQuick.SqlUniqueException(MultiDoorElement.ColumnSensorOpenDoor);
                    return false;
                }
            }

            if (multiDoorElelemnt.MultiDoor != null)
            {
                var multiDoorElelemntsWithSameDoorIndex = multiDoorElementId != null
                    ? SelectLinq<MultiDoorElement>(
                        multiDoorElelemntFromDb =>
                            multiDoorElelemntFromDb.IdMultiDoorElement != multiDoorElementId.Value &&
                            multiDoorElelemntFromDb.MultiDoor != null &&
                            multiDoorElelemntFromDb.MultiDoor.IdMultiDoor == multiDoorElelemnt.MultiDoor.IdMultiDoor &&
                            multiDoorElelemntFromDb.DoorIndex == multiDoorElelemnt.DoorIndex)
                    : SelectLinq<MultiDoorElement>(
                        multiDoorElelemntFromDb =>
                            multiDoorElelemntFromDb.MultiDoor != null &&
                            multiDoorElelemntFromDb.MultiDoor.IdMultiDoor == multiDoorElelemnt.MultiDoor.IdMultiDoor &&
                            multiDoorElelemntFromDb.DoorIndex == multiDoorElelemnt.DoorIndex);

                if (multiDoorElelemntsWithSameDoorIndex != null &&
                    multiDoorElelemntsWithSameDoorIndex.Any())
                {
                    error = new IwQuick.SqlUniqueException(MultiDoorElement.ColumnDoorIndex);
                    return false;
                }
            }

            return true;
        }

        public override void AfterInsert(MultiDoorElement newObject)
        {
            AfterInsertUpdate(newObject, newObject.MultiDoor);
        }

        public override void AfterUpdate(MultiDoorElement newObject, MultiDoorElement oldObjectBeforUpdate)
        {
            AfterInsertUpdate(newObject, newObject.MultiDoor);
        }

        public void AfterInsertUpdate(MultiDoorElement multiDoorElement, MultiDoor multiDoor)
        {
            if (multiDoorElement.ElectricStrike != null)
            {
                var electricStrike = Outputs.Singleton.GetObjectForEdit(multiDoorElement.ElectricStrike.IdOutput);
                if (electricStrike != null)
                {
                    var wasChanged = false;
                    if (electricStrike.ControlType != (byte)OutputControl.unblocked)
                    {
                        electricStrike.ControlType = (byte)OutputControl.unblocked;
                        wasChanged = true;
                    }

                    if (multiDoor != null)
                    {
                        if (multiDoor.ElectricStrikeImpulse)
                        {
                            if (electricStrike.OutputType != (byte) OutputCharacteristic.pulsed)
                            {
                                electricStrike.OutputType = (byte) OutputCharacteristic.pulsed;
                                wasChanged = true;
                            }

                            if (electricStrike.SettingsPulseLength != multiDoor.ElectricStrikeImpulseDelay)
                            {
                                electricStrike.SettingsPulseLength = multiDoor.ElectricStrikeImpulseDelay;
                                wasChanged = true;
                            }
                        }
                        else
                        {
                            if (electricStrike.OutputType != (byte) OutputCharacteristic.level)
                            {
                                electricStrike.OutputType = (byte) OutputCharacteristic.level;
                                wasChanged = true;
                            }
                        }

                        if (electricStrike.SettingsDelayToOn != multiDoor.DoorDelayBeforeUnlock)
                        {
                            electricStrike.SettingsDelayToOn = multiDoor.DoorDelayBeforeUnlock;
                            wasChanged = true;
                        }

                        if (electricStrike.SettingsDelayToOff != multiDoor.DoorDelayBeforeLock)
                        {
                            electricStrike.SettingsDelayToOff = multiDoor.DoorDelayBeforeLock;
                            wasChanged = true;
                        }
                    }

                    if (wasChanged)
                        Outputs.Singleton.Update(electricStrike);

                    Outputs.Singleton.EditEnd(electricStrike);
                }
            }

            if (multiDoorElement.ExtraElectricStrike != null)
            {
                var extraElectricStrike =
                    Outputs.Singleton.GetObjectForEdit(multiDoorElement.ExtraElectricStrike.IdOutput);
                if (extraElectricStrike != null)
                {
                    var wasChanged = false;
                    if (extraElectricStrike.ControlType != (byte) OutputControl.unblocked)
                    {
                        extraElectricStrike.ControlType = (byte)OutputControl.unblocked;
                        wasChanged = true;
                    }

                    if (multiDoor != null)
                    {
                        if (multiDoor.ExtraElectricStrikeImpulse)
                        {
                            if (extraElectricStrike.OutputType != (byte) OutputCharacteristic.pulsed)
                            {
                                extraElectricStrike.OutputType = (byte) OutputCharacteristic.pulsed;
                                wasChanged = true;
                            }

                            if (extraElectricStrike.SettingsPulseLength !=
                                multiDoor.ExtraElectricStrikeImpulseDelay)
                            {
                                extraElectricStrike.SettingsPulseLength =
                                    multiDoor.ExtraElectricStrikeImpulseDelay;
                                wasChanged = true;
                            }
                        }
                        else
                        {
                            if (extraElectricStrike.OutputType != (byte) OutputCharacteristic.level)
                            {
                                extraElectricStrike.OutputType = (byte) OutputCharacteristic.level;
                                wasChanged = true;
                            }
                        }

                        if (extraElectricStrike.SettingsDelayToOn != multiDoor.DoorDelayBeforeUnlock)
                        {
                            extraElectricStrike.SettingsDelayToOn = multiDoor.DoorDelayBeforeUnlock;
                            wasChanged = true;
                        }

                        if (extraElectricStrike.SettingsDelayToOff != multiDoor.DoorDelayBeforeLock)
                        {
                            extraElectricStrike.SettingsDelayToOff = multiDoor.DoorDelayBeforeLock;
                            wasChanged = true;
                        }
                    }

                    if (wasChanged)
                        Outputs.Singleton.Update(extraElectricStrike);

                    Outputs.Singleton.EditEnd(extraElectricStrike);
                }
            }

            if (multiDoorElement.SensorOpenDoor != null)
            {
                var sensorOpenDoor = Inputs.Singleton.GetObjectForEdit(multiDoorElement.SensorOpenDoor.IdInput);
                if (sensorOpenDoor != null)
                {
                    var wasChanged = false;

                    if (sensorOpenDoor.DelayToOn != 0)
                    {
                        sensorOpenDoor.DelayToOn = 0;
                        wasChanged = true;
                    }

                    if (multiDoor != null)
                    {
                        if (sensorOpenDoor.Inverted != multiDoor.SensorsOpenDoorsInverted)
                        {
                            sensorOpenDoor.Inverted = multiDoor.SensorsOpenDoorsInverted;
                            wasChanged = true;
                        }


                        if (sensorOpenDoor.DelayToOff != multiDoor.DoorDelayBeforeClose)
                        {
                            sensorOpenDoor.DelayToOff = multiDoor.DoorDelayBeforeClose;
                            wasChanged = true;
                        }

                        if (multiDoor.SensorsOpenDoorsBalanced)
                        {
                            if (sensorOpenDoor.InputType != (byte) InputType.BSI)
                            {
                                sensorOpenDoor.InputType = (byte) InputType.BSI;
                                wasChanged = true;
                            }
                        }
                        else
                        {
                            if (sensorOpenDoor.InputType != (byte) InputType.DI)
                            {
                                sensorOpenDoor.InputType = (byte) InputType.DI;
                                wasChanged = true;
                            }
                        }
                    }

                    if (sensorOpenDoor.AlarmOn)
                    {
                        sensorOpenDoor.AlarmOn = false;
                        wasChanged = true;
                    }

                    if (sensorOpenDoor.AlarmTamper)
                    {
                        sensorOpenDoor.AlarmTamper = false;
                        wasChanged = true;
                    }

                    if (wasChanged)
                        Inputs.Singleton.Update(sensorOpenDoor);

                    Inputs.Singleton.EditEnd(sensorOpenDoor);
                }
            }
        }

        public override void CUDSpecial(
            MultiDoorElement multiDoorElement,
            ObjectDatabaseAction objectDatabaseAction)
        {
            if (multiDoorElement == null ||
                multiDoorElement.MultiDoor == null)
            {
                return;
            }

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        multiDoorElement.MultiDoor.GetId(),
                        multiDoorElement.MultiDoor.GetObjectType()));

                return;
            }

            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(multiDoorElement.MultiDoor);
                return;
            }

            DataReplicationManager.Singleton.SendModifiedObjectToCcus(multiDoorElement);
        }

        public ICollection<MultiDoorElementShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            var list = SelectByCriteria(filterSettings, out error);

            if (list != null)
            {
                return
                    new LinkedList<MultiDoorElementShort>(
                        list.Select(
                            multiDoorElement =>
                                new MultiDoorElementShort(multiDoorElement)
                                {
                                    State = GetMultiDoorElementState(multiDoorElement.IdMultiDoorElement)
                                }));
            }

            return null;
        }

        public ICollection<IModifyObject> ListModifyElevatorDoors(
            int floorNumber,
            IEnumerable<Guid> alreadyAddedElevatorDoors,
            Guid? parentCcuId,
            out Exception error)
        {
            var multiDoorElements = List(out error) as IEnumerable<MultiDoorElement>;

            if (multiDoorElements == null)
                return null;

            multiDoorElements =
                multiDoorElements.Where(
                    multiDoorElement => multiDoorElement.DoorIndex == floorNumber &&
                                        multiDoorElement.MultiDoor.Type == MultiDoorType.Elevator);

            if (alreadyAddedElevatorDoors != null)
            {
                multiDoorElements =
                    multiDoorElements.Where(
                        multiDoorElement =>
                            !alreadyAddedElevatorDoors.Contains(multiDoorElement.IdMultiDoorElement));
            }

            if (parentCcuId != null)
                multiDoorElements =
                    multiDoorElements.Where(
                        multiDoorElement =>
                            MultiDoors.Singleton.GetParentCcuId(multiDoorElement.MultiDoor.IdMultiDoor) ==
                            parentCcuId);

            return
                new LinkedList<IModifyObject>(
                    multiDoorElements
                        .Select(multiDoorElement => new MultiDoorElementModObj(multiDoorElement))
                        .OrderBy(multiDoorElementModObj => multiDoorElementModObj.ToString())
                        .Cast<IModifyObject>());
        }

        public ICollection<IModifyObject> ListModifyObjects(out Exception error)
        {
            var multiDoorElements = List(out error);

            if (multiDoorElements == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    multiDoorElements.Select(
                        multiDoorElement => new MultiDoorElementModObj(multiDoorElement))
                        .OrderBy(multiDoorElementModObj => multiDoorElementModObj.ToString())
                        .Cast<IModifyObject>());
        }

        public ICollection<IModifyObject> ListNotUsedInputsModifyObjects(
            Guid? multiDoorElementId,
            Guid? ccuId,
            IEnumerable<Guid> usedInputs,
            out Exception error)
        {
            var inputs = Inputs.Singleton.List(out error) as IEnumerable<Input>;

            if (inputs == null)
                return null;

            if (ccuId != null)
            {
                inputs =
                    inputs.Where(input => Inputs.Singleton.GetParentCCU(input.IdInput) == ccuId);
            }

            var usedInputsInDoorElements = GetInputsUsedInDoorElements(multiDoorElementId, usedInputs);

            return
                new LinkedList<IModifyObject>(
                    inputs.Where(input => !usedInputsInDoorElements.Contains(input.IdInput))
                        .Select(input => new InputModifyObj(input))
                        .OrderBy(inputShort => inputShort.ToString())
                        .Cast<IModifyObject>());
        }

        private HashSet<Guid> GetInputsUsedInDoorElements(Guid? multiDoorElementId, IEnumerable<Guid> usedInputs)
        {
            var doorEnvironments =
                SelectLinq<DoorEnvironment>(doorEnvironemnt => doorEnvironemnt.SensorsLockDoors != null ||
                                                               doorEnvironemnt.SensorsOpenDoors != null ||
                                                               doorEnvironemnt.SensorsOpenMaxDoors != null ||
                                                               doorEnvironemnt.PushButtonInternal != null ||
                                                               doorEnvironemnt.PushButtonExternal != null);

            var alarmAreas = SelectLinq<AlarmArea>(alarmArea => alarmArea.ActivationStateInputEIS != null);

            var multiDoorElements = multiDoorElementId != null
                ? SelectLinq<MultiDoorElement>(
                    multiDoorElement =>
                        multiDoorElement.IdMultiDoorElement != multiDoorElementId &&
                        multiDoorElement.SensorOpenDoor != null)
                : SelectLinq<MultiDoorElement>(multiDoorElement => multiDoorElement.SensorOpenDoor != null);

            if (usedInputs == null)
                usedInputs = Enumerable.Empty<Guid>();

            if (doorEnvironments != null)
                usedInputs =
                    usedInputs
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.SensorsLockDoors != null)
                            .Select(doorEnvironment => doorEnvironment.SensorsLockDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.SensorsOpenDoors != null)
                            .Select(doorEnvironment => doorEnvironment.SensorsOpenDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.SensorsOpenMaxDoors != null)
                            .Select(doorEnvironment => doorEnvironment.SensorsOpenMaxDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.PushButtonInternal != null)
                            .Select(doorEnvironment => doorEnvironment.PushButtonInternal.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.PushButtonExternal != null)
                            .Select(doorEnvironment => doorEnvironment.PushButtonExternal.IdInput));

            if (alarmAreas != null)
                usedInputs =
                    usedInputs
                        .Concat(alarmAreas
                            .Select(alarmArea => alarmArea.ActivationStateInputEIS.Value));

            if (multiDoorElements != null)
                usedInputs =
                    usedInputs
                        .Concat(multiDoorElements
                            .Select(multiDoorElement => multiDoorElement.SensorOpenDoor.IdInput));

            return new HashSet<Guid>(usedInputs);
        }

        public bool IsInputUsedInDoorElement(Guid inputId, Guid? multiDoorElementId, IEnumerable<Guid> usedInputs)
        {
            return GetInputsUsedInDoorElements(multiDoorElementId, usedInputs).Contains(inputId);
        }

        public bool IsInputUsedOnlyInDoorElement(Guid inputId)
        {
            var multiDoorElements = SelectLinq<MultiDoorElement>(
                multiDoorElement =>
                    multiDoorElement.SensorOpenDoor != null &&
                    multiDoorElement.SensorOpenDoor.IdInput == inputId);

            return multiDoorElements != null && multiDoorElements.Count > 0;
        }

        public ICollection<IModifyObject> ListNotUsedOutputsModifyObjects(
            Guid? multiDoorElementId,
            Guid? ccuId,
            IEnumerable<Guid> usedOutputs,
            out Exception error)
        {
            var outputs = Outputs.Singleton.List(out error) as IEnumerable<Output>;

            if (outputs == null)
                return null;

            if (ccuId != null)
            {
                outputs =
                    outputs.Where(output => Outputs.Singleton.GetParentCCU(output.IdOutput) == ccuId);
            }

            var usedOutputsInDoorElements = GetOutputsUsedInDoorElements(multiDoorElementId, usedOutputs);

            return
                new LinkedList<IModifyObject>(
                    outputs.Where(output => !usedOutputsInDoorElements.Contains(output.IdOutput))
                        .Select(output => new OutputModifyObj(output))
                        .OrderBy(outputShort => outputShort.ToString())
                        .Cast<IModifyObject>());
        }

        private HashSet<Guid> GetOutputsUsedInDoorElements(Guid? multiDoorElementId, IEnumerable<Guid> usedOutputs)
        {
            var doorEnvironments =
                SelectLinq<DoorEnvironment>(
                    doorEnvironemnt => doorEnvironemnt.ActuatorsElectricStrike != null ||
                                       doorEnvironemnt.ActuatorsElectricStrikeOpposite != null ||
                                       doorEnvironemnt.ActuatorsExtraElectricStrike != null ||
                                       doorEnvironemnt.ActuatorsExtraElectricStrikeOpposite != null ||
                                       doorEnvironemnt.ActuatorsBypassAlarm != null ||
                                       doorEnvironemnt.DoorAjarOutput != null ||
                                       doorEnvironemnt.IntrusionOutput != null ||
                                       doorEnvironemnt.SabotageOutput != null);

            var alarmAreas = SelectLinq<AlarmArea>(
                alarmArea =>
                    alarmArea.OutputActivation != null
                    || alarmArea.OutputAlarmState != null
                    || alarmArea.OutputPrewarning != null
                    || alarmArea.OutputTmpUnsetEntry != null
                    || alarmArea.OutputTmpUnsetExit != null
                    || alarmArea.OutputAAlarm != null
                    || alarmArea.OutputSiren != null
                    || alarmArea.OutputSabotage != null
                    || alarmArea.OutputNotAcknowledged != null
                    || alarmArea.OutputMotion != null
                    || alarmArea.SetUnsetOutputEIS != null
                    || alarmArea.OutputSetByObjectForAaFailed != null);

            var multiDoorElements = multiDoorElementId != null
                ? SelectLinq<MultiDoorElement>(
                    multiDoorElement =>
                        multiDoorElement.IdMultiDoorElement != multiDoorElementId &&
                        (multiDoorElement.ElectricStrike != null ||
                         multiDoorElement.ExtraElectricStrike != null))
                : SelectLinq<MultiDoorElement>(
                    multiDoorElement =>
                        multiDoorElement.ElectricStrike != null ||
                        multiDoorElement.ExtraElectricStrike != null);

            if (usedOutputs == null)
                usedOutputs = Enumerable.Empty<Guid>();

            if (doorEnvironments != null)
                usedOutputs =
                    usedOutputs
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.ActuatorsElectricStrike != null)
                            .Select(doorEnvironment => doorEnvironment.ActuatorsElectricStrike.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.ActuatorsElectricStrikeOpposite != null)
                            .Select(doorEnvironment => doorEnvironment.ActuatorsElectricStrikeOpposite.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.ActuatorsExtraElectricStrike != null)
                            .Select(doorEnvironment => doorEnvironment.ActuatorsExtraElectricStrike.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.ActuatorsExtraElectricStrikeOpposite != null)
                            .Select(doorEnvironment => doorEnvironment.ActuatorsExtraElectricStrikeOpposite.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.ActuatorsBypassAlarm != null)
                            .Select(doorEnvironment => doorEnvironment.ActuatorsBypassAlarm.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.DoorAjarOutput != null)
                            .Select(doorEnvironment => doorEnvironment.DoorAjarOutput.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.IntrusionOutput != null)
                            .Select(doorEnvironment => doorEnvironment.IntrusionOutput.IdOutput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironment => doorEnvironment.SabotageOutput != null)
                            .Select(doorEnvironment => doorEnvironment.SabotageOutput.IdOutput));

            if (alarmAreas != null)
                usedOutputs =
                    usedOutputs
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputActivation != null)
                            .Select(alarmArea => alarmArea.OutputActivation.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputAlarmState != null)
                            .Select(alarmArea => alarmArea.OutputAlarmState.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputPrewarning != null)
                            .Select(alarmArea => alarmArea.OutputPrewarning.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputTmpUnsetEntry != null)
                            .Select(alarmArea => alarmArea.OutputTmpUnsetEntry.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputTmpUnsetExit != null)
                            .Select(alarmArea => alarmArea.OutputTmpUnsetExit.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputAAlarm != null)
                            .Select(alarmArea => alarmArea.OutputAAlarm.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputSiren != null)
                            .Select(alarmArea => alarmArea.OutputSiren.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputSabotage != null)
                            .Select(alarmArea => alarmArea.OutputSabotage.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputNotAcknowledged != null)
                            .Select(alarmArea => alarmArea.OutputNotAcknowledged.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputMotion != null)
                            .Select(alarmArea => alarmArea.OutputMotion.IdOutput))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.SetUnsetOutputEIS != null)
                            .Select(alarmArea => alarmArea.SetUnsetOutputEIS.Value))
                        .Concat(alarmAreas
                            .Where(alarmArea => alarmArea.OutputSetByObjectForAaFailed != null)
                            .Select(alarmArea => alarmArea.OutputSetByObjectForAaFailed.IdOutput));

            if (multiDoorElements != null)
                usedOutputs =
                    usedOutputs
                        .Concat(multiDoorElements
                            .Where(multiDoorElement => multiDoorElement.ElectricStrike != null)
                            .Select(multiDoorElement => multiDoorElement.ElectricStrike.IdOutput))
                        .Concat(multiDoorElements
                            .Where(multiDoorElement => multiDoorElement.ExtraElectricStrike != null)
                            .Select(multiDoorElement => multiDoorElement.ExtraElectricStrike.IdOutput));

            return new HashSet<Guid>(usedOutputs);
        }

        public bool IsOutputUsedInDoorElement(Guid outputId, Guid? multiDoorElementId, IEnumerable<Guid> usedOutputs)
        {
            return GetOutputsUsedInDoorElements(multiDoorElementId, usedOutputs).Contains(outputId);
        }

        public bool IsOutputUsedOnlyInDoorElement(Guid outputId)
        {
            var multiDoorElements = SelectLinq<MultiDoorElement>(
                multiDoorElement =>
                    (multiDoorElement.ElectricStrike != null &&
                     multiDoorElement.ElectricStrike.IdOutput == outputId) ||
                    (multiDoorElement.ExtraElectricStrike != null &&
                     multiDoorElement.ExtraElectricStrike.IdOutput == outputId));

            return multiDoorElements != null && multiDoorElements.Count > 0;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                var multiDoorElements = List();

                return multiDoorElements != null
                    ? new LinkedList<AOrmObject>(
                        multiDoorElements
                            .Select(multiDoorElement =>
                            {
                                LoadObjectsInRelationshipGetById(multiDoorElement);
                                return multiDoorElement;
                            })
                            .OrderBy(multiDoorElement => multiDoorElement.Name).Cast<AOrmObject>())
                    : null;
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<MultiDoorElement>(
                        multiDoorElement => multiDoorElement.Name.IndexOf(name) >= 0)
                    : SelectLinq<MultiDoorElement>(
                        multiDoorElement =>
                            multiDoorElement.Name.IndexOf(name) >= 0 ||
                            multiDoorElement.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult
                        .Select(multiDoorElement =>
                        {
                            LoadObjectsInRelationshipGetById(multiDoorElement);
                            return multiDoorElement;
                        })
                        .OrderBy(multiDoorElement => multiDoorElement.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<MultiDoorElement> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<MultiDoorElement>(
                        multiDoorElement =>
                            multiDoorElement.Name.IndexOf(name) >= 0 ||
                            multiDoorElement.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult
                        .Select(multiDoorElement =>
                        {
                            LoadObjectsInRelationshipGetById(multiDoorElement);
                            return multiDoorElement;
                        })
                        .OrderBy(multiDoorElement => multiDoorElement.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<MultiDoorElement>(
                        multiDoorElement => multiDoorElement.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult
                        .Select(multiDoorElement =>
                        {
                            LoadObjectsInRelationshipGetById(multiDoorElement);
                            return multiDoorElement;
                        })
                        .OrderBy(multiDoorElement => multiDoorElement.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public void GetParentCCU(ICollection<Guid> ccus, Guid multiDoorElementId)
        {
            var multiDoorElement = GetById(multiDoorElementId);
            if (ccus != null && multiDoorElement != null)
            {
                MultiDoors.Singleton.GetParentCCU(ccus, multiDoorElement.MultiDoor);
            }
        }

        public Guid GetParentCcuId(Guid multiDoorElementId)
        {
            var multiDoorElement = GetById(multiDoorElementId);
            return MultiDoors.Singleton.GetParentCcuId(multiDoorElement.MultiDoor);
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid multiDoorElementId)
        {
            var objects = new List<AOrmObject>();

            var multiDoorElement = GetById(multiDoorElementId);
            if (multiDoorElement != null)
            {
                if (multiDoorElement.BlockOnOffObjectType != null &&
                    multiDoorElement.BlockOnOffObjectType != ObjectType.Input &&
                    multiDoorElement.BlockOnOffObjectType != ObjectType.Output &&
                    multiDoorElement.BlockOnOffObjectId != null)
                {
                    var ormObject =
                        CgpServerRemotingProvider.Singleton.GetTableObject(multiDoorElement.BlockOnOffObjectType.Value,
                            multiDoorElement.BlockOnOffObjectId.Value.ToString());

                    if (ormObject != null)
                    {
                        objects.Add(ormObject);
                    }
                }
            }

            return objects;
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(MultiDoorElement multiDoorElement)
        {
            if (multiDoorElement.ElectricStrike != null)
            {
                yield return multiDoorElement.ElectricStrike;
            }

            if (multiDoorElement.ExtraElectricStrike != null)
            {
                yield return multiDoorElement.ExtraElectricStrike;
            }
            
            if (multiDoorElement.SensorOpenDoor != null)
            {
                yield return multiDoorElement.SensorOpenDoor;
            }

            if (multiDoorElement.BlockAlarmArea != null)
            {
                yield return multiDoorElement.BlockAlarmArea;
            }

            if (multiDoorElement.BlockOnOffObjectType != null && multiDoorElement.BlockOnOffObjectId != null)
            {
                CgpServerRemotingProvider.Singleton.GetTableObject(
                    multiDoorElement.BlockOnOffObjectType.Value,
                    multiDoorElement.BlockOnOffObjectId.ToString());
            }
        }

        public IEnumerable<MultiDoorElement> GetMultiDoorElementsForAlarmArea(Guid alarmAreaId)
        {
            return SelectLinq<MultiDoorElement>(
                multiDoorElement =>
                    multiDoorElement.BlockAlarmArea != null &&
                    multiDoorElement.BlockAlarmArea.IdAlarmArea == alarmAreaId);
        }

        public IEnumerable<MultiDoorElement> GetMultiDoorElementsForOnOffObject(AOnOffObject onOffObject)
        {
            if (onOffObject == null)
                return null;

            return SelectLinq<MultiDoorElement>(
                multiDoorElement =>
                    multiDoorElement.BlockOnOffObjectType != null &&
                     multiDoorElement.BlockOnOffObjectId != null &&
                     multiDoorElement.BlockOnOffObjectType == onOffObject.GetObjectType() &&
                     multiDoorElement.BlockOnOffObjectId == onOffObject.GetId() as Guid?);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var multiDoorElement = GetById(idObj);
                if (multiDoorElement == null)
                    return null;

                var result = new LinkedList<AOrmObject>();
                if (multiDoorElement.MultiDoor != null)
                {
                    result.AddLast(multiDoorElement.MultiDoor);
                }

                if (multiDoorElement.Floor != null)
                {
                    result.AddLast(multiDoorElement.Floor);
                }
                
                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(
                    multiDoorElement.IdMultiDoorElement,
                    ObjectType.MultiDoorElement);
                
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.AddLast(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(
                    multiDoorElement.IdMultiDoorElement,
                    ObjectType.MultiDoorElement);

                if (usedInAz != null)
                {
                    foreach (var person in usedInAz)
                    {
                        var outPerson = Persons.Singleton.GetById(person.IdPerson);
                        result.AddLast(outPerson);
                    }
                }

                return
                    result.Count > 0
                        ? result.OrderBy(orm => orm.ToString()).ToList()
                        : null;
            }
            catch
            {
                return null;
            }
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var multiDoorElement = ormObject as MultiDoorElement;

            if (multiDoorElement == null)
                return null;

            return multiDoorElement.MultiDoor;
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            if (referencedObjects == null || referencedObjects.Count == 0)
                return true;

            return referencedObjects.All(ormObject => ormObject.GetObjectType() == ObjectType.MultiDoor);
        }

        public DoorEnvironmentState GetMultiDoorElementState(Guid multiDoorElementId)
        {
            return CCUConfigurationHandler.Singleton.GetDoorEnvironmentState(
                GetParentCcuId(multiDoorElementId),
                multiDoorElementId);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.MultiDoorElement; }
        }

        public ICollection<MultiDoorElement> GetMultiDoorElementsForCcu(Guid idCcu)
        {
            var multiDoorElements = SelectLinq<MultiDoorElement>(
                multiDoorElement =>
                    multiDoorElement.MultiDoor != null
                    && multiDoorElement.MultiDoor.CardReader != null
                    && ((multiDoorElement.MultiDoor.CardReader.CCU != null
                         && multiDoorElement.MultiDoor.CardReader.CCU.IdCCU == idCcu)
                        || (multiDoorElement.MultiDoor.CardReader.DCU != null
                            && multiDoorElement.MultiDoor.CardReader.DCU.CCU != null
                            && multiDoorElement.MultiDoor.CardReader.DCU.CCU.IdCCU == idCcu)));

            if (multiDoorElements == null)
                return null;

            return new LinkedList<MultiDoorElement>(
                multiDoorElements.Select(
                    multiDoorElement =>
                        GetById(multiDoorElement.IdMultiDoorElement)));
        }
    }
}
