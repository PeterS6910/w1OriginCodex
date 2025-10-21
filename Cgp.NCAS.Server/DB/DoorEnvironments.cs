using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;
using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class DoorEnvironments :
        ANcasBaseOrmTableWithAlarmInstruction<DoorEnvironments, DoorEnvironment>, 
        IDoorEnvironments
    {
        private DoorEnvironments()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<DoorEnvironment>())
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var doorEnvironment = ormObject as DoorEnvironment;

            return
                doorEnvironment != null
                    ? (doorEnvironment.DCU != null
                        ? (AOrmObject)doorEnvironment.DCU
                        : doorEnvironment.CCU)
                    : null;
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(DoorEnvironment doorEnvironment)
        {
            yield return doorEnvironment.SensorsLockDoors;
            yield return doorEnvironment.SensorsOpenDoors;
            yield return doorEnvironment.SensorsOpenMaxDoors;
            yield return doorEnvironment.PushButtonInternal;
            yield return doorEnvironment.PushButtonExternal;

            yield return doorEnvironment.ActuatorsElectricStrike;
            yield return doorEnvironment.ActuatorsElectricStrikeOpposite;
            yield return doorEnvironment.ActuatorsExtraElectricStrike;
            yield return doorEnvironment.ActuatorsExtraElectricStrikeOpposite;
            yield return doorEnvironment.ActuatorsBypassAlarm;

            yield return doorEnvironment.IntrusionPresentationGroup;
            yield return doorEnvironment.DoorAjarPresentationGroup;
            yield return doorEnvironment.SabotagePresentationGroup;

            if (doorEnvironment.DoorEnvironmentAlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>();

                foreach (var doorEnvironmentAlarmArc in doorEnvironment.DoorEnvironmentAlarmArcs)
                {
                    if (!addedAlarmArcIds.Add(doorEnvironmentAlarmArc.AlarmArc.IdAlarmArc))
                        continue;

                    yield return doorEnvironmentAlarmArc.AlarmArc;
                }
            }
        }

        protected override IModifyObject CreateModifyObject(DoorEnvironment ormbObject)
        {
            return new DoorEnvironmentModifyObj(ormbObject);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.DOOR_ENVIRONMENTS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.DoorEnvironmentsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.DOOR_ENVIRONMENTS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.DoorEnvironmentsInsertDeletePerform), login);
        }

        public override void CUDSpecial(DoorEnvironment doorEnvironment, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        doorEnvironment.GetId(),
                        doorEnvironment.GetObjectType()));
            }
            else if (doorEnvironment != null)
            {
                if (objectDatabaseAction == ObjectDatabaseAction.Insert
                    && doorEnvironment.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetObjectForEdit(doorEnvironment.DCU.IdDCU);
                    DCUs.Singleton.Update(dcu);
                    DCUs.Singleton.EditEnd(dcu);

                    return;
                }

                DataReplicationManager.Singleton.SendModifiedObjectToCcus(doorEnvironment);
            }
        }

        private IEnumerable<Output> GetUsedOutputs(DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.ActuatorsElectricStrike != null)
                yield return doorEnvironment.ActuatorsElectricStrike;

            if (doorEnvironment.ActuatorsElectricStrikeOpposite != null)
                yield return doorEnvironment.ActuatorsElectricStrikeOpposite;

            if (doorEnvironment.ActuatorsExtraElectricStrike != null)
                yield return doorEnvironment.ActuatorsExtraElectricStrike;

            if (doorEnvironment.ActuatorsExtraElectricStrikeOpposite != null)
                yield return doorEnvironment.ActuatorsExtraElectricStrikeOpposite;

            if (doorEnvironment.ActuatorsBypassAlarm != null)
                yield return doorEnvironment.ActuatorsBypassAlarm;
        }

        private bool NeedToResetOutput(Output output)
        {
            return (output.SettingsDelayToOn != 0
                    || output.SettingsDelayToOff != 0
                    || output.OutputType != (byte) OutputType.Level
                    || output.ControlType != (byte) OutputControl.unblocked
                    || output.SettingsPulseLength != 0
                    || output.SettingsPulseDelay != 0
                    || output.SettingsForcedToOff);
        }

        protected override IEnumerable<DoorEnvironment> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    doorEnvironment.LocalAlarmInstruction != null
                    && doorEnvironment.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterUpdate(DoorEnvironment newDoorEnvironment,
            DoorEnvironment oldDoorEnvironmentBeforeUpdate)
        {
            if (newDoorEnvironment == null
                || oldDoorEnvironmentBeforeUpdate == null)
            {
                return;
            }

            base.AfterUpdate(
                newDoorEnvironment,
                oldDoorEnvironmentBeforeUpdate);

            var oldOutputs = new LinkedList<Output>(GetUsedOutputs(oldDoorEnvironmentBeforeUpdate));

            var newOutputsIds = new HashSet<Guid>(GetUsedOutputs(newDoorEnvironment)
                .Select(
                    output =>
                        output.IdOutput));

            foreach (var oldOutput in oldOutputs)
            {
                if (newOutputsIds.Contains(oldOutput.IdOutput))
                    continue;

                ResetOutput(oldOutput);
            }

            if (newDoorEnvironment.SensorsOpenDoors != null)
            {
                SetDelayToOnOffForInput(
                    newDoorEnvironment.SensorsOpenDoors,
                    newDoorEnvironment.DoorDelayBeforeBreakIn,
                    newDoorEnvironment.DoorDelayBeforeClose);
            }

            if (oldDoorEnvironmentBeforeUpdate.SensorsOpenDoors != null
                && (newDoorEnvironment.SensorsOpenDoors == null
                    || !newDoorEnvironment.SensorsOpenDoors.IdInput.Equals(
                        oldDoorEnvironmentBeforeUpdate.SensorsOpenDoors.IdInput)))
            {
                SetDelayToOnOffForInput(
                    oldDoorEnvironmentBeforeUpdate.SensorsOpenDoors,
                    0,
                    0);
            }

            if (newDoorEnvironment.SensorsOpenMaxDoors != null)
            {
                SetDelayToOnOffForInput(
                    newDoorEnvironment.SensorsOpenMaxDoors,
                    newDoorEnvironment.DoorDelayBeforeBreakIn,
                    newDoorEnvironment.DoorDelayBeforeClose);
            }

            if (oldDoorEnvironmentBeforeUpdate.SensorsOpenMaxDoors != null
                && (newDoorEnvironment.SensorsOpenMaxDoors == null
                    || !newDoorEnvironment.SensorsOpenMaxDoors.IdInput.Equals(
                        oldDoorEnvironmentBeforeUpdate.SensorsOpenMaxDoors.IdInput)))
            {
                SetDelayToOnOffForInput(
                    oldDoorEnvironmentBeforeUpdate.SensorsOpenMaxDoors,
                    0,
                    0);
            }

            if (newDoorEnvironment.DoorAjarPresentationGroup != null &&
                !newDoorEnvironment.DoorAjarPresentationGroup.Compare(
                    oldDoorEnvironmentBeforeUpdate.DoorAjarPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_DoorAjar,
                    ObjectType.DoorEnvironment, newDoorEnvironment.IdDoorEnvironment);
            }

            if (newDoorEnvironment.IntrusionPresentationGroup != null &&
                !newDoorEnvironment.IntrusionPresentationGroup.Compare(
                    oldDoorEnvironmentBeforeUpdate.IntrusionPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_Intrusion,
                    ObjectType.DoorEnvironment, newDoorEnvironment.IdDoorEnvironment);
            }

            if (newDoorEnvironment.SabotagePresentationGroup != null &&
                !newDoorEnvironment.SabotagePresentationGroup.Compare(
                    oldDoorEnvironmentBeforeUpdate.SabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_Sabotage,
                    ObjectType.DoorEnvironment, newDoorEnvironment.IdDoorEnvironment);
            }
        }

        private void ResetOutput(Output output)
        {
            if (!NeedToResetOutput(output))
                return;

            var outputForUpdate = Outputs.Singleton.GetObjectForEdit(output.IdOutput);

            outputForUpdate.SettingsDelayToOn = 0;
            outputForUpdate.SettingsDelayToOff = 0;
            outputForUpdate.OutputType = (byte)OutputType.Level;
            outputForUpdate.ControlType = (byte)OutputControl.unblocked;
            outputForUpdate.SettingsPulseLength = 0;
            outputForUpdate.SettingsPulseDelay = 0;
            outputForUpdate.SettingsForcedToOff = false;

            Outputs.Singleton.Update(outputForUpdate);
            Outputs.Singleton.EditEnd(outputForUpdate);
        }

        private void SetDelayToOnOffForInput(Input input, int delayToOn, int delayToOff)
        {
            var inputForUpdate = Inputs.Singleton.GetObjectForEdit(input.IdInput);

            inputForUpdate.DelayToOn = delayToOn;
            inputForUpdate.DelayToOff = delayToOff;

            Inputs.Singleton.Update(inputForUpdate);
            Inputs.Singleton.EditEnd(inputForUpdate);
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(DoorEnvironment doorEnvironment)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    doorEnvironment.IdDoorEnvironment,
                    ObjectType.DoorEnvironment),
                1);
        }

        public override void AfterDelete(DoorEnvironment doorEnvironment)
        {
            base.AfterDelete(doorEnvironment);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(doorEnvironment));
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(DoorEnvironment.COLUMNCCU, false));
            c.AddOrder(new Order(DoorEnvironment.COLUMNNUMBER, true));
        }

        protected override void LoadObjectsInRelationship(DoorEnvironment obj)
        {
            if (obj.CCU != null)
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);

            if (obj.DCU != null)
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);

            if (obj.SensorsLockDoors != null)
                obj.SensorsLockDoors = Inputs.Singleton.GetById(obj.SensorsLockDoors.IdInput);

            if (obj.SensorsOpenDoors != null)
                obj.SensorsOpenDoors = Inputs.Singleton.GetById(obj.SensorsOpenDoors.IdInput);

            if (obj.SensorsOpenMaxDoors != null)
                obj.SensorsOpenMaxDoors = Inputs.Singleton.GetById(obj.SensorsOpenMaxDoors.IdInput);

            if (obj.ActuatorsElectricStrike != null)
                obj.ActuatorsElectricStrike = Outputs.Singleton.GetById(obj.ActuatorsElectricStrike.IdOutput);

            if (obj.ActuatorsElectricStrikeOpposite != null)
                obj.ActuatorsElectricStrikeOpposite = Outputs.Singleton.GetById(obj.ActuatorsElectricStrikeOpposite.IdOutput);

            if (obj.ActuatorsExtraElectricStrike != null)
                obj.ActuatorsExtraElectricStrike = Outputs.Singleton.GetById(obj.ActuatorsExtraElectricStrike.IdOutput);

            if (obj.ActuatorsExtraElectricStrikeOpposite != null)
                obj.ActuatorsExtraElectricStrikeOpposite = Outputs.Singleton.GetById(obj.ActuatorsExtraElectricStrikeOpposite.IdOutput);

            if (obj.ActuatorsBypassAlarm != null)
                obj.ActuatorsBypassAlarm = Outputs.Singleton.GetById(obj.ActuatorsBypassAlarm.IdOutput);

            if (obj.DoorAjarPresentationGroup != null)
                obj.DoorAjarPresentationGroup = PresentationGroups.Singleton.GetById(obj.DoorAjarPresentationGroup.IdGroup);

            if (obj.IntrusionPresentationGroup != null)
                obj.IntrusionPresentationGroup = PresentationGroups.Singleton.GetById(obj.IntrusionPresentationGroup.IdGroup);

            if (obj.SabotagePresentationGroup != null)
                obj.SabotagePresentationGroup = PresentationGroups.Singleton.GetById(obj.SabotagePresentationGroup.IdGroup);

            if (obj.IntrusionOutput != null)
                obj.IntrusionOutput = Outputs.Singleton.GetById(obj.IntrusionOutput.IdOutput);

            if (obj.DoorAjarOutput != null)
                obj.DoorAjarOutput = Outputs.Singleton.GetById(obj.DoorAjarOutput.IdOutput);

            if (obj.SabotageOutput != null)
                obj.SabotageOutput = Outputs.Singleton.GetById(obj.SabotageOutput.IdOutput);

            if (obj.CardReaderExternal != null)
                obj.CardReaderExternal = CardReaders.Singleton.GetById(obj.CardReaderExternal.IdCardReader);

            if (obj.PushButtonInternal != null)
                obj.PushButtonInternal = Inputs.Singleton.GetById(obj.PushButtonInternal.IdInput);

            if (obj.CardReaderInternal != null)
                obj.CardReaderInternal = CardReaders.Singleton.GetById(obj.CardReaderInternal.IdCardReader);

            if (obj.PushButtonExternal != null)
                obj.PushButtonExternal = Inputs.Singleton.GetById(obj.PushButtonExternal.IdInput);

            if (obj.DoorEnvironmentAlarmArcs != null)
            {
                var doorEnvironmentAlarmArcs = new LinkedList<DoorEnvironmentAlarmArc>();

                foreach (var doorEnvironmentAlarmArc in obj.DoorEnvironmentAlarmArcs)
                {
                    doorEnvironmentAlarmArcs.AddLast(
                        DoorEnvironmentAlarmArcs.Singleton.GetById(
                            doorEnvironmentAlarmArc.IdDoorEnvironmentAlarmArc));
                }

                obj.DoorEnvironmentAlarmArcs.Clear();

                foreach (var doorEnvironmentAlarmArc in doorEnvironmentAlarmArcs)
                {
                    obj.DoorEnvironmentAlarmArcs.Add(doorEnvironmentAlarmArc);
                }
            }
        }

        protected override void LoadObjectsInRelationshipGetById(DoorEnvironment obj)
        {
            if (obj == null)
                return;

            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
            }

            if (obj.DCU != null)
            {
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);
            }
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            if (referencedObjects != null && referencedObjects.Count > 0)
            {
                foreach (var ormObject in referencedObjects)
                {
                    if (!(ormObject is CCU) && !(ormObject is DCU))
                        return false;
                }
            }

            return true;
        }

        public DoorEnvironment CreateNew(string name, int number)
        {
            return CreateNew(name, number, false);
        }

        public DoorEnvironment CreateNew(string name, int number, bool isDcu)
        {
            var newDoorEnvironment = new DoorEnvironment();
            if (isDcu)
                newDoorEnvironment.Name = name;
            else
                newDoorEnvironment.Name = name + " " + (number + 1);

            newDoorEnvironment.BlockedByLicence = !AllowToConfigureDoorEnvironment();
            newDoorEnvironment.Number = (byte)number;
            newDoorEnvironment.DoorTimeUnlock = 5;
            newDoorEnvironment.DoorTimeOpen = 15;
            newDoorEnvironment.DoorTimePreAlarm = 5;
            newDoorEnvironment.DoorDelayBeforeUnlock = 0;
            newDoorEnvironment.DoorDelayBeforeLock = 0;
            newDoorEnvironment.DoorDelayBeforeClose = 1000;
            newDoorEnvironment.IntrusionAlarmOn = true;
            newDoorEnvironment.SabotageAlarmOn = true;
            return newDoorEnvironment;
        }

        public bool AllowToConfigureDoorEnvironment()
        {
            var allow = true;
#if !DEBUG
            if (!Cgp.Server.CgpServer.Singleton.DemoLicense)
            {
                string localisedName = string.Empty;
                object value = null;
                int allowedCount = 0;
                if (!NCASServer.Singleton.GetLicencePropertyInfo(Cgp.NCAS.Globals.RequiredLicenceProperties.DoorEnvironmentCount.ToString(), out localisedName, out value))
                {
                    Eventlogs.Singleton.InsertEvent("Get licence property failed", this.GetType().Assembly.GetName().Name, null,
                        "Failed to get licence property: " + NCAS.Globals.RequiredLicenceProperties.DoorEnvironmentCount.ToString());
                    return false;
                }
                else
                {
                    if (!int.TryParse(value.ToString(), out allowedCount))
                    {
                        Eventlogs.Singleton.InsertEvent("Get licence property failed", this.GetType().Assembly.GetName().Name, null,
                        "Failed to get licence property: " + NCAS.Globals.RequiredLicenceProperties.DoorEnvironmentCount.ToString());
                        return false;
                    }
                    else
                        allow = GetConfiguredCount() < allowedCount;
                }
            }
#endif
            return allow;
        }

        public DoorEnvironment CreateNewForDCU()
        {
            return CreateNew(CgpServer.Singleton.LocalizationHelper.GetString("DoorEnvironmentForDCUName"), 0, true);
        }

        public bool IsInputInDoorEnvironments(Guid inputGuid)
        {
            var input = Inputs.Singleton.GetById(inputGuid);
            if (input == null)
                return false;

            return IsInputInDoorEnvironments(input);
        }

        public bool IsInputInDoorEnvironments(Input input)
        {
            if (input == null)
                return false;

            var doorEnvironments =
                SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.SensorsLockDoors == input ||
                                                               doorEnvironment.SensorsOpenDoors == input ||
                                                               doorEnvironment.SensorsOpenMaxDoors == input ||
                                                               doorEnvironment.PushButtonInternal == input ||
                                                               doorEnvironment.PushButtonExternal == input);

            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return MultiDoorElements.Singleton.IsInputUsedOnlyInDoorElement(input.IdInput);
        }

        public AOrmObject GetInputDoorEnvironment(Guid inputGuid)
        {
            var input = Inputs.Singleton.GetById(inputGuid);
            if (input == null)
                return null;

            var doorEnvironments =
                SelectLinq<DoorEnvironment>(
                    doorEnvironment =>
                        (doorEnvironment.SensorsLockDoors != null &&
                         doorEnvironment.SensorsLockDoors.IdInput == input.IdInput) ||
                        (doorEnvironment.SensorsOpenDoors != null &&
                         doorEnvironment.SensorsOpenDoors.IdInput == input.IdInput) ||
                        (doorEnvironment.SensorsOpenMaxDoors != null &&
                         doorEnvironment.SensorsOpenMaxDoors.IdInput == input.IdInput) ||
                        (doorEnvironment.PushButtonInternal != null &&
                         doorEnvironment.PushButtonInternal.IdInput == input.IdInput) ||
                        (doorEnvironment.PushButtonExternal != null &&
                         doorEnvironment.PushButtonExternal.IdInput == input.IdInput));

            if (doorEnvironments != null && doorEnvironments.Count > 0)
            {
                var de = doorEnvironments.ElementAt(0);
                return GetById(de.IdDoorEnvironment);
            }

            var multiDoorElements =
                SelectLinq<MultiDoorElement>(
                    multiDoorElement =>
                        multiDoorElement.SensorOpenDoor != null &&
                        multiDoorElement.SensorOpenDoor.IdInput == input.IdInput);

            if (multiDoorElements != null && multiDoorElements.Count > 0)
            {
                var multiDoorElement = multiDoorElements.ElementAt(0);
                return MultiDoorElements.Singleton.GetById(multiDoorElement.IdMultiDoorElement);
            }

            return null;
        }

        public bool IsOutputInDoorEnvironmentsActuators(Output output)
        {
            if (output == null)
                return false;
            var doorEnvironments = SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.ActuatorsElectricStrike == output
                || doorEnvironment.ActuatorsElectricStrikeOpposite == output || doorEnvironment.ActuatorsExtraElectricStrike == output ||
                doorEnvironment.ActuatorsExtraElectricStrikeOpposite == output || doorEnvironment.ActuatorsBypassAlarm == output);
            
            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(output.IdOutput);
        }


        public AOrmObject GetOutputDoorEnvironment(Guid outputGuid)
        {
            var output = Outputs.Singleton.GetById(outputGuid);
            if (output == null)
                return null;

            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    (doorEnvironment.ActuatorsElectricStrike != null &&
                     doorEnvironment.ActuatorsElectricStrike.IdOutput == output.IdOutput) ||
                    (doorEnvironment.ActuatorsElectricStrikeOpposite != null &&
                     doorEnvironment.ActuatorsElectricStrikeOpposite.IdOutput == output.IdOutput) ||
                    (doorEnvironment.ActuatorsExtraElectricStrike != null &&
                     doorEnvironment.ActuatorsExtraElectricStrike.IdOutput == output.IdOutput) ||
                    (doorEnvironment.ActuatorsExtraElectricStrikeOpposite != null &&
                     doorEnvironment.ActuatorsExtraElectricStrikeOpposite.IdOutput == output.IdOutput) ||
                    (doorEnvironment.ActuatorsBypassAlarm != null &&
                     doorEnvironment.ActuatorsBypassAlarm.IdOutput == output.IdOutput));

            if (doorEnvironments != null && doorEnvironments.Count > 0)
            {
                var de = doorEnvironments.ElementAt(0);
                return GetById(de.IdDoorEnvironment);
            }

            var multiDoorElements =
                SelectLinq<MultiDoorElement>(
                    multiDoorElement =>
                        (multiDoorElement.ElectricStrike != null &&
                         multiDoorElement.ElectricStrike.IdOutput == output.IdOutput) ||
                        (multiDoorElement.ExtraElectricStrike != null &&
                         multiDoorElement.ExtraElectricStrike.IdOutput == output.IdOutput));

            if (multiDoorElements != null && multiDoorElements.Count > 0)
            {
                var multiDoorElement = multiDoorElements.ElementAt(0);
                return MultiDoorElements.Singleton.GetById(multiDoorElement.IdMultiDoorElement);
            }

            return null;
        }

        public bool IsOutputInDoorEnvironments(Guid outputGuid)
        {
            var output = Outputs.Singleton.GetById(outputGuid);
            if (output == null)
                return false;

            return IsOutputInDoorEnvironments(output);
        }

        public bool IsOutputInDoorEnvironments(Output output)
        {
            if (output == null)
                return false;

            var doorEnvironments = SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.ActuatorsElectricStrike == output
                || doorEnvironment.ActuatorsElectricStrikeOpposite == output || doorEnvironment.ActuatorsExtraElectricStrike == output
                || doorEnvironment.ActuatorsBypassAlarm == output || doorEnvironment.IntrusionOutput == output || doorEnvironment.DoorAjarOutput == output
                || doorEnvironment.SabotageOutput == output || doorEnvironment.ActuatorsExtraElectricStrikeOpposite == output);
            
            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(output.IdOutput);
        }

        public ICollection<Input> GetInputsNotInDoorEnvironments(Guid doorEnvironmentGuid)
        {
            Exception error;
            var inputs = Inputs.Singleton.List(out error) as IEnumerable<Input>;

            if (inputs == null)
                return null;

            var doorEnvironment = doorEnvironmentGuid != Guid.Empty ? Singleton.GetById(doorEnvironmentGuid) : null;

            if (doorEnvironment != null)
            {
                inputs =
                    inputs.Where(input =>
                    {
                        if (input.BlockingType == (byte)BlockingType.BlockedByObject)
                            return false;
                        
                        if (doorEnvironment.CCU != null && input.CCU != null)
                            return input.CCU.IdCCU == doorEnvironment.CCU.IdCCU;

                        if (doorEnvironment.DCU != null && input.DCU != null)
                            return input.DCU.IdDCU == doorEnvironment.DCU.IdDCU;

                        return false;
                    });
            }

            var doorEnvironments = doorEnvironment != null
                ? SelectLinq<DoorEnvironment>(
                    doorEnvironemntFromDb =>
                        doorEnvironemntFromDb.IdDoorEnvironment != doorEnvironment.IdDoorEnvironment &&
                        (doorEnvironemntFromDb.SensorsLockDoors != null ||
                         doorEnvironemntFromDb.SensorsOpenDoors != null ||
                         doorEnvironemntFromDb.SensorsOpenMaxDoors != null ||
                         doorEnvironemntFromDb.PushButtonInternal != null ||
                         doorEnvironemntFromDb.PushButtonExternal != null))
                : SelectLinq<DoorEnvironment>(
                    doorEnvironemntFromDb =>
                        doorEnvironemntFromDb.SensorsLockDoors != null ||
                        doorEnvironemntFromDb.SensorsOpenDoors != null ||
                        doorEnvironemntFromDb.SensorsOpenMaxDoors != null ||
                        doorEnvironemntFromDb.PushButtonInternal != null ||
                        doorEnvironemntFromDb.PushButtonExternal != null);

            var alarmAreas = SelectLinq<AlarmArea>(alarmArea => alarmArea.ActivationStateInputEIS != null);

            var multiDoorElements =
                SelectLinq<MultiDoorElement>(multiDoorElement => multiDoorElement.SensorOpenDoor != null);

            var usedInputs = Enumerable.Empty<Guid>();

            if (doorEnvironments != null)
                usedInputs =
                    usedInputs
                        .Concat(doorEnvironments
                            .Where(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsLockDoors != null)
                            .Select(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsLockDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsOpenDoors != null)
                            .Select(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsOpenDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsOpenMaxDoors != null)
                            .Select(doorEnvironemntFromDb => doorEnvironemntFromDb.SensorsOpenMaxDoors.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironemntFromDb => doorEnvironemntFromDb.PushButtonInternal != null)
                            .Select(doorEnvironemntFromDb => doorEnvironemntFromDb.PushButtonInternal.IdInput))
                        .Concat(doorEnvironments
                            .Where(doorEnvironemntFromDb => doorEnvironemntFromDb.PushButtonExternal != null)
                            .Select(doorEnvironemntFromDb => doorEnvironemntFromDb.PushButtonExternal.IdInput));

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

            var usedInputsHashSet = new HashSet<Guid>(usedInputs);

            return
                new LinkedList<Input>(
                    inputs.Where(input => !usedInputsHashSet.Contains(input.IdInput))
                        .OrderBy(input => input.ToString()));
        }

        public IList<Output> GetAllCCUOutputsNotInDoorEnvironmentsActuators(Guid doorEnvironmentGuid)
        {
            var outputs = new List<Output>();
            var doorEnvironment = Singleton.GetById(doorEnvironmentGuid);
            if (doorEnvironment != null)
            {
                CCU ccu = null;
                if (doorEnvironment.CCU != null)
                    ccu = doorEnvironment.CCU;
                else if (doorEnvironment.DCU != null)
                    ccu = doorEnvironment.DCU.CCU;

                if (ccu != null)
                {
                    var availableOutputs = Outputs.Singleton.GetOutputsFromCCUAndItsDCUs(ccu);
                    availableOutputs = Outputs.Singleton.FilterOutputsFromActivators(availableOutputs, doorEnvironment, Guid.Empty);
                    if (availableOutputs != null && availableOutputs.Count > 0)
                        outputs.AddRange(availableOutputs);
                }
            }
            else
            {
                IList<Output> availableOutputs = 
                    Outputs.Singleton.FilterOutputsFromActivators(
                        true, 
                        Guid.Empty, 
                        false, 
                        Guid.Empty);

                if (availableOutputs != null && availableOutputs.Count > 0)
                    outputs.AddRange(availableOutputs);
            }

            return outputs;
        }

        public ICollection<CardReader> GetCardReadersNotInDoorEnvironments(Guid doorEnvironmentGuid)
        {
            Exception error;
            var cardReaders = CardReaders.Singleton.List(out error) as IEnumerable<CardReader>;

            if (cardReaders == null)
                return null;

            var doorEnvironment = doorEnvironmentGuid != Guid.Empty ? Singleton.GetById(doorEnvironmentGuid) : null;

            if (doorEnvironment != null)
            {
                cardReaders =
                    cardReaders.Where(
                        cardReader =>
                        {
                            if (doorEnvironment.CCU != null && cardReader.CCU != null)
                                return doorEnvironment.CCU.IdCCU == cardReader.CCU.IdCCU;

                            if (doorEnvironment.DCU != null && cardReader.DCU != null)
                                return doorEnvironment.DCU.IdDCU == cardReader.DCU.IdDCU;

                            return false;
                        });
            }

            var doorEnvironments = doorEnvironment != null
                ? SelectLinq<DoorEnvironment>(
                    doorEnvironmentFromDb =>
                        doorEnvironmentFromDb.IdDoorEnvironment != doorEnvironment.IdDoorEnvironment &&
                        (doorEnvironmentFromDb.CardReaderInternal != null ||
                         doorEnvironmentFromDb.CardReaderExternal != null))
                : SelectLinq<DoorEnvironment>(
                    doorEnvironmentFromDb =>
                        doorEnvironmentFromDb.CardReaderInternal != null ||
                        doorEnvironmentFromDb.CardReaderExternal != null);

            var multiDoors = MultiDoors.Singleton.List();

            var usedCardReaders = Enumerable.Empty<Guid>();

            if (doorEnvironments != null)
                usedCardReaders =
                    usedCardReaders
                        .Concat(doorEnvironments
                            .Where(doorEnvironmentFromDb => doorEnvironmentFromDb.CardReaderInternal != null)
                            .Select(doorEnvironmentFromDb => doorEnvironmentFromDb.CardReaderInternal.IdCardReader))
                        .Concat(doorEnvironments
                            .Where(doorEnvironmentFromDb => doorEnvironmentFromDb.CardReaderExternal != null)
                            .Select(doorEnvironmentFromDb => doorEnvironmentFromDb.CardReaderExternal.IdCardReader));

            if (multiDoors != null)
                usedCardReaders =
                    usedCardReaders.Concat(
                        multiDoors.Select(
                            multiDoor => multiDoor.CardReader.IdCardReader));

            var usedCardReadersHashSet = new HashSet<Guid>(usedCardReaders);

            return
                new LinkedList<CardReader>(
                    cardReaders.Where(cardReader => !usedCardReadersHashSet.Contains(cardReader.IdCardReader))
                        .OrderBy(cardReader => cardReader.ToString()));
        }

        public DoorEnvironment GetDoorEnvironmentForCardReader(Guid idCardReader)
        {
            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    (doorEnvironment.CardReaderExternal != null
                     && doorEnvironment.CardReaderExternal.IdCardReader.Equals(idCardReader))
                    || (doorEnvironment.CardReaderInternal != null
                        && doorEnvironment.CardReaderInternal.IdCardReader.Equals(idCardReader)));


            return doorEnvironments != null
                ? doorEnvironments.FirstOrDefault()
                : null;
        }


        public IList<Output> GetNotUsedOutputs(Guid doorEnvironmentGuid)
        {
            var outputsFromAlarmAreas = AlarmAreas.Singleton.GetOutputsUsedInAlarmAreas();

            var outputs = new List<Output>();
            var doorEnvironment = Singleton.GetById(doorEnvironmentGuid);
            if (doorEnvironment != null)
            {
                if (doorEnvironment.CCU != null && doorEnvironment.CCU.Outputs != null && doorEnvironment.CCU.Outputs.Count > 0)
                {
                    var availableOutputs = Outputs.Singleton.FilterOutputsFromAllUsed(doorEnvironment.CCU.Outputs.ToList(), doorEnvironment);
                    if (availableOutputs != null && availableOutputs.Count > 0)
                        outputs.AddRange(availableOutputs);
                }
                else if (doorEnvironment.DCU != null && doorEnvironment.DCU.Outputs != null && doorEnvironment.DCU.Outputs.Count > 0)
                {
                    var availableOutputs = Outputs.Singleton.FilterOutputsFromAllUsed(doorEnvironment.DCU.Outputs.ToList(), doorEnvironment);
                    if (availableOutputs != null && availableOutputs.Count > 0)
                        outputs.AddRange(availableOutputs);
                }
            }
            else
            {
                var availableOutputs = Outputs.Singleton.List();
                if (availableOutputs != null && availableOutputs.Count > 0)
                {
                    availableOutputs = Outputs.Singleton.FilterOutputsFromAllUsed(availableOutputs.ToList(), null);
                    if (availableOutputs != null && availableOutputs.Count > 0)
                        outputs = availableOutputs.ToList();
                }
            }

            foreach (var output in outputsFromAlarmAreas)
            {
                if (outputs.Contains(output))
                    outputs.Remove(output);
            }

            return outputs;
        }

        public bool? DoorEnvironmentAccessGranted(DoorEnvironment doorEnvironment)
        {
            return CCUConfigurationHandler.Singleton.DoorEnvironmentAccessGranted(doorEnvironment);
        }

        public DoorEnvironmentState GetDoorEnvironmentState(DoorEnvironment doorEnvironment)
        {
            return
                doorEnvironment != null
                    ? CCUConfigurationHandler.Singleton.GetDoorEnvironmentState(
                        GetParentCcuId(doorEnvironment),
                        doorEnvironment.IdDoorEnvironment)
                    : DoorEnvironmentState.Unknown;
        }

        public DoorEnvironmentState GetDoorEnvironmentState(Guid doorEnvironmentId)
        {
            return GetDoorEnvironmentState(GetById(doorEnvironmentId));
        }

        public bool UnconfigureDoorEnvironments(DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment.Configured)
            {
                doorEnvironment = Singleton.GetObjectForEdit(doorEnvironment.IdDoorEnvironment);
                if (doorEnvironment != null)
                {
                    doorEnvironment.ActuatorsDoorEnvironment = null;
                    doorEnvironment.SensorsLockDoors = null;
                    doorEnvironment.SensorsOpenDoors = null;
                    doorEnvironment.SensorsOpenMaxDoors = null;
                    doorEnvironment.ActuatorsElectricStrike = null;
                    doorEnvironment.ActuatorsExtraElectricStrike = null;
                    doorEnvironment.ActuatorsElectricStrikeOpposite = null;
                    doorEnvironment.ActuatorsExtraElectricStrikeOpposite = null;
                    doorEnvironment.ActuatorsBypassAlarm = null;

                    doorEnvironment.DoorAjarOutput = null;
                    doorEnvironment.IntrusionOutput = null;
                    doorEnvironment.SabotageOutput = null;

                    doorEnvironment.CardReaderExternal = null;
                    doorEnvironment.CardReaderInternal = null;

                    doorEnvironment.PushButtonExternal = null;
                    doorEnvironment.PushButtonInternal = null;

                    return Singleton.Update(doorEnvironment);
                }
            }

            return false;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var doorEnvironment = GetById(idObj);
                if (doorEnvironment == null)
                    return null;

                var result = new List<AOrmObject>();
                if (doorEnvironment.CCU != null)
                {
                    var ccu = CCUs.Singleton.GetById(doorEnvironment.CCU.IdCCU);
                    result.Add(ccu);
                }

                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(doorEnvironment.IdDoorEnvironment, ObjectType.DoorEnvironment);
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.Add(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(doorEnvironment.IdDoorEnvironment, ObjectType.DoorEnvironment);
                if (usedInAz != null)
                {
                    foreach (var person in usedInAz)
                    {
                        var outPerson = Persons.Singleton.GetById(person.IdPerson);
                        result.Add(outPerson);
                    }
                }

                if (result.Count > 0)
                {
                    return result.OrderBy(orm => orm.ToString()).ToList();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public ICollection<DoorEnvironment> GetDoorEnvironmentsForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null) return null;
                return SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.IntrusionPresentationGroup == pg ||
                    doorEnvironment.DoorAjarPresentationGroup == pg || doorEnvironment.SabotagePresentationGroup == pg);
            }
            catch
            {
                return null;
            }
        }

        public bool HasAccessToAccessGranted(Guid doorEnvironmentId)
        {
            var doorEnvironment = GetById(doorEnvironmentId);

            if (doorEnvironment == null)
                return false;

            if (AccessChecker.IsSuperAdminLogged())
                return true;

            var login = AccessChecker.GetActualLogin();

            if (login == null)
                return false;

            if (AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(
                    AccessNCAS.DoorEnvironmentsAccessGrantedPerform),
                login))
            {
                return true;
            }

            if (login.Person == null)
                return false;

            var cardReaderExternal = doorEnvironment.CardReaderExternal;
            var cardReaderInternal = doorEnvironment.CardReaderInternal;

            if (cardReaderExternal == null && cardReaderInternal == null)
                return true;

            var cardReaders = ACLPersons.Singleton.LoadActiveCardReaders(login.Person);

            return cardReaderInternal != null && cardReaders.ContainsKey(cardReaderInternal.IdCardReader) ||
                   cardReaderExternal != null && cardReaders.ContainsKey(cardReaderExternal.IdCardReader);
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<DoorEnvironment> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (single)
                {
                    linqResult = SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.Name.IndexOf(name) >= 0);
                }
                else
                {
                    linqResult = SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.Name.IndexOf(name) >= 0
                        || doorEnvironment.Description.IndexOf(name) >= 0);
                }
            }

            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(doorEnvironment => doorEnvironment.CCU).ThenBy(doorEnvironment => doorEnvironment.Number).ToList();
                IList<DoorEnvironment> listDEs = linqResult.ToList();
                ArrayList.Adapter((IList)listDEs).Sort();
                foreach (var doorEnvironment in listDEs)
                {
                    resultList.Add(GetObjectById(doorEnvironment.IdDoorEnvironment));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<DoorEnvironment> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = 
                    SelectLinq<DoorEnvironment>(
                        doorEnvironment => 
                            doorEnvironment.Name.IndexOf(name) >= 0 ||
                            doorEnvironment.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<DoorEnvironment> linqResult = 
                    string.IsNullOrEmpty(name) 
                        ? List()
                        : SelectLinq<DoorEnvironment>(doorEnvironment => doorEnvironment.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(IEnumerable<DoorEnvironment> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(doorEnvironment => doorEnvironment.CCU).ThenBy(doorEnvironment => doorEnvironment.Number).ToList();
                IList<DoorEnvironment> listDEs = linqResult.ToList();
                ArrayList.Adapter((IList)listDEs).Sort();
                foreach (var doorEnvironment in listDEs)
                {
                    resultList.Add(GetById(doorEnvironment.IdDoorEnvironment));
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public void GetParentCCU(ICollection<Guid> ccus, Guid idDoorEnvironment)
        {
            var doorEnvironment = GetById(idDoorEnvironment);
            if (ccus != null && doorEnvironment != null)
            {
                if (doorEnvironment.CCU != null)
                {
                    CCUs.Singleton.GetParentCCU(ccus, doorEnvironment.CCU.IdCCU);
                }
                else if (doorEnvironment.DCU != null)
                {
                    DCUs.Singleton.GetParentCCU(ccus, doorEnvironment.DCU.IdDCU);
                }
            }
        }

        public Guid GetParentCcuId(DoorEnvironment doorEnvironment)
        {
            if (doorEnvironment == null)
                return Guid.Empty;

            if (doorEnvironment.CCU != null)
            {
                return doorEnvironment.CCU.IdCCU;
            }
            
            if (doorEnvironment.DCU != null && doorEnvironment.DCU.CCU != null)
            {
                return doorEnvironment.DCU.CCU.IdCCU;
            }

            return Guid.Empty;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listDoorEnvironment = List(out error);
            var listDoorEnvironmentModifyObj = new List<IModifyObject>();
            if (listDoorEnvironment != null)
            {
                foreach (var doorEnvironment in listDoorEnvironment)
                {
                    listDoorEnvironmentModifyObj.Add(new DoorEnvironmentModifyObj(doorEnvironment));
                }
                listDoorEnvironmentModifyObj = listDoorEnvironmentModifyObj.OrderBy(doorEnvironment => doorEnvironment.ToString()).ToList();
            }
            return listDoorEnvironmentModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsOnlyFromCCUs(out Exception error)
        {
            var listDoorEnvironment = List(out error);
            var listDoorEnvironmentModifyObj = new List<IModifyObject>();
            if (listDoorEnvironment != null)
            {
                foreach (var doorEnvironment in listDoorEnvironment)
                {
                    if (doorEnvironment != null && doorEnvironment.CCU != null)
                        listDoorEnvironmentModifyObj.Add(new DoorEnvironmentModifyObj(doorEnvironment));
                }
                listDoorEnvironmentModifyObj = listDoorEnvironmentModifyObj.OrderBy(doorEnvironment => doorEnvironment.ToString()).ToList();
            }
            return listDoorEnvironmentModifyObj;
        }

        public int GetConfiguredCount()
        {
            var doorEnvironments = List();
            if (doorEnvironments == null || doorEnvironments.Count == 0)
                return 0;
            var result = 0;
            foreach (var de in doorEnvironments)
            {
                if (de.Configured)
                    result++;
            }
            return result;
        }

        public bool UsedSensorInAnotherDoorEnvironment(Guid idInput, Guid idDoorEnvironment)
        {
            var input = Inputs.Singleton.GetById(idInput);
            if (input == null)
                return false;

            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    doorEnvironment.IdDoorEnvironment != idDoorEnvironment &&
                    ((doorEnvironment.SensorsLockDoors != null &&
                      doorEnvironment.SensorsLockDoors.IdInput == input.IdInput) ||
                     (doorEnvironment.SensorsOpenDoors != null &&
                      doorEnvironment.SensorsOpenDoors.IdInput == input.IdInput) ||
                     (doorEnvironment.SensorsOpenMaxDoors != null &&
                      doorEnvironment.SensorsOpenMaxDoors.IdInput == input.IdInput)));
            
            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return AlarmAreas.Singleton.IsInputInAlarmAreas(input, Guid.Empty) ||
                   MultiDoorElements.Singleton.IsInputUsedOnlyInDoorElement(idInput);
        }

        public bool UsedActuatorInAnotherDoorEnvironment(Guid idOutput, Guid idDoorEnvironment)
        {
            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    doorEnvironment.IdDoorEnvironment != idDoorEnvironment &&
                    ((doorEnvironment.ActuatorsElectricStrike != null &&
                      doorEnvironment.ActuatorsElectricStrike.IdOutput == idOutput) ||
                     (doorEnvironment.ActuatorsElectricStrikeOpposite != null &&
                      doorEnvironment.ActuatorsElectricStrikeOpposite.IdOutput == idOutput) ||
                     (doorEnvironment.ActuatorsExtraElectricStrike != null &&
                      doorEnvironment.ActuatorsExtraElectricStrike.IdOutput == idOutput) ||
                     (doorEnvironment.ActuatorsBypassAlarm != null &&
                      doorEnvironment.ActuatorsBypassAlarm.IdOutput == idOutput) ||
                     (doorEnvironment.IntrusionOutput != null &&
                      doorEnvironment.IntrusionOutput.IdOutput == idOutput) ||
                     (doorEnvironment.DoorAjarOutput != null &&
                      doorEnvironment.DoorAjarOutput.IdOutput == idOutput) ||
                     (doorEnvironment.SabotageOutput != null &&
                      doorEnvironment.SabotageOutput.IdOutput == idOutput) ||
                     (doorEnvironment.ActuatorsExtraElectricStrikeOpposite != null &&
                      doorEnvironment.ActuatorsExtraElectricStrikeOpposite.IdOutput == idOutput)));

            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return AlarmAreas.Singleton.IsOutputInAlarmAreas(idOutput) ||
                   MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(idOutput);
        }

        public bool UsedCardReaderInAnotherDoorEnvironment(Guid idCardReader, Guid idDoorEnvironment)
        {
            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    doorEnvironment.IdDoorEnvironment != idDoorEnvironment &&
                    ((doorEnvironment.CardReaderExternal != null &&
                      doorEnvironment.CardReaderExternal.IdCardReader == idCardReader) ||
                     (doorEnvironment.CardReaderInternal != null &&
                      doorEnvironment.CardReaderInternal.IdCardReader == idCardReader)));
            
            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return MultiDoors.Singleton.IsCardReaderUsedInMultiDoor(idCardReader);
        }

        public bool UsedPushButtonInAnotherDoorEnvironment(Guid idInput, Guid idDoorEnvironment)
        {
            var doorEnvironments = SelectLinq<DoorEnvironment>(
                doorEnvironment =>
                    doorEnvironment.IdDoorEnvironment != idDoorEnvironment &&
                    ((doorEnvironment.PushButtonInternal != null &&
                      doorEnvironment.PushButtonInternal.IdInput == idInput) ||
                     (doorEnvironment.PushButtonExternal != null &&
                      doorEnvironment.PushButtonExternal.IdInput == idInput)));

            return doorEnvironments != null && doorEnvironments.Count > 0;
        }

        public bool UsedSensorsInAnotherDoorEnvironment(Guid idOutput, Guid idDoorEnvironment)
        {
            var output = Outputs.Singleton.GetById(idOutput);
            if (output == null)
                return false;

            var doorEnvironments =
                SelectLinq<DoorEnvironment>(doorEnvironment =>
                    doorEnvironment.IdDoorEnvironment != idDoorEnvironment &&
                    ((doorEnvironment.ActuatorsElectricStrike != null &&
                      doorEnvironment.ActuatorsElectricStrike.IdOutput == output.IdOutput) ||
                     (doorEnvironment.ActuatorsElectricStrikeOpposite != null &&
                      doorEnvironment.ActuatorsElectricStrikeOpposite.IdOutput == output.IdOutput) ||
                     (doorEnvironment.ActuatorsExtraElectricStrike != null &&
                      doorEnvironment.ActuatorsExtraElectricStrike.IdOutput == output.IdOutput) ||
                     (doorEnvironment.ActuatorsBypassAlarm != null &&
                      doorEnvironment.ActuatorsBypassAlarm.IdOutput == output.IdOutput) ||
                     (doorEnvironment.ActuatorsExtraElectricStrikeOpposite != null &&
                      doorEnvironment.ActuatorsExtraElectricStrikeOpposite.IdOutput == output.IdOutput)));
            
            if (doorEnvironments != null && doorEnvironments.Count > 0)
                return true;

            return AlarmAreas.Singleton.IsOutputInAlarmAreasAsEISSetUnsetOutput(output, Guid.Empty) ||
                   MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(idOutput);
        }

        public SecurityLevel? GetSecondCardReaderSecurityLevelUsedInDoorEnvironmentsWithCardReader(Guid cardReaderGuid)
        {
            try
            {

                var cardReader = CardReaders.Singleton.GetById(cardReaderGuid);
                if (cardReader == null) return null;

                Exception error;
                IList<FilterSettings> filterCR = new List<FilterSettings>();
                var filterSetting = new FilterSettings(DoorEnvironment.COLUMNCARDREADEREXTERNAL, cardReader, ComparerModes.EQUALL);
                filterCR.Add(filterSetting);

                var resultList = 
                    Singleton.SelectByCriteria(filterCR, out error);

                if (resultList != null && resultList.Count > 0)
                {
                    var de = resultList.First();
                    return (SecurityLevel)de.CardReaderInternal.SecurityLevel;
                }

                filterCR.Clear();
                filterSetting = new FilterSettings(DoorEnvironment.COLUMNCARDREADERINTERNAL, cardReader, ComparerModes.EQUALL);
                filterCR.Add(filterSetting);

                resultList = Singleton.SelectByCriteria(filterCR, out error) as List<DoorEnvironment>;
                if (resultList != null && resultList.Count > 0)
                {
                    var de = resultList.First();
                    return (SecurityLevel)de.CardReaderExternal.SecurityLevel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public ICollection<DoorEnvironmentShort> GetShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var deList = SelectByCriteria(filterSettings, out error);
            ICollection<DoorEnvironmentShort> resultList = new List<DoorEnvironmentShort>();

            if (deList != null)
            {
                foreach (var de in deList)
                {
                    var deShort = new DoorEnvironmentShort(de)
                    {
                        State = Singleton.GetDoorEnvironmentState(de)
                    };

                    deShort.StringState = deShort.State.ToString();
                    deShort.Configured = de.Configured;
                    resultList.Add(deShort);
                }
            }

            return resultList.OrderBy(shortDe => shortDe.FullName).ToList();
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidDoorEnvironment)
        {
            var objects = new List<AOrmObject>();

            var doorEnvironment = GetById(guidDoorEnvironment);
            if (doorEnvironment != null)
            {
                if (doorEnvironment.ObjBlockAlarmDoorAjarObjectType != null && doorEnvironment.ObjBlockAlarmDoorAjarId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(doorEnvironment.ObjBlockAlarmDoorAjarId.Value, doorEnvironment.ObjBlockAlarmDoorAjarObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (doorEnvironment.ObjBlockAlarmIntrusionObjectType != null && doorEnvironment.ObjBlockAlarmIntrusionId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(doorEnvironment.ObjBlockAlarmIntrusionId.Value, doorEnvironment.ObjBlockAlarmIntrusionObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (doorEnvironment.ObjBlockAlarmSabotageObjectType != null && doorEnvironment.ObjBlockAlarmSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(doorEnvironment.ObjBlockAlarmSabotageId.Value, doorEnvironment.ObjBlockAlarmSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (doorEnvironment.DoorEnvironmentAlarmArcs != null)
                {
                    objects.AddRange(doorEnvironment.DoorEnvironmentAlarmArcs.Select(
                        doorEnvironmentAlarmArc =>
                            doorEnvironmentAlarmArc.AlarmArc)
                        .Cast<AOrmObject>());
                }
            }

            return objects;
        }

        public override bool CanCreateObject()
        {
            return false;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.DoorEnvironment; }
        }

        public bool HasCardReaderDoorEnvironment(Guid idCardReader)
        {
            return GetDoorEnvironmentForCardReader(idCardReader) != null;
        }
    }
}



