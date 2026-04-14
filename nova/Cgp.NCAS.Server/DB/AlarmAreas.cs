using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.Data;
using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Globals;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AlarmAreas :
        ANcasBaseOrmTableWithAlarmInstruction<AlarmAreas, AlarmArea>,
        IAlarmAreas
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<AlarmArea>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(AlarmArea obj)
            {
                var subObjects = new List<AOrmObjectWithVersion>();

                if (obj.AACardReaders != null)
                    foreach (var aaCardReader in obj.AACardReaders)
                    {
                        subObjects.Add(aaCardReader);
                    }

                if (obj.AAInputs != null)
                    foreach (var aaInput in obj.AAInputs)
                    {
                        subObjects.Add(aaInput);
                    }

                return subObjects;
            }
        }

        private AlarmAreas()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        private static IEnumerable<Output> GetDirectOutputs(AlarmArea alarmArea)
        {
            yield return alarmArea.OutputActivation;
            yield return alarmArea.OutputAlarmState;
            yield return alarmArea.OutputPrewarning;
            yield return alarmArea.OutputTmpUnsetEntry;
            yield return alarmArea.OutputTmpUnsetExit;
            yield return alarmArea.OutputAAlarm;
            yield return alarmArea.OutputSiren;
            yield return alarmArea.OutputSabotage;
            yield return alarmArea.OutputNotAcknowledged;
            yield return alarmArea.OutputMotion;
            yield return alarmArea.OutputSetByObjectForAaFailed;
        }

        protected override IModifyObject CreateModifyObject(AlarmArea ormbObject)
        {
            return new AlarmAreaModifyObj(ormbObject);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(
            AlarmArea alarmArea)
        {
            yield return GetImplicitCCUForAlarmArea(alarmArea.IdAlarmArea);

            var aaCardReaders = alarmArea.AACardReaders;

            if (aaCardReaders != null)
                foreach (var aaCardReader in aaCardReaders)
                    yield return aaCardReader.CardReader;

            var aaInputs = alarmArea.AAInputs;

            if (aaInputs != null)
                foreach (var aaInput in aaInputs)
                    yield return aaInput.Input;

            yield return alarmArea.ObjForAutomaticAct;

            yield return alarmArea.ObjForForcedTimeBuying;

            foreach (var output in GetDirectOutputs(alarmArea))
                yield return output;

            if (alarmArea.SetUnsetOutputEIS != null)
                yield return Outputs.Singleton.GetById(alarmArea.SetUnsetOutputEIS);

            if (alarmArea.PresentationGroup != null)
                yield return alarmArea.PresentationGroup;

            if (alarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null)
                yield return alarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup;

            if (alarmArea.AlarmAreaAlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>();

                foreach (var alarmAreaAlarmArc in alarmArea.AlarmAreaAlarmArcs)
                {
                    var idAlarmArc = alarmAreaAlarmArc.IdAlarmArc;

                    if (!addedAlarmArcIds.Add(idAlarmArc))
                        continue;

                    yield return AlarmArcs.Singleton.GetById(idAlarmArc);
                }
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            var dateOrder = new Order("Name", true);
            c.AddOrder(dateOrder);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.ALARM_AREAS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.AlarmAreasInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.ALARM_AREAS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.AlarmAreasInsertDeletePerform), login);
        }

        public override void CUDSpecial(AlarmArea alarmArea, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        alarmArea.GetId(),
                        alarmArea.GetObjectType()));
            }
            else if (alarmArea != null)
            {
                CCUConfigurationHandler.Singleton.LoadImplicitCCUForAlarmArea(alarmArea);
                AACardReaders.Singleton.SetImplicitAAToCardReader(alarmArea.IdAlarmArea);
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(alarmArea);
            }
        }

        public override void BeforeUpdate(AlarmArea oldObj)
        {
            if (oldObj == null)
                return;

            var aaInputs = new List<AAInput>();

            if (oldObj.AAInputs != null)
            {
                aaInputs.AddRange(oldObj.AAInputs);
            }

            oldObj.AAInputs = aaInputs;
        }

        public override void AfterUpdate(AlarmArea newAlarmArea, AlarmArea oldAlarmAreaBeforUpdate)
        {
            if (newAlarmArea == null || oldAlarmAreaBeforUpdate == null)
            {
                return;
            }

            base.AfterUpdate(
                newAlarmArea,
                oldAlarmAreaBeforUpdate);

            if (newAlarmArea.PresentationGroup != null &&
                !newAlarmArea.PresentationGroup.Compare(oldAlarmAreaBeforUpdate.PresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(
                    AlarmType.AlarmArea_Alarm,
                    ObjectType.AlarmArea,
                    newAlarmArea.IdAlarmArea);

                NCASServer.Singleton.PresentationGroupChanged(
                    AlarmType.AlarmArea_AAlarm,
                    ObjectType.AlarmArea,
                    newAlarmArea.IdAlarmArea);
            }

            if (newAlarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null &&
                !newAlarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup.Compare(
                    oldAlarmAreaBeforUpdate.AlarmAreaSetByOnOffObjectFailedPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(
                    AlarmType.AlarmArea_SetByOnOffObjectFailed,
                    ObjectType.AlarmArea,
                    newAlarmArea.IdAlarmArea);
            }

            var inputs = oldAlarmAreaBeforUpdate.AAInputs.ToDictionary(key => key.IdAAInput, value => value);

            foreach (var aaInput in newAlarmArea.AAInputs)
                if (!inputs.ContainsKey(aaInput.IdAAInput))
                    inputs.Add(aaInput.IdAAInput, aaInput);

            ActualizeAlternaNameForInputs(inputs.Values);

            if (newAlarmArea.ObjForAutomaticAct != null
                && newAlarmArea.ObjForAutomaticAct.GetObjectType() == ObjectType.Input)
            {
                SetAlarmTamperStateForBSIInput(newAlarmArea.ObjForAutomaticAct.GetId());
            }

            if (newAlarmArea.ObjForForcedTimeBuying != null
                && newAlarmArea.ObjForForcedTimeBuying.GetObjectType() == ObjectType.Input)
            {
                SetAlarmTamperStateForBSIInput(newAlarmArea.ObjForForcedTimeBuying.GetId());
            }
        }

        private void SetAlarmTamperStateForBSIInput(object idInput)
        {
            var input = Inputs.Singleton.GetObjectForEdit(idInput);

            if (input != null
                && input.InputType == (byte)InputType.BSI)
            {
                input.AlarmTamper = true;
                Inputs.Singleton.Update(input);
            }

            Inputs.Singleton.EditEnd(input);
        }

        private void ActualizeAlternaNameForInputs(IEnumerable<AAInput> aaInputs)
        {
            if (aaInputs == null)
                return;

            foreach (var aaInput in aaInputs)
            {
                var inputForEdit = Inputs.Singleton.GetObjectForEdit(aaInput.Input.IdInput);

                if (inputForEdit != null)
                {
                    inputForEdit.ActualizeAlternateName();
                    Inputs.Singleton.Update(inputForEdit);
                    Inputs.Singleton.EditEnd(inputForEdit);
                }
            }
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(AlarmArea alarmArea)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    alarmArea.IdAlarmArea,
                    ObjectType.AlarmArea),
                1);
        }

        protected override IEnumerable<AlarmArea> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<AlarmArea>(
                alarmArea =>
                    alarmArea.LocalAlarmInstruction != null
                    && alarmArea.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterInsert(AlarmArea alarmArea)
        {
            base.AfterInsert(alarmArea);

            ActualizeAlternaNameForInputs(alarmArea.AAInputs);
        }

        public override void AfterDelete(AlarmArea alarmArea)
        {
            base.AfterDelete(alarmArea);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(alarmArea));

            ActualizeAlternaNameForInputs(alarmArea.AAInputs);
        }

        protected override void LoadObjectsInRelationship(AlarmArea obj)
        {
            if (obj.AAInputs != null)
            {
                IList<AAInput> list = new List<AAInput>();

                foreach (var aaInput in obj.AAInputs)
                {
                    list.Add(AAInputs.Singleton.GetById(aaInput.IdAAInput));
                }

                obj.AAInputs.Clear();
                foreach (var aaInput in list)
                    obj.AAInputs.Add(aaInput);
            }

            if (obj.AACardReaders != null)
            {
                IList<AACardReader> list = new List<AACardReader>();

                foreach (var aaCardReader in obj.AACardReaders)
                {
                    list.Add(AACardReaders.Singleton.GetById(aaCardReader.IdAACardReader));
                }

                obj.AACardReaders.Clear();
                foreach (var aaCardReader in list)
                    obj.AACardReaders.Add(aaCardReader);
            }

            if (obj.PresentationGroup != null)
                obj.PresentationGroup = PresentationGroups.Singleton.GetById(obj.PresentationGroup.IdGroup);

            if (obj.OutputActivation != null)
                obj.OutputActivation = Outputs.Singleton.GetById(obj.OutputActivation.IdOutput);

            if (obj.OutputAlarmState != null)
                obj.OutputAlarmState = Outputs.Singleton.GetById(obj.OutputAlarmState.IdOutput);

            if (obj.OutputPrewarning != null)
                obj.OutputPrewarning = Outputs.Singleton.GetById(obj.OutputPrewarning.IdOutput);

            if (obj.OutputTmpUnsetEntry != null)
                obj.OutputTmpUnsetEntry = Outputs.Singleton.GetById(obj.OutputTmpUnsetEntry.IdOutput);

            if (obj.OutputTmpUnsetExit != null)
                obj.OutputTmpUnsetExit = Outputs.Singleton.GetById(obj.OutputTmpUnsetExit.IdOutput);

            if (obj.OutputAAlarm != null)
                obj.OutputAAlarm = Outputs.Singleton.GetById(obj.OutputAAlarm.IdOutput);

            if (obj.OutputSiren != null)
                obj.OutputSiren = Outputs.Singleton.GetById(obj.OutputSiren.IdOutput);

            if (obj.OutputSabotage != null)
                obj.OutputSabotage = Outputs.Singleton.GetById(obj.OutputSabotage.IdOutput);

            if (obj.OutputNotAcknowledged != null)
                obj.OutputNotAcknowledged = Outputs.Singleton.GetById(obj.OutputNotAcknowledged.IdOutput);

            if (obj.OutputMotion != null)
                obj.OutputMotion = Outputs.Singleton.GetById(obj.OutputMotion.IdOutput);

            if (obj.OutputSetByObjectForAaFailed != null)
                obj.OutputSetByObjectForAaFailed = Outputs.Singleton.GetById(obj.OutputSetByObjectForAaFailed.IdOutput);

            if (obj.AlarmAreaAlarmArcs != null)
            {
                var alarmAreaAlarmArcs = new LinkedList<AlarmAreaAlarmArc>();

                foreach (var alarmAreaAlarmArc in obj.AlarmAreaAlarmArcs)
                {
                    alarmAreaAlarmArcs.AddLast(
                        AlarmAreaAlarmArcs.Singleton.GetById(
                            alarmAreaAlarmArc.IdAlarmAreaAlarmArc));
                }

                obj.AlarmAreaAlarmArcs.Clear();

                foreach (var alarmAreaAlarmArc in alarmAreaAlarmArcs)
                {
                    obj.AlarmAreaAlarmArcs.Add(alarmAreaAlarmArc);
                }
            }

            if (obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null)
            {
                obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup.IdGroup);
            }

            if (obj.SensorAlarmPresentationGroup != null)
            {
                obj.SensorAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.SensorAlarmPresentationGroup.IdGroup);
            }

            if (obj.SensorTamperAlarmPresentationGroup != null)
            {
                obj.SensorTamperAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.SensorTamperAlarmPresentationGroup.IdGroup);
            }
        }

        protected override void SaveOnOffObjects(AlarmArea ormObject)
        {
            if (ormObject.ObjForAutomaticAct != null)
            {
                string type;
                object id;

                ormObject.ObjForAutomaticAct.SaveToDatabase(out type, out id);
                ormObject.ObjForAutomaticActType = type;
                ormObject.ObjForAutomaticActId = id as Guid?;
            }
            else
            {
                ormObject.ObjForAutomaticActType = null;
                ormObject.ObjForAutomaticActId = null;
            }

            if (ormObject.ObjForForcedTimeBuying != null)
            {
                string type;
                object id;

                ormObject.ObjForForcedTimeBuying.SaveToDatabase(out type, out id);
                ormObject.ObjForForcedTimeBuyingType = type;
                ormObject.ObjForForcedTimeBuyingId = id as Guid?;
            }
            else
            {
                ormObject.ObjForForcedTimeBuyingType = null;
                ormObject.ObjForForcedTimeBuyingId = null;
            }
        }

        protected override void ReadOnOffObject(AlarmArea obj)
        {
            if (obj.ObjForAutomaticActType != null && obj.ObjForAutomaticActId != null)
            {
                obj.ObjForAutomaticAct = GetAOnOffObjectByTypeAndId(
                    obj.ObjForAutomaticActType, 
                    obj.ObjForAutomaticActId);
            }

            if (obj.ObjForForcedTimeBuyingType != null && obj.ObjForForcedTimeBuyingId != null)
            {
                obj.ObjForForcedTimeBuying = GetAOnOffObjectByTypeAndId(
                    obj.ObjForForcedTimeBuyingType,
                    obj.ObjForForcedTimeBuyingId);
            }
        }

        private static AOnOffObject GetAOnOffObjectByTypeAndId(string type, Guid? id)
        {
            var objType = AOnOffObject.GetOnOffObjectType(type);
            AOnOffObject onOffObject;

            if (objType == typeof (TimeZone))
            {
                onOffObject = TimeZones.Singleton.GetById(id);
            }
            else if (objType == typeof (DailyPlan))
            {
                onOffObject = DailyPlans.Singleton.GetById(id);
            }
            else if (objType == typeof (Input))
            {
                onOffObject = Inputs.Singleton.GetById(id);
            }
            else if (objType == typeof (Output))
            {
                onOffObject = Outputs.Singleton.GetById(id);
            }
            else
            {
                onOffObject = null;
            }
            return onOffObject;
        }

        public AlarmAreaActionResult SetAlarmArea(Guid alarmAreaGuid, bool noPrewarning)
        {
            var login = AccessChecker.GetActualLogin();

            // Check rights to unset alarm area
            bool allowedByLogin;
            bool timeBuyingMatrixFailed;

            if (!HasAccessForAlarmAreaAction(
                AccessNCAS.AlarmAreasSetPerform,
                alarmAreaGuid, login,
                out allowedByLogin,
                out timeBuyingMatrixFailed))
            {
                return AlarmAreaActionResult.FailedInsufficientRights;
            }

            // noPrewarning require rights for unconditional set
            if (noPrewarning
                && !HasAccessForAlarmAreaAction(
                    AccessNCAS.AlarmAreasUnconditionalSetPerform,
                    alarmAreaGuid,
                    login,
                    out allowedByLogin,
                    out timeBuyingMatrixFailed))
            {
                return AlarmAreaActionResult.FailedInsufficientRights;
            }

            var result = CCUConfigurationHandler.Singleton.SetAlarmArea(
                alarmAreaGuid,
                login.IdLogin,
                noPrewarning);

            return result;
        }

        public AlarmAreaActionResult UnsetAlarmArea(Guid alarmAreaGuid, int timeToBuy)
        {
            var login = AccessChecker.GetActualLogin();
            Guid guidPerson = Guid.Empty;
            bool timeBuyingMatrixFailed;

            if (timeToBuy > 0 || timeToBuy == -1)
            {
                bool enabledByLogin;

                bool unsetRights = HasAccessForAlarmAreaAction(
                    AccessNCAS.AlarmAreasTimeBuyingPerform,
                    alarmAreaGuid, login,
                    out enabledByLogin,
                    out timeBuyingMatrixFailed);

                if (!unsetRights)
                {
                    if (timeBuyingMatrixFailed)
                        return AlarmAreaActionResult.FailedTimeBuyingNotAvaible;

                    return AlarmAreaActionResult.FailedInsufficientRights;
                }

                // If action is not enabled by login it has to contain person guid to check ACL
                if (!enabledByLogin)
                    guidPerson = login.Person.IdPerson;
            }
            else if(timeToBuy == 0)
            {
                bool enabledByLogin;
                bool uunsetRights = HasAccessForAlarmAreaAction(
                    AccessNCAS.AlarmAreasUnsetPerform,
                    alarmAreaGuid, login,
                    out enabledByLogin,
                    out timeBuyingMatrixFailed);

                if (!uunsetRights)
                    return AlarmAreaActionResult.FailedInsufficientRights;

                // If action is not enabled by login it has to contain person guid to check ACL
                if (!enabledByLogin)
                    guidPerson = login.Person.IdPerson;
            }
            else
            {
                return AlarmAreaActionResult.FailedTimeToBuyIsZero;
            }

            var result = CCUConfigurationHandler.Singleton.UnsetAlarmArea(alarmAreaGuid, login.IdLogin, guidPerson, timeToBuy);

            return result;
        }

        public AlarmAreaActionResult UnconditionalSetAlarmArea(Guid alarmAreaGuid, bool noPrewarning)
        {
            var login = AccessChecker.GetActualLogin();

            // Check rights for unconditional set
            bool allowedByLogin;
            bool timeBuyingMatrixFailed;

            if (!HasAccessForAlarmAreaAction(
                AccessNCAS.AlarmAreasUnconditionalSetPerform,
                alarmAreaGuid,
                login,
                out allowedByLogin,
                out timeBuyingMatrixFailed))
            {
                return AlarmAreaActionResult.FailedInsufficientRights;
            }

            return CCUConfigurationHandler.Singleton.UnconditionalSetAlarmArea(
                alarmAreaGuid,
                login.IdLogin,
                noPrewarning);
        }

        public ActivationState GetAlarmAreaActivationState(Guid alarmAreaGuid)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaActivationState(alarmAreaGuid);
        }

        public RequestActivationState GetAlarmAreaRequestActivationState(Guid alarmAreaGuid)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaRequestActivationState(alarmAreaGuid);
        }

        private static bool IsActivated(Guid alarmAreaGuid)
        {
            var aaAlarmState = CCUConfigurationHandler.Singleton.GetAlarmAreaActivationState(alarmAreaGuid);

            switch (aaAlarmState)
            {
                case ActivationState.Set:
                case ActivationState.TemporaryUnsetExit:
                case ActivationState.TemporaryUnsetEntry:
                    return true;
            }

            return false;
        }

        public AlarmAreaAlarmState GetAlarmAreaAlarmState(Guid alarmAreaGuid)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaAlarmState(alarmAreaGuid);
        }

        public State GetAlarmAreaSabotageState(Guid alarmAreaGuid)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaSabotageState(alarmAreaGuid);
        }

        public CCU GetImplicitCCUForAlarmArea(Guid guidAlarmArea)
        {
            return CCUConfigurationHandler.Singleton.GetImplicitCCUForAlarmArea(guidAlarmArea);
        }

        public IList<Output> GetSpecialOutputs(Guid alarmAreaGuid)
        {
            var ccu = CCUConfigurationHandler.Singleton.GetImplicitCCUForAlarmArea(alarmAreaGuid);

            if (ccu != null)
            {
                var outputsFromCCU = Outputs.Singleton.GetOutputsFromCCUAndItsDCUs(ccu);
                outputsFromCCU = Outputs.Singleton.FilterOutputsFromActivators(outputsFromCCU, null, alarmAreaGuid);

                var outputs = new List<Output>();
                if (outputsFromCCU != null && outputsFromCCU.Count > 0)
                {
                    foreach (var output in outputsFromCCU)
                    {
                        if (output != null && !IsOutputInAlarmAreasAsEISSetUnsetOutput(output, alarmAreaGuid))
                        {
                            outputs.Add(output);
                        }
                    }
                }

                return outputs;
            }

            return null;
        }

        public bool IsOutputInAlarmAreas(Guid outputGuid)
        {
            var output = Outputs.Singleton.GetById(outputGuid);
            if (output == null)
                return false;

            return IsOutputInAlarmAreas(output, Guid.Empty);
        }

        public bool IsOutputInAlarmAreas(Output output, Guid idAlarmArea)
        {
            if (output == null)
                return false;

            var alarmAreas = UsedOutputAsSpecialOutputOrEisOutputInAlarmAreas(output);

            return alarmAreas != null
                   && alarmAreas.Any(
                       alarmArea =>
                           alarmArea.IdAlarmArea != idAlarmArea);
        }

        /// <summary>
        /// Return true if output is used as set/umset otuput for EIS in at least one alarm area. Otherwise return false.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="idAlarmArea"></param>
        /// <returns></returns>
        public bool IsOutputInAlarmAreasAsEISSetUnsetOutput(Output output, Guid idAlarmArea)
        {
            if (output == null)
                return false;

            var alarmAreas = SelectLinq<AlarmArea>(alarmArea => alarmArea.IdAlarmArea != idAlarmArea &&
                alarmArea.SetUnsetOutputEIS == output.IdOutput);
            return alarmAreas != null && alarmAreas.Count > 0;
        }

        /// <summary>
        /// Return true if input is used in at least one alarm area. Otherwise return false.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="idAlarmArea"></param>
        /// <returns></returns>
        public bool IsInputInAlarmAreas(Input input, Guid idAlarmArea)
        {
            if (input == null)
                return false;

            var alarmAreas = SelectLinq<AlarmArea>(alarmArea => alarmArea.IdAlarmArea != idAlarmArea &&
                alarmArea.ActivationStateInputEIS == input.IdInput);
            return alarmAreas != null && alarmAreas.Count > 0;
        }

        /// <summary>
        /// Return true if output is used as special output for siren in at least one alarm area. Otherwise return false.
        /// </summary>
        /// <param name="outputGuid"></param>
        /// <returns></returns>
        public bool IsOutputUsedAsSiren(Guid outputGuid)
        {
            var output = Outputs.Singleton.GetById(outputGuid);
            if (output == null)
                return false;

            var alarmAreas = SelectLinq<AlarmArea>(alarmArea => alarmArea.OutputSiren == output);
            return alarmAreas != null && alarmAreas.Count > 0;
        }

        public List<Output> GetOutputsUsedInAlarmAreas()
        {
            var result = new List<Output>();
            var idOutputs = new HashSet<Guid>();

            var alarmAreas = List().ToList();
            foreach (var alarmArea in alarmAreas)
            {
                if (alarmArea.OutputActivation != null && idOutputs.Add(alarmArea.OutputActivation.IdOutput))
                    result.Add(alarmArea.OutputActivation);
                if (alarmArea.OutputAlarmState != null && idOutputs.Add(alarmArea.OutputAlarmState.IdOutput))
                    result.Add(alarmArea.OutputAlarmState);
                if (alarmArea.OutputPrewarning != null && idOutputs.Add(alarmArea.OutputPrewarning.IdOutput))
                    result.Add(alarmArea.OutputPrewarning);
                if (alarmArea.OutputTmpUnsetEntry != null && idOutputs.Add(alarmArea.OutputTmpUnsetEntry.IdOutput))
                    result.Add(alarmArea.OutputTmpUnsetEntry);
                if (alarmArea.OutputTmpUnsetExit != null && idOutputs.Add(alarmArea.OutputTmpUnsetExit.IdOutput))
                    result.Add(alarmArea.OutputTmpUnsetExit);
                if (alarmArea.OutputAAlarm != null && idOutputs.Add(alarmArea.OutputAAlarm.IdOutput))
                    result.Add(alarmArea.OutputAAlarm);
                if (alarmArea.OutputSiren != null && idOutputs.Add(alarmArea.OutputSiren.IdOutput))
                    result.Add(alarmArea.OutputSiren);
                if (alarmArea.OutputSabotage != null && idOutputs.Add(alarmArea.OutputSabotage.IdOutput))
                    result.Add(alarmArea.OutputSabotage);
                if (alarmArea.OutputNotAcknowledged != null && idOutputs.Add(alarmArea.OutputNotAcknowledged.IdOutput))
                    result.Add(alarmArea.OutputNotAcknowledged);
                if (alarmArea.OutputMotion != null && idOutputs.Add(alarmArea.OutputMotion.IdOutput))
                    result.Add(alarmArea.OutputMotion);
                if (alarmArea.OutputSetByObjectForAaFailed != null && idOutputs.Add(alarmArea.OutputSetByObjectForAaFailed.IdOutput))
                    result.Add(alarmArea.OutputSetByObjectForAaFailed);
            }

            return result;
        }

        public ICollection<AlarmArea> UsedOutputInAlarmAreas(Output output)
        {
            if (output == null)
            {
                return null;
            }

            var dicAlarmAreas = new SyncDictionary<Guid, AlarmArea>();

            var alarmAreasSpecialEisOutput = UsedOutputAsSpecialOutputOrEisOutputInAlarmAreas(output);
            if (alarmAreasSpecialEisOutput != null && alarmAreasSpecialEisOutput.Count > 0)
            {
                foreach (var alarmArea in alarmAreasSpecialEisOutput)
                {
                    if (alarmArea != null)
                    {
                        dicAlarmAreas[alarmArea.IdAlarmArea] = alarmArea;
                    }
                }
            }

            var alarmAreasOnOffObject = UsedLikeOnOffObject(output);
            if (alarmAreasOnOffObject != null && alarmAreasOnOffObject.Count > 0)
            {
                foreach (var alarmArea in alarmAreasOnOffObject)
                {
                    if (alarmArea != null)
                    {
                        dicAlarmAreas[alarmArea.IdAlarmArea] = alarmArea;
                    }
                }
            }

            return dicAlarmAreas.Count > 0 ? dicAlarmAreas.ValuesSnapshot : null;
        }

        private ICollection<AlarmArea> UsedOutputAsSpecialOutputOrEisOutputInAlarmAreas(Output output)
        {
            if (output == null)
            {
                return null;
            }

            return
                SelectLinq<AlarmArea>(
                    alarmArea =>
                        (alarmArea.OutputActivation != null
                         && alarmArea.OutputActivation.IdOutput == output.IdOutput)
                        || (alarmArea.OutputAlarmState != null
                            && alarmArea.OutputAlarmState.IdOutput == output.IdOutput)
                        || (alarmArea.OutputPrewarning != null
                            && alarmArea.OutputPrewarning.IdOutput == output.IdOutput)
                        || (alarmArea.OutputTmpUnsetEntry != null
                            && alarmArea.OutputTmpUnsetEntry.IdOutput == output.IdOutput)
                        || (alarmArea.OutputTmpUnsetExit != null
                            && alarmArea.OutputTmpUnsetExit.IdOutput == output.IdOutput)
                        || (alarmArea.OutputAAlarm != null
                            && alarmArea.OutputAAlarm.IdOutput == output.IdOutput)
                        || (alarmArea.OutputSiren != null
                            && alarmArea.OutputSiren.IdOutput == output.IdOutput)
                        || alarmArea.SetUnsetOutputEIS == output.IdOutput
                        || (alarmArea.OutputSabotage != null
                            && alarmArea.OutputSabotage.IdOutput == output.IdOutput)
                        || (alarmArea.OutputNotAcknowledged != null
                            && alarmArea.OutputNotAcknowledged.IdOutput == output.IdOutput)
                        || (alarmArea.OutputMotion != null
                            && alarmArea.OutputMotion.IdOutput == output.IdOutput)
                        || (alarmArea.OutputSetByObjectForAaFailed != null
                            && alarmArea.OutputSetByObjectForAaFailed.IdOutput == output.IdOutput));
        }

        public ICollection<AlarmArea> UsedInputInAlarmAreas(Input input)
        {
            if (input == null)
                return null;

            var dicAlarmAreas = new SyncDictionary<Guid, AlarmArea>();

            var alarmAreasEisInput = 
                UsedInputAsEisInputInAlarmAreas(input);

            if (alarmAreasEisInput != null)
                foreach (var alarmArea in alarmAreasEisInput)
                    if (alarmArea != null)
                        dicAlarmAreas[alarmArea.IdAlarmArea] = alarmArea;

            var alarmAreasOnOffObject = 
                UsedLikeOnOffObject(input);

            if (alarmAreasOnOffObject != null)
                foreach (var alarmArea in alarmAreasOnOffObject)
                    if (alarmArea != null)
                        dicAlarmAreas[alarmArea.IdAlarmArea] = alarmArea;

            return 
                dicAlarmAreas.Count > 0
                    ? dicAlarmAreas.ValuesSnapshot
                    : null;
        }

        public ICollection<AlarmArea> UsedInputAsEisInputInAlarmAreas(Input input)
        {
            return 
                input != null
                    ? SelectLinq<AlarmArea>(
                        alarmArea => alarmArea.ActivationStateInputEIS == input.IdInput) 
                    : null;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var alarmArea = GetById(idObj);

                if (alarmArea == null) 
                    return null;

                var result = new List<AOrmObject>();

                var usedInAcl = 
                    ACLSettings.Singleton.UsedLikeCardReaderObject(
                        alarmArea.IdAlarmArea, 
                        ObjectType.AlarmArea);

                if (usedInAcl != null)
                    foreach (var acl in usedInAcl)
                        result.Add(AccessControlLists.Singleton.GetById(acl.IdAccessControlList));

                var usedInAz = 
                    AccessZones.Singleton.UsedLikeCardReaderObject(
                        alarmArea.IdAlarmArea, 
                        ObjectType.AlarmArea);

                if (usedInAz != null)
                    foreach (var person in usedInAz)
                        result.Add(Persons.Singleton.GetById(person.IdPerson));

                var usedInAclAa = 
                    ACLSettingAAs.Singleton.GetReferencedAclFormAa(alarmArea);

                if (usedInAclAa != null)
                    foreach (var acl in usedInAclAa)
                        result.Add(AccessControlLists.Singleton.GetById(acl.IdAccessControlList));

                var multiDoors = MultiDoors.Singleton.GetMultiDoorsForAlarmArea(alarmArea.IdAlarmArea);
                if (multiDoors != null)
                {
                    result.AddRange(
                        multiDoors.Select(
                            multiDoorProxy =>
                                MultiDoors.Singleton.GetById(multiDoorProxy.IdMultiDoor))
                            .Cast<AOrmObject>());
                }

                var floors = Floors.Singleton.GetFloorsForAlarmArea(alarmArea.IdAlarmArea);
                if (floors != null)
                {
                    result.AddRange(
                        floors.Select(
                            floorProxy =>
                                MultiDoors.Singleton.GetById(floorProxy.IdFloor))
                            .Cast<AOrmObject>());
                }

                var multiDoorElements = MultiDoorElements.Singleton.GetMultiDoorElementsForAlarmArea(alarmArea.IdAlarmArea);
                if (multiDoorElements != null)
                {
                    result.AddRange(
                        multiDoorElements.Select(
                            multiDoorElementProxy =>
                                MultiDoorElements.Singleton.GetById(multiDoorElementProxy.IdMultiDoorElement))
                            .Cast<AOrmObject>());
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

        public ICollection<AlarmArea> GetAlarmAreasForPg(PresentationGroup pg)
        {
            try
            {
                return 
                    pg != null
                        ? SelectLinq<AlarmArea>(
                            alarmArea => alarmArea.PresentationGroup == pg
                            || alarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup == pg)
                        : null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsInputInOtherActivatedAlarmArea(Guid inputGuid, Guid alarmAreaGuid)
        {
            try
            {
                ICollection<AlarmArea> alarmAreas =
                    SelectLinq<AlarmArea>(
                        alarmArea => alarmArea.IdAlarmArea != alarmAreaGuid);

                foreach (var alarmArea in alarmAreas)
                {
                    if (!IsSensorForAlarmArea(inputGuid, alarmArea.IdAlarmArea))
                        continue;

                    if (!IsActivated(alarmArea.IdAlarmArea))
                        continue;

                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public bool IsSensorForAlarmArea(Guid inputGuid, Guid alarmAreaGuid)
        {
            var selectedAlarmArea = 
                SelectLinq<AlarmArea>(
                    alarmArea => alarmArea.IdAlarmArea == alarmAreaGuid)
                .FirstOrDefault();

            if (selectedAlarmArea == null)
                return false;

            return 
                selectedAlarmArea.AAInputs
                    .Any(
                        aaInput => 
                            inputGuid == aaInput.Input.IdInput && 
                            Inputs.Singleton.IsActivated(aaInput.Input));
        }

        public ICollection<AlarmArea> UsedLikeOnOffObject(AOnOffObject onOffObj)
        {
            try
            {
                var type = onOffObj.GetType();

                var typeName = 
                    Assembly.CreateQualifiedName(
                        type.Assembly.GetName().Name, 
                        type.FullName);

                var idOnOffObj = onOffObj.GetId() as Guid?;

                return SelectLinq<AlarmArea>(
                    alarmArea => 
                        (alarmArea.ObjForAutomaticActId == idOnOffObj 
                            && alarmArea.ObjForAutomaticActType == typeName)
                        || (alarmArea.ObjForForcedTimeBuyingId == idOnOffObj
                            && alarmArea.ObjForAutomaticActType == typeName));
            }
            catch
            {
                return null;
            }
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<AlarmArea> linqResult;

            if (single && string.IsNullOrEmpty(name))
            {
                linqResult = List();
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                linqResult =
                    single
                        ? SelectLinq<AlarmArea>(aa => aa.ToString().IndexOf(name) >= 0)
                        : SelectLinq<AlarmArea>(
                            aa =>
                                aa.ToString().IndexOf(name) >= 0 ||
                                aa.Description.IndexOf(name) >= 0);
            }

            if (linqResult == null)
                return resultList;

            linqResult = linqResult.OrderBy(aa => aa.ToString()).ToList();

            foreach (var aa in linqResult)
                resultList.Add(aa);

            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<AlarmArea> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<AlarmArea>(aa => aa.FullTextSearchString.ToLower().Contains(name.ToLower())
                                                || aa.Description.ToLower().Contains(name.ToLower()));
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<AlarmArea>(aa => aa.FullTextSearchString.ToLower().Contains(name.ToLower())).ToList();

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<AlarmArea> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();

            if (linqResult == null)
                return resultList;

            linqResult = linqResult.OrderBy(aa => aa.ToString()).ToList();

            foreach (var aa in linqResult)
                resultList.Add(aa);

            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<AlarmAreaShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings, 
            out Exception error)
        {
            var listAlarmArea = SelectByCriteria(filterSettings, out error);
            ICollection<AlarmAreaShort> result = new List<AlarmAreaShort>();
            if (listAlarmArea != null)
            {
                foreach (var alarmArea in listAlarmArea)
                {
                    var alarmAreaActivationState =
                        GetAlarmAreaActivationState(alarmArea.IdAlarmArea);

                    var alarmAreaRequestActivationState =
                        GetAlarmAreaRequestActivationState(alarmArea.IdAlarmArea);

                    var alarmAreaAlarmState =
                        GetAlarmAreaAlarmState(alarmArea.IdAlarmArea);

                    var aaShort = new AlarmAreaShort(alarmArea)
                    {
                        ActivationState = alarmAreaActivationState,
                        StringActivationState = alarmAreaActivationState.ToString(),
                        RequestActivationState = alarmAreaRequestActivationState,
                        StringRequestActivationState = alarmAreaRequestActivationState.ToString(),
                        AlarmState = alarmAreaAlarmState,
                        StringAlarmState = alarmAreaAlarmState.ToString()
                    };
                    result.Add(aaShort);
                }
            }
            return result;
        }

        public ICollection<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listAlarmArea = List(out error);
            if (listAlarmArea == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    listAlarmArea.Select(alarmArea => new AlarmAreaModifyObj(alarmArea))
                        .OrderBy(alarmAreaShort => alarmAreaShort.ToString())
                        .Cast<IModifyObject>());
        }

        public ICollection<IModifyObject> ListModifyObjects(Guid? ccuid, out Exception error)
        {
            if (ccuid == null)
                return ListModifyObjects(out error);

            var listAlarmArea = List(out error);
            if (listAlarmArea == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    listAlarmArea
                        .Where(alarmArea =>
                        {
                            var ccu = GetImplicitCCUForAlarmArea(alarmArea.IdAlarmArea);
                            return ccu != null && ccu.IdCCU == ccuid;
                        })
                        .Select(alarmArea => new AlarmAreaModifyObj(alarmArea))
                        .OrderBy(alarmAreaShort => alarmAreaShort.ToString())
                        .Cast<IModifyObject>());
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idAlarmArea)
        {
            var alarmarea = GetById(idAlarmArea);
            if (ccus != null && alarmarea != null)
            {
                var ccu = GetImplicitCCUForAlarmArea(alarmarea.IdAlarmArea);
                if (ccu != null)
                {
                    CCUs.Singleton.GetParentCCU(ccus, ccu.IdCCU);
                }
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idAlarmArea)
        {
            var objects = new List<AOrmObject>();

            var alarmarea = GetById(idAlarmArea);
            if (alarmarea != null)
            {
                var ccu = GetImplicitCCUForAlarmArea(alarmarea.IdAlarmArea);
                if (ccu != null && ccu.IdCCU == guidCCU)
                {
                    if (alarmarea.AACardReaders != null)
                    {
                        foreach (var item in alarmarea.AACardReaders)
                        {
                            if (item != null)
                            {
                                objects.Add(item);
                            }
                        }
                    }
                    if (alarmarea.AAInputs != null)
                    {
                        foreach (var item in alarmarea.AAInputs)
                        {
                            if (item != null)
                            {
                                objects.Add(item);
                            }
                        }
                    }

                    if (alarmarea.ObjForAutomaticAct != null
                        && DataReplicationManager.OnOffObjectShouldBeStoredOnCcu(
                            guidCCU,
                            alarmarea.ObjForAutomaticAct))
                    {
                        objects.Add(alarmarea.ObjForAutomaticAct);
                    }

                    if (alarmarea.ObjForForcedTimeBuying != null)
                    {
                        objects.Add(alarmarea.ObjForForcedTimeBuying);
                    }

                    if (alarmarea.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType != null 
                        && alarmarea.ObjBlockAlarmAreaSetByOnOffObjectFailedId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                            alarmarea.ObjBlockAlarmAreaSetByOnOffObjectFailedId.Value,
                            alarmarea.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType.Value);

                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (alarmarea.AlarmAreaAlarmArcs != null)
                    {
                        objects.AddRange(alarmarea.AlarmAreaAlarmArcs.Select(
                            alarmAreaAlarmArc =>
                                AlarmArcs.Singleton.GetById(alarmAreaAlarmArc.IdAlarmArc))
                            .Cast<AOrmObject>());
                    }
                }
            }

            return objects;
        }

        public bool SetCardReaderToAlarmArea(Guid guidCardReader, Guid guidImplicitCCU)
        {
            if (guidImplicitCCU != Guid.Empty)
            {
                var ccus = new List<Guid>();
                CardReaders.Singleton.GetParentCCU(ccus, guidCardReader);

                if (!ccus.Contains(guidImplicitCCU))
                    return false;
            }

            return true;
        }

        public bool SetSensorToAlarmArea(Guid guidInput, Guid guidImplicitCCU)
        {
            if (guidImplicitCCU != Guid.Empty)
            {
                var ccus = new List<Guid>();
                Inputs.Singleton.GetParentCCU(ccus, guidInput);

                if (!ccus.Contains(guidImplicitCCU))
                    return false;
            }

            return true;
        }

        public void InheritGeneralSettingsChanged(bool isOn)
        {
            CCUConfigurationHandler.Singleton.AlarmsAreaReportingToCRChanged(isOn);
        }

        public bool HasAccessForAlarmAreaActionFromCurrentLogin(AccessNCAS action, Guid alarmAreaGuid)
        {
            var login = AccessChecker.GetActualLogin();
            bool allowedByLogin;
            bool timeBuyingMatrixFailed;

            return HasAccessForAlarmAreaAction(
                action,
                alarmAreaGuid,
                login,
                out allowedByLogin,
                out timeBuyingMatrixFailed);
        }

        private static bool HasAccessForAlarmAreaAction(
            AccessNCAS action, 
            Guid alarmAreaGuid, 
            Login login, 
            out bool allowedByLogin,
            out bool timeBuyingMatrixFailed)
        {
            if (action != AccessNCAS.AlarmAreasSetPerform
                && action != AccessNCAS.AlarmAreasUnconditionalSetPerform
                && action != AccessNCAS.AlarmAreasUnsetPerform
                && action != AccessNCAS.AlarmAreasTimeBuyingPerform)
            {
                throw new NotSupportedException(
                    "Invalid alarm area action. Supproted actions: AlarmAreasSet, AlarmAreasUnconditionalSet, AlarmAreasUnset, AlarmAreasTimeBuying");
            }

            timeBuyingMatrixFailed = false;

            // Check if access is granted by login
            if (action == AccessNCAS.AlarmAreasSetPerform
                || action == AccessNCAS.AlarmAreasUnconditionalSetPerform)
            {
                if (AccessChecker.HasAccessControl(
                    NCASAccess.GetAccess(action),
                    login))
                {
                    allowedByLogin = true;
                    return true;
                }
            }
            else if (HasAccessForAlarmAreaActionByTimeBuyingMatrix(
                alarmAreaGuid,
                action,
                AccessChecker.HasAccessControl(
                    NCASAccess.GetAccess(AccessNCAS.AlarmAreasUnsetPerform),
                    login),
                AccessChecker.HasAccessControl(
                    NCASAccess.GetAccess(AccessNCAS.AlarmAreasTimeBuyingPerform),
                    login)))
            {
                allowedByLogin = true;
                return true;
            }
            else
                timeBuyingMatrixFailed = true;

            allowedByLogin = false;

            // Access is denied by login and can not be granted by person if is null
            if (login.Person == null)
                return false;

            // Check access according to person
            var aclSetting =
                ACLPersons.Singleton.GetAclForPerson(login.Person)
                .Select(acl => acl.ACLSettingAAs)
                .Where(aCLSettingAAs => aCLSettingAAs != null)
                .SelectMany(aclSettnigsAA => aclSettnigsAA)
                .FirstOrDefault(aclSettingAA => aclSettingAA.AlarmArea != null
                            && aclSettingAA.AlarmArea.IdAlarmArea == alarmAreaGuid);

            // Person do not have required ACLSetting
            if (aclSetting == null)
                return false;

            switch (action)
            {
                case AccessNCAS.AlarmAreasSetPerform:
                    return aclSetting.AlarmAreaSet;

                case AccessNCAS.AlarmAreasUnconditionalSetPerform:
                    return aclSetting.AlarmAreaUnconditionalSet;

                case AccessNCAS.AlarmAreasUnsetPerform:
                    timeBuyingMatrixFailed = !HasAccessForAlarmAreaActionByTimeBuyingMatrix(
                        alarmAreaGuid,
                        AccessNCAS.AlarmAreasUnsetPerform,
                        aclSetting.AlarmAreaUnset,
                        aclSetting.AlarmAreaTimeBuying);
                    return !timeBuyingMatrixFailed;

                case AccessNCAS.AlarmAreasTimeBuyingPerform:
                    timeBuyingMatrixFailed = !HasAccessForAlarmAreaActionByTimeBuyingMatrix(
                        alarmAreaGuid,
                        AccessNCAS.AlarmAreasTimeBuyingPerform,
                        aclSetting.AlarmAreaUnset,
                        aclSetting.AlarmAreaTimeBuying);
                    return !timeBuyingMatrixFailed;
            }

            return false;
        }

        private static bool HasAccessForAlarmAreaActionByTimeBuyingMatrix(
            Guid idAlarmArea,
            AccessNCAS action,
            bool hasAccessForUnsetAction,
            bool hasAccessForTimeBuyingAction)
        {
            var timeBuyingMatrixState = CCUConfigurationHandler.Singleton.GetTimeBuyingMatrixState(idAlarmArea);

            switch (action)
            {
                case AccessNCAS.AlarmAreasTimeBuyingPerform:
                    return
                        hasAccessForTimeBuyingAction
                        && (!timeBuyingMatrixState.HasValue
                            || timeBuyingMatrixState.Value == TimeBuyingMatrixState.O4AA_ON_AND_O4TBA_ON
                            || timeBuyingMatrixState.Value == TimeBuyingMatrixState.O4TBA_ON);

                case AccessNCAS.AlarmAreasUnsetPerform:
                    return hasAccessForUnsetAction
                           || (timeBuyingMatrixState.HasValue
                               && hasAccessForTimeBuyingAction
                               && (timeBuyingMatrixState.Value == TimeBuyingMatrixState.O4AA_OFF_AND_O4TBA_OFF
                                   || timeBuyingMatrixState.Value == TimeBuyingMatrixState.O4AA_OFF_AND_O4TBA_ON
                                   || timeBuyingMatrixState.Value == TimeBuyingMatrixState.O4TBA_OFF));

                default:
                    return false;
            }
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.AlarmArea; }
        }

        public bool ShowExtendedTimeBuying
        {
            get
            {
                return GeneralOptions.Singleton.ShowHiddenFeature(
                    CgpServerGlobals.EXTENDED_TIMEBUYING_FEATURE_NAME);
            }
        }

        public int GetNewId()
        {
            var alarmAreas = List();

            if (alarmAreas == null)
                return 1;

            var alarmAreasId = new HashSet<int>(
                alarmAreas.Select(
                    alarmArea =>
                        alarmArea.Id));

            for (var newId = 1; newId < 100; newId++)
            {
                if (!alarmAreasId.Contains(newId))
                    return newId;
            }

            return 1;
        }

        public AlarmArea GetAlarmAreaBySectionId(int sectionId)
        {
            return SelectLinq<AlarmArea>(
                alarmArea => alarmArea.Id == sectionId).FirstOrDefault();
        }

        public bool ReplaceAlarmAreasSectionId(AlarmArea existingAlarmArea, AlarmArea editingAlarmArea)
        {
            bool editAllowed;
            Exception error;

            existingAlarmArea = GetObjectForEditById(existingAlarmArea.IdAlarmArea, out editAllowed);

            if (!editAllowed)
                return false;

            int existingAlarmAreaSectionId = existingAlarmArea.Id;
            existingAlarmArea.Id = GetNewId();

            if (!Update(existingAlarmArea, out error))
                return false;

            if (error != null)
                return false;

            int newIdForExistingAlarmArea = GetNewId();

            if (editingAlarmArea.IdAlarmArea != Guid.Empty)
            {
                editingAlarmArea = GetObjectForEditById(editingAlarmArea.IdAlarmArea, out editAllowed);

                if (!editAllowed)
                    return false;

                newIdForExistingAlarmArea = editingAlarmArea.Id;
                editingAlarmArea.Id = existingAlarmAreaSectionId;

                if (!Update(editingAlarmArea, out error))
                    return false;

                EditEnd(editingAlarmArea);

                if (error != null)
                    return false;
            }
            else
            {
                editingAlarmArea.Id = existingAlarmAreaSectionId;

                if (!Insert(editingAlarmArea, out error))
                    return false;

                if (error != null)
                    return false;
            }

            existingAlarmArea.Id = newIdForExistingAlarmArea;

            if (!Update(existingAlarmArea, out error))
                return false;

            EditEnd(existingAlarmArea);

            if (error != null)
                return false;

            return true;
        }

        public State? GetSensorState(
            Guid idAlarmArea,
            Guid idInput)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaSensorState(
                idAlarmArea,
                idInput);
        }

        public SensorBlockingType? GetSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput)
        {
            return CCUConfigurationHandler.Singleton.GetAlarmAreaSensorBlockingType(
                idAlarmArea,
                idInput);
        }

        public void SetAlarmAreaSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
            CCUConfigurationHandler.Singleton.SetSensorBlockingType(
                idAlarmArea,
                idInput,
                sensorBlockingType);
        }

        public string GetSensorSectionId(
            Guid idInput,
            Guid idAlarmArea)
        {
            var alarmArea = GetById(idAlarmArea);

            if (alarmArea == null
                || alarmArea.AAInputs == null)
            {
                return string.Empty;
            }

            foreach (var aaInput in alarmArea.AAInputs)
            {
                if (aaInput.Input == null
                    || aaInput.Input.IdInput != idInput)
                    continue;

                return string.Format(
                    "{0}{1}",
                    alarmArea.Id.ToString("D2"),
                    aaInput.Id.ToString("D2"));
            }

            return string.Empty;
        }

        public TimeBuyingMatrixState? GetTimeBuyingMatrixState(Guid idAlarmArea)
        {
            return CCUConfigurationHandler.Singleton.GetTimeBuyingMatrixState(idAlarmArea);
        }
    }
}
