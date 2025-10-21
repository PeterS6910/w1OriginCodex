using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Globals;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class Outputs :
        ANcasBaseOrmTableWithAlarmInstruction<Outputs, Output>,
        IOutputs
    {
        private Outputs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<Output>())
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var output = ormObject as Output;

            if (output == null)
                return null;

            var dcu = output.DCU;

            return
                dcu != null
                    ? (AOrmObject)dcu
                    : output.CCU;
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(Output output)
        {
            ReadOnOffObject(output);
            yield return output.OnOffObject;

            yield return output.AlarmPresentationGroup;
        }

        protected override IModifyObject CreateModifyObject(Output ormbObject)
        {
            return new OutputModifyObj(ormbObject);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.OUTPUTS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.OutputsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.OUTPUTS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.OutputsInsertDeletePerform), login);
        }

        public override void CUDSpecial(Output output, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        output.GetId(),
                        output.GetObjectType()));
            }
            else if (output != null)
            {
                if (objectDatabaseAction == ObjectDatabaseAction.Insert
                    && output.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetObjectForEdit(output.DCU.IdDCU);
                    DCUs.Singleton.Update(dcu);
                    DCUs.Singleton.EditEnd(dcu);

                    return;
                }

                DataReplicationManager.Singleton.SendModifiedObjectToCcus(output);
            }
        }

        protected override IEnumerable<Output> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<Output>(
                output =>
                    output.LocalAlarmInstruction != null
                    && output.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterUpdate(Output newOutput, Output oldOutputBeforeUpdate)
        {
            if (newOutput == null)
            {
                return;
            }

            base.AfterUpdate(
                newOutput,
                oldOutputBeforeUpdate);

            var idParentObject = Guid.Empty;
            if (newOutput.CCU != null)
            {
                idParentObject = newOutput.CCU.IdCCU;
            }
            else if (newOutput.DCU != null)
            {
                idParentObject = newOutput.DCU.IdDCU;
            }

            object[] objOutput = { idParentObject, newOutput.IdOutput };
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunOutputEditChanged,
                DelegateSequenceBlockingMode.Asynchronous, false, objOutput);

            if (oldOutputBeforeUpdate == null)
            {
                return;
            }

            if (newOutput.AlarmPresentationGroup != null &&
                !newOutput.AlarmPresentationGroup.Compare(oldOutputBeforeUpdate.AlarmPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Output_Alarm, ObjectType.Output,
                    newOutput.IdOutput);
            }
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(Output output)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    output.IdOutput,
                    ObjectType.Output),
                1);
        }

        public override void AfterDelete(Output output)
        {
            base.AfterDelete(output);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(output));
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(Output.COLUMNCCU, false));
            c.AddOrder(new Order(Output.COLUMNDCU, false));
            c.AddOrder(new Order(Output.COLUMNOUTPUTNUMBER, true));
        }

        protected override void LoadObjectsInRelationship(Output obj)
        {
            if (obj.AlarmPresentationGroup != null)
            {
                obj.AlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmPresentationGroup.IdGroup);
            }

            if (obj.DCU != null)
            {
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);
                if (obj.DCU.CCU != null)
                {
                    obj.DCU.CCU = CCUs.Singleton.GetById(obj.DCU.CCU.IdCCU);
                }
            }

            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
            }
        }

        protected override void LoadObjectsInRelationshipGetById(Output obj)
        {
            if (obj.AlarmPresentationGroup != null)
            {
                obj.AlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmPresentationGroup.IdGroup);
            }

            if (obj.DCU != null)
            {
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);
                if (obj.DCU.CCU != null)
                {
                    obj.DCU.CCU = CCUs.Singleton.GetById(obj.DCU.CCU.IdCCU);
                }
            }

            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
            }
        }

        protected override void SaveOnOffObjects(Output ormObject)
        {
            if (ormObject.OnOffObject != null)
            {
                string type;
                object id;
                ormObject.OnOffObject.SaveToDatabase(out type, out id);
                ormObject.OnOffObjectType = type;
                ormObject.OnOffObjectId = id as Guid?;
            }
            else
            {
                ormObject.OnOffObjectType = null;
                ormObject.OnOffObjectId = null;
            }
        }

        protected override void ReadOnOffObject(Output obj)
        {
            if (obj.OnOffObjectType != null && obj.OnOffObjectId != null)
            {
                var objType = AOnOffObject.GetOnOffObjectType(obj.OnOffObjectType);

                if (objType == typeof(TimeZone))
                {
                    obj.OnOffObject = TimeZones.Singleton.GetById(obj.OnOffObjectId);
                }
                else if (objType == typeof(DailyPlan))
                {
                    obj.OnOffObject = DailyPlans.Singleton.GetById(obj.OnOffObjectId);
                }
                else if (objType == typeof(Input))
                {
                    obj.OnOffObject = Inputs.Singleton.GetById(obj.OnOffObjectId);
                }
                else if (objType == typeof(Output))
                {
                    obj.OnOffObject = Singleton.GetById(obj.OnOffObjectId);
                }
            }
        }

        public Output CreateNewOne(string name, int number)
        {
            var newOutput = new Output
            {
                Name = name,
                OutputNumber = (byte)number,
                OutputType = 0,
                SettingsDelayToOn = 0,
                SettingsDelayToOff = 0
            };
            return newOutput;
        }

        public Output GetByOutputNumber(DCU dcu, byte number)
        {
            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            var oneFilterSetting = new FilterSettings(Output.COLUMNDCU, dcu, ComparerModes.EQUALL);
            filterSettings.Add(oneFilterSetting);
            oneFilterSetting = new FilterSettings(Output.COLUMNOUTPUTNUMBER, number, ComparerModes.EQUALL);
            filterSettings.Add(oneFilterSetting);

            var ret = SelectByCriteria(filterSettings);

            if (null == ret || ret.Count != 1)
                return null;
            return ret.ElementAt(0);
        }

        public OutputState GetActualStates(Output output)
        {
            return CCUConfigurationHandler.Singleton.GetOutputState(output);
        }

        public OutputState GetActualStatesByGuid(Guid idOutput)
        {
            var output = GetById(idOutput);
            if (output != null)
            {
                return CCUConfigurationHandler.Singleton.GetOutputState(output);
            }
            return OutputState.Unknown;
        }

        public OutputState GetRealStates(Output output)
        {
            return CCUConfigurationHandler.Singleton.GetOutputRealState(output);
        }

        public OutputState GetRealStatesByGuid(Guid idOutput)
        {
            var output = GetById(idOutput);
            if (output != null)
            {
                return CCUConfigurationHandler.Singleton.GetOutputRealState(output);
            }
            return OutputState.Unknown;
        }

        public bool IsActivated(Output output)
        {
            return CCUConfigurationHandler.Singleton.OutputIsActivated(output);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var sOutput = GetById(idObj);
                if (sOutput == null)
                    return null;

                var result = new List<AOrmObject>();
                if (sOutput.CCU != null)
                {
                    var ccu = CCUs.Singleton.GetById(sOutput.CCU.IdCCU);
                    result.Add(ccu);
                }
                if (sOutput.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetById(sOutput.DCU.IdDCU);
                    result.Add(dcu);
                }

                var de = DoorEnvironments.Singleton.GetOutputDoorEnvironment(sOutput.IdOutput);
                if (de != null)
                    result.Add(de);

                var onOffInpput = Inputs.Singleton.UsedLikeOnOffObject(sOutput);
                if (onOffInpput != null && onOffInpput.Count > 0)
                {
                    foreach (var input in onOffInpput)
                    {
                        var outInput = Inputs.Singleton.GetById(input.IdInput);
                        result.Add(outInput);
                    }
                }

                var onOffOutput = Singleton.UsedLikeOnOffObject(sOutput);
                if (onOffOutput != null && onOffOutput.Count > 0)
                {
                    foreach (var output in onOffOutput)
                    {
                        var outOutput = Singleton.GetById(output.IdOutput);
                        result.Add(outOutput);
                    }
                }

                var onOffCardReader = CardReaders.Singleton.UsedLikeOnOffObject(sOutput);
                if (onOffCardReader != null && onOffCardReader.Count > 0)
                {
                    foreach (var cardReader in onOffCardReader)
                    {
                        var outCardReader = CardReaders.Singleton.GetById(cardReader.IdCardReader);
                        result.Add(outCardReader);
                    }
                }

                var listAlarmArea = AlarmAreas.Singleton.UsedOutputInAlarmAreas(sOutput);
                if (listAlarmArea != null)
                {
                    foreach (var alarmArea in listAlarmArea)
                    {
                        var outAlarmArea = AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea);
                        result.Add(outAlarmArea);
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

        public ICollection<Output> UsedLikeOnOffObject(AOnOffObject onOffObj)
        {
            try
            {
                var type = Assembly.CreateQualifiedName(onOffObj.GetType().Assembly.GetName().Name, onOffObj.GetType().FullName);
                var idOnOffObj = onOffObj.GetId() as Guid?;

                return SelectLinq<Output>(output => output.OnOffObjectId == idOnOffObj &&
                    output.OnOffObjectType == type);
            }
            catch
            {
                return null;
            }
        }

        public ICollection<Output> GetOutputsForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null) return null;
                return SelectLinq<Output>(output => output.AlarmPresentationGroup == pg);
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
            ICollection<Output> linqResult;

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
                        ? SelectLinq<Output>(o => o.Name.IndexOf(name) >= 0) :
                        SelectLinq<Output>(o => o.Name.IndexOf(name) >= 0 || o.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(output => output.CCU).ThenBy(output => output.DCU).ThenBy(output => output.OutputNumber).ToList();
                IList<Output> listOutputs = linqResult.ToList();
                ArrayList.Adapter((IList)listOutputs).Sort();
                foreach (var dp in listOutputs)
                {
                    resultList.Add(GetObjectById(dp.IdOutput));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Output> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<Output>(o => o.Name.IndexOf(name) >= 0 || o.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<Output> linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Output>(o => o.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(IEnumerable<Output> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(output => output.CCU).ThenBy(output => output.DCU).ThenBy(output => output.OutputNumber).ToList();
                IList<Output> listOutputs = linqResult.ToList();
                ArrayList.Adapter((IList)listOutputs).Sort();
                foreach (var output in listOutputs)
                {
                    resultList.Add(GetById(output.IdOutput));
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<OutputShort> ShortSelectByCriteria(ICollection<FilterSettings> filterSettings, out Exception error)
        {
            var listOutput = SelectByCriteria(filterSettings, out error);
            ICollection<OutputShort> result = new List<OutputShort>();
            if (listOutput != null)
            {
                foreach (var output in listOutput)
                {
                    var outputShort = new OutputShort(output)
                    {
                        State = GetActualStates(output),
                        RealState = GetRealStates(output)
                    };
                    result.Add(outputShort);
                }
            }
            return result.OrderBy(output => output.FullName).ToList();
        }

        public ICollection<OutputShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings)
        {
            var listOutput = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            ICollection<OutputShort> result = new List<OutputShort>();
            if (listOutput != null)
            {
                foreach (var output in listOutput)
                {
                    var outputShort = new OutputShort(output)
                    {
                        State = GetActualStates(output),
                        RealState = GetRealStates(output)
                    };
                    result.Add(outputShort);
                }
            }
            return result.OrderBy(output => output.FullName).ToList();
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listOutput = List(out error);
            IList<IModifyObject> listOutputModifyObj = null;
            if (listOutput != null)
            {
                listOutputModifyObj = new List<IModifyObject>();
                foreach (var output in listOutput)
                {
                    listOutputModifyObj.Add(new OutputModifyObj(output));
                }
                listOutputModifyObj = listOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }
            return listOutputModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsFromCCU(Guid idCCU, out Exception error)
        {
            var listOutput = List(out error);
            IList<IModifyObject> listOutputModifyObj = null;
            if (listOutput != null)
            {
                listOutputModifyObj = new List<IModifyObject>();
                foreach (var output in listOutput)
                {
                    var outputCCUGuid = Guid.Empty;

                    if (output.DCU != null)
                    {
                        if (output.DCU.CCU != null)
                        {
                            outputCCUGuid = output.DCU.CCU.IdCCU;
                        }
                    }
                    else if (output.CCU != null)
                    {
                        outputCCUGuid = output.CCU.IdCCU;
                    }

                    if (outputCCUGuid == idCCU)
                    {
                        listOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                }
                listOutputModifyObj = listOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }

            return listOutputModifyObj;
        }

        /// <summary>
        /// Return outputs from CCU that are not used in the door environments
        /// </summary>
        /// <param name="idCCU"></param>
        /// <param name="idAlarmArea"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public IList<IModifyObject> ListModifyObjectsFromCCUNotUsedInDoorEnvironmentsAnaAlarmAreas(Guid idCCU, Guid idAlarmArea, out Exception error)
        {
            error = null;
            ICollection<Output> listOutput;

            if (idCCU != Guid.Empty)
            {
                listOutput = SelectLinq<Output>(output => (output.CCU != null && output.CCU.IdCCU == idCCU) ||
                    (output.DCU != null && output.DCU.CCU != null && output.DCU.CCU.IdCCU == idCCU));
            }
            else
            {
                listOutput = List(out error);
            }

            IList<IModifyObject> listOutputModifyObj = null;
            if (listOutput != null)
            {
                listOutputModifyObj = new List<IModifyObject>();
                foreach (var output in listOutput)
                {
                    if (output != null && !DoorEnvironments.Singleton.IsOutputInDoorEnvironments(output) &&
                        !AlarmAreas.Singleton.IsOutputInAlarmAreas(output, idAlarmArea))
                    {
                        listOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                }

                listOutputModifyObj = listOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }

            return listOutputModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsForAlarmAreaActivationObject(Guid idAlarmArea, out Exception error)
        {
            var ccu = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(idAlarmArea);
            if (ccu == null)
                return ListModifyObjects(out error);
            var onlyFromAlarmAreaCCU = true;
            var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
            var alarmAreaFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU);
            Version alarmAreaFW = null;
            if (!string.IsNullOrEmpty(alarmAreaFirmwareVersion))
            {
                try
                {
                    alarmAreaFW = new Version(alarmAreaFirmwareVersion.Split(' ')[0]);
                }
                catch { }
            }
            if (alarmAreaFW != null && alarmAreaFW >= minimalFW)
            {
                onlyFromAlarmAreaCCU = false;
            }

            var listOutput = List(out error);
            var listOutputModifyObj = new List<IModifyObject>();
            if (listOutput != null)
            {
                foreach (var output in listOutput)
                {
                    if (onlyFromAlarmAreaCCU)
                    {
                        var actOutputIdCCU = GetParentCCU(output.IdOutput);
                        if (actOutputIdCCU == ccu.IdCCU)
                            listOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                    else
                    {
                        var actOutputIdCCU = GetParentCCU(output.IdOutput);
                        var firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(actOutputIdCCU);
                        Version currentFW = null;
                        if (!string.IsNullOrEmpty(firmwareVersion))
                        {
                            try
                            {
                                currentFW = new Version(firmwareVersion.Split(' ')[0]);
                            }
                            catch { }
                        }

                        if (currentFW != null && currentFW >= minimalFW)
                            listOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                }
                listOutputModifyObj = listOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }
            return listOutputModifyObj;
        }

        public IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listOutput = SelectByCriteria(filterSettings, out error);
            var listOutputModifyObj = new List<IModifyObject>();
            if (listOutput != null)
            {
                foreach (var output in listOutput)
                {
                    listOutputModifyObj.Add(new OutputModifyObj(output));
                }
                listOutputModifyObj = listOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }
            return listOutputModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsForOtputControlByObject(Guid idOutput, IList<FilterSettings> filterSettings, out Exception error)
        {
            var onlyFromOutputCCU = true;
            var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
            var outputCCUGuid = Singleton.GetParentCCU(idOutput);
            var outputFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(outputCCUGuid);
            Version outputFW = null;
            if (!string.IsNullOrEmpty(outputFirmwareVersion))
            {
                try
                {
                    outputFW = new Version(outputFirmwareVersion.Split(' ')[0]);
                }
                catch { }
            }
            if (outputFW != null && outputFW >= minimalFW)
            {
                onlyFromOutputCCU = false;
            }

            var listOutput = SelectByCriteria(filterSettings, out error);
            var lisOutputModifyObj = new List<IModifyObject>();
            if (listOutput != null)
            {
                foreach (var output in listOutput)
                {
                    if (onlyFromOutputCCU)
                    {
                        var actOutputIdCCU = GetParentCCU(output.IdOutput);
                        if (actOutputIdCCU == outputCCUGuid)
                            lisOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                    else
                    {
                        var actOutputIdCCU = GetParentCCU(output.IdOutput);
                        var firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(actOutputIdCCU);
                        Version currentFW = null;
                        if (!string.IsNullOrEmpty(firmwareVersion))
                        {
                            try
                            {
                                currentFW = new Version(firmwareVersion.Split(' ')[0]);
                            }
                            catch { }
                        }

                        if (currentFW != null && currentFW >= minimalFW)
                            lisOutputModifyObj.Add(new OutputModifyObj(output));
                    }
                }
                lisOutputModifyObj = lisOutputModifyObj.OrderBy(output => output.ToString()).ToList();
            }
            return lisOutputModifyObj;
        }

        public bool OutputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(Guid idOutput, Guid idAddindOutput)
        {
            if (GetParentCCU(idOutput) != GetParentCCU(idAddindOutput))
            {
                var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
                var outputCCUGuid = Singleton.GetParentCCU(idOutput);
                var outputFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(outputCCUGuid);
                Version outputFW = null;
                if (!string.IsNullOrEmpty(outputFirmwareVersion))
                {
                    try
                    {
                        outputFW = new Version(outputFirmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (outputFW == null || outputFW < minimalFW)
                {
                    return false;
                }

                outputCCUGuid = Singleton.GetParentCCU(idAddindOutput);
                outputFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(outputCCUGuid);
                outputFW = null;
                if (!string.IsNullOrEmpty(outputFirmwareVersion))
                {
                    try
                    {
                        outputFW = new Version(outputFirmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (outputFW == null || outputFW < minimalFW)
                {
                    return false;
                }
            }

            return true;
        }

        public bool OutputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(Guid idAlarmArea, Guid idAddindOutput)
        {

            var ccu = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(idAlarmArea);
            if (ccu == null)
                return true;
            if (ccu.IdCCU != GetParentCCU(idAddindOutput))
            {
                var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
                var alarmAreaFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU);
                Version alarmAreaFW = null;
                if (!string.IsNullOrEmpty(alarmAreaFirmwareVersion))
                {
                    try
                    {
                        alarmAreaFW = new Version(alarmAreaFirmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (alarmAreaFW == null || alarmAreaFW < minimalFW)
                {
                    return false;
                }

                var outputCCUGuid = Singleton.GetParentCCU(idAddindOutput);
                var outputFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(outputCCUGuid);
                Version outputFW = null;
                if (!string.IsNullOrEmpty(outputFirmwareVersion))
                {
                    try
                    {
                        outputFW = new Version(outputFirmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (outputFW == null || outputFW < minimalFW)
                {
                    return false;
                }
            }

            return true;
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idOutput)
        {
            var output = GetById(idOutput);
            if (ccus != null && output != null)
            {
                if (output.CCU != null)
                {
                    CCUs.Singleton.GetParentCCU(ccus, output.CCU.IdCCU);
                }
                else if (output.DCU != null)
                {
                    DCUs.Singleton.GetParentCCU(ccus, output.DCU.IdDCU);
                }
            }
        }

        public Guid GetParentCCU(Guid idOutput)
        {
            var output = GetById(idOutput);
            if (output != null)
            {
                if (output.CCU != null)
                {
                    return output.CCU.IdCCU;
                }
                if (output.DCU != null)
                {
                    return output.DCU.CCU.IdCCU;
                }
            }

            return Guid.Empty;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idOutput)
        {
            var objects = new List<AOrmObject>();

            var output = GetById(idOutput);
            if (output != null)
            {
                ReadOnOffObject(output);
                if (output.OnOffObject != null
                    && DataReplicationManager.OnOffObjectShouldBeStoredOnCcu(
                        guidCCU,
                        output.OnOffObject))
                {
                    objects.Add(output.OnOffObject);
                }
            }

            return objects;
        }

        public bool ExistOutputWithIndex(byte index, DCU dcu)
        {
            var selectOutput = SelectLinq<Output>(output => output.DCU == dcu);
            if (selectOutput != null)
            {
                foreach (var output in selectOutput)
                {
                    if (output.OutputNumber == index)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ExistCcuOutputWithIndex(byte index, CCU ccu)
        {
            var selectOutput = SelectLinq<Output>(input => input.CCU == ccu);
            if (selectOutput != null)
            {
                foreach (var output in selectOutput)
                {
                    if (output.OutputNumber == index)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void RunOutputEditChanged(ARemotingCallbackHandler remoteHandler, object[] objOutput)
        {
            if (objOutput == null || objOutput.Length != 2) return;

            if (remoteHandler is OutputEditChangedHandler)
                (remoteHandler as OutputEditChangedHandler).RunEvent((Guid)objOutput[0], (Guid)objOutput[1]);
        }

        public IList<Output> FilterOutputsFromActivators(bool allOutputs, Guid ccuGuid, bool allowDoorEnvironment, Guid doorEnvironmentGuid)
        {
            List<Output> result;
            DoorEnvironment doorEnvironment = null;

            if (allOutputs)
                result = Singleton.List().ToList();
            else
            {
                if (ccuGuid == Guid.Empty)
                    return null;
                CCU ccu = CCUs.Singleton.GetById(ccuGuid);
                if (ccu == null)
                    return null;

                result = GetOutputsFromCCUAndItsDCUs(ccu);
            }

            if (allowDoorEnvironment)
            {
                if (doorEnvironmentGuid == Guid.Empty)
                    return null;
                doorEnvironment = DoorEnvironments.Singleton.GetById(doorEnvironmentGuid);
                if (doorEnvironment == null)
                    return null;
            }

            return FilterOutputsFromActivators(result, doorEnvironment, Guid.Empty);
        }

        public List<Output> FilterOutputsFromActivators(List<Output> outputs, DoorEnvironment allowedDoorEnvironment, Guid editingAlarmAreaId)
        {
            if (outputs == null || outputs.Count == 0)
                return null;

            var guidOutputs = new List<Guid>();
            var guidRemoveOutputs = new List<Guid>();

            foreach (var output in outputs)
            {
                guidOutputs.Add(output.IdOutput);
            }

            var dEnvironments = List<DoorEnvironment>();

            foreach (var environment in dEnvironments)
            {
                if (allowedDoorEnvironment != null && allowedDoorEnvironment.Compare(environment))
                    continue;

                if (environment.ActuatorsElectricStrike != null && guidOutputs.Contains(environment.ActuatorsElectricStrike.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsElectricStrike.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsElectricStrike.IdOutput);
                }

                if (environment.ActuatorsElectricStrikeOpposite != null && guidOutputs.Contains(environment.ActuatorsElectricStrikeOpposite.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsElectricStrikeOpposite.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsElectricStrikeOpposite.IdOutput);
                }

                if (environment.ActuatorsExtraElectricStrikeOpposite != null && guidOutputs.Contains(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput);
                }

                if (environment.ActuatorsExtraElectricStrike != null && guidOutputs.Contains(environment.ActuatorsExtraElectricStrike.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsExtraElectricStrike.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsExtraElectricStrike.IdOutput);
                }


                if (environment.ActuatorsBypassAlarm != null && guidOutputs.Contains(environment.ActuatorsBypassAlarm.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsBypassAlarm.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsBypassAlarm.IdOutput);
                }
            }

            var removeOutputs = new List<Output>();

            foreach (var output in outputs)
            {
                if (guidRemoveOutputs.Contains(output.IdOutput) ||
                    AlarmAreas.Singleton.IsOutputInAlarmAreasAsEISSetUnsetOutput(output, editingAlarmAreaId) ||
                    MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(output.IdOutput))
                {
                    removeOutputs.Add(output);
                }
            }

            foreach (var output in removeOutputs)
            {
                outputs.Remove(output);
            }

            return outputs;
        }

        public List<Output> FilterOutputsFromAllUsed(List<Output> outputs, DoorEnvironment allowedDoorEnvironment)
        {
            if (outputs == null || outputs.Count == 0)
                return null;

            var guidOutputs = new List<Guid>();
            var guidRemoveOutputs = new List<Guid>();

            foreach (var output in outputs)
            {
                guidOutputs.Add(output.IdOutput);
            }

            var dEnvironments = List<DoorEnvironment>();

            foreach (var environment in dEnvironments)
            {
                if (allowedDoorEnvironment != null && allowedDoorEnvironment.Compare(environment))
                    continue;

                if (environment.ActuatorsElectricStrike != null && guidOutputs.Contains(environment.ActuatorsElectricStrike.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsElectricStrike.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsElectricStrike.IdOutput);
                }

                if (environment.ActuatorsElectricStrikeOpposite != null && guidOutputs.Contains(environment.ActuatorsElectricStrikeOpposite.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsElectricStrikeOpposite.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsElectricStrikeOpposite.IdOutput);
                }

                if (environment.ActuatorsExtraElectricStrikeOpposite != null && guidOutputs.Contains(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsExtraElectricStrikeOpposite.IdOutput);
                }

                if (environment.ActuatorsExtraElectricStrike != null && guidOutputs.Contains(environment.ActuatorsExtraElectricStrike.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsExtraElectricStrike.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsExtraElectricStrike.IdOutput);
                }


                if (environment.ActuatorsBypassAlarm != null && guidOutputs.Contains(environment.ActuatorsBypassAlarm.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.ActuatorsBypassAlarm.IdOutput))
                        guidRemoveOutputs.Add(environment.ActuatorsBypassAlarm.IdOutput);
                }

                if (environment.DoorAjarOutput != null && guidOutputs.Contains(environment.DoorAjarOutput.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.DoorAjarOutput.IdOutput))
                        guidRemoveOutputs.Add(environment.DoorAjarOutput.IdOutput);
                }

                if (environment.IntrusionOutput != null && guidOutputs.Contains(environment.IntrusionOutput.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.IntrusionOutput.IdOutput))
                        guidRemoveOutputs.Add(environment.IntrusionOutput.IdOutput);
                }

                if (environment.SabotageOutput != null && guidOutputs.Contains(environment.SabotageOutput.IdOutput))
                {
                    if (!guidRemoveOutputs.Contains(environment.SabotageOutput.IdOutput))
                        guidRemoveOutputs.Add(environment.SabotageOutput.IdOutput);
                }
            }

            var removeOutputs = new List<Output>();
            foreach (var output in outputs)
            {
                if (guidRemoveOutputs.Contains(output.IdOutput) ||
                    AlarmAreas.Singleton.IsOutputInAlarmAreas(output, Guid.Empty) ||
                    output.ControlType == (byte)OutputControl.controledByObject ||
                    MultiDoorElements.Singleton.IsOutputUsedOnlyInDoorElement(output.IdOutput))
                {
                    removeOutputs.Add(output);
                }
            }

            foreach (var output in removeOutputs)
            {
                outputs.Remove(output);
            }

            return outputs;
        }

        public List<Output> GetOutputsFromCCUAndItsDCUs(CCU ccu)
        {
            if (ccu == null)
                return null;

            var result = new List<Output>();
            if (ccu.Outputs != null && ccu.Outputs.Count > 0)
                result.AddRange(ccu.Outputs);
            if (ccu.DCUs != null && ccu.DCUs.Count > 0)
            {
                foreach (var dcu in ccu.DCUs)
                {
                    if (dcu.Outputs != null && dcu.Outputs.Count > 0)
                        result.AddRange(dcu.Outputs);
                }
            }

            return result;
        }

        public override bool CanCreateObject()
        {
            return false;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Output; }
        }
    }
}
