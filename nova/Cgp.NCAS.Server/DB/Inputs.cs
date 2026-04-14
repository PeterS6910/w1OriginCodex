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
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.DB;
using Contal.Cgp.NCAS.Definitions;

using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class Inputs :
        ANcasBaseOrmTableWithAlarmInstruction<Inputs, Input>, 
        IInputs
    {
        private Inputs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<Input>())
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var input = ormObject as Input;

            if (input == null)
                return null;

            var dcu = input.DCU;

            return
                dcu != null
                    ? (AOrmObject)dcu
                    : input.CCU;
        }

        protected override IModifyObject CreateModifyObject(Input ormbObject)
        {
            return new InputModifyObj(ormbObject);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(Input input)
        {
            ReadOnOffObject(input);
            yield return input.OnOffObject;

            yield return input.AlarmPresentationGroup;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.INPUTS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.InputsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.INPUTS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.InputsInsertDeletePerform), login);
        }

        public override void CUDSpecial(Input input, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        input.GetId(),
                        input.GetObjectType()));
            }
            else if (input != null)
            {
                if (objectDatabaseAction == ObjectDatabaseAction.Insert
                    && input.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetObjectForEdit(input.DCU.IdDCU);
                    DCUs.Singleton.Update(dcu);
                    DCUs.Singleton.EditEnd(dcu);

                    return;
                }

                DataReplicationManager.Singleton.SendModifiedObjectToCcus(input);
            }
        }

        protected override IEnumerable<Input> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<Input>(
                input =>
                    input.LocalAlarmInstruction != null
                    && input.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterUpdate(Input newInput, Input oldInputBeforeUpdate)
        {
            if (newInput == null)
            {
                return;
            }

            base.AfterUpdate(
                newInput,
                oldInputBeforeUpdate);

            var idParentObject = Guid.Empty;
            if (newInput.CCU != null)
            {
                idParentObject = newInput.CCU.IdCCU;
            }
            else if (newInput.DCU != null)
            {
                idParentObject = newInput.DCU.IdDCU;
            }

            object[] objInput = {idParentObject, newInput.IdInput};
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunInputEditChanged,
                DelegateSequenceBlockingMode.Asynchronous, false, objInput);

            if (oldInputBeforeUpdate == null)
            {
                return;
            }

            if (newInput.AlarmPresentationGroup != null &&
                !newInput.AlarmPresentationGroup.IdGroup.Equals(
                    oldInputBeforeUpdate.AlarmPresentationGroup != null
                        ? oldInputBeforeUpdate.AlarmPresentationGroup.IdGroup
                        : Guid.Empty))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Input_Alarm, ObjectType.Input, newInput.IdInput);
            }

            if (newInput.TamperAlarmPresentationGroup != null &&
                !newInput.TamperAlarmPresentationGroup.IdGroup.Equals(
                    oldInputBeforeUpdate.TamperAlarmPresentationGroup != null
                        ? oldInputBeforeUpdate.TamperAlarmPresentationGroup.IdGroup
                        : Guid.Empty))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Input_Tamper, ObjectType.Input, newInput.IdInput);
            }
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(Input input)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    input.IdInput,
                    ObjectType.Input),
                1);
        }

        public override void AfterDelete(Input input)
        {
            base.AfterDelete(input);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(input));
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(Input.COLUMNCCU, false));
            c.AddOrder(new Order(Input.COLUMNDCUSDCU, false));
            c.AddOrder(new Order(Input.COLUMNINPUTNUMBER, true));
        }

        protected override void LoadObjectsInRelationship(Input obj)
        {
            if (obj.AlarmPresentationGroup != null)
            {
                obj.AlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmPresentationGroup.IdGroup);
            }

            if (obj.TamperAlarmPresentationGroup != null)
            {
                obj.TamperAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.TamperAlarmPresentationGroup.IdGroup);
            }
        }

        protected override void LoadObjectsInRelationshipGetById(Input obj)
        {
            if (obj == null)
                return;

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

            if (obj.AAInputs != null)
            {
                var list = new LinkedList<AAInput>(
                    obj.AAInputs.Select(
                        aaInput =>
                            AAInputs.Singleton.GetById(aaInput.IdAAInput)));

                obj.AAInputs.Clear();
                foreach (var aaInput in list)
                    obj.AAInputs.Add(aaInput);
            }
        }

        protected override void SaveOnOffObjects(Input ormObject)
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

        protected override void ReloadBeforeUpdateDelete(Input ormObject)
        {
            // Reload AAInputs for input before updating or deleting
            if (ormObject != null)
            {
                var input = GetById(ormObject.IdInput);
                if (input != null)
                {
                    if (ormObject.AAInputs == null)
                    {
                        ormObject.AAInputs = input.AAInputs;
                    }
                    else
                    {
                        ormObject.AAInputs.Clear();
                        if (input.AAInputs != null && input.AAInputs.Count > 0)
                        {
                            foreach (var aaInput in input.AAInputs)
                            {
                                ormObject.AAInputs.Add(aaInput);
                            }
                        }
                    }
                }
            }
        }

        protected override void ReadOnOffObject(Input obj)
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
                    obj.OnOffObject = Singleton.GetById(obj.OnOffObjectId);
                }
                else if (objType == typeof(Output))
                {
                    obj.OnOffObject = Outputs.Singleton.GetById(obj.OnOffObjectId);
                }
            }
        }

        public Input CreateNewOne(string name, string nickName, int inputNumber)
        {
            var newInput = new Input
            {
                Name = name,
                NickName = nickName,
                InputType = (byte) InputType.DI,
                Inverted = false,
                HighPriority = false,
                OffACK = false,
                DelayToOn = 0,
                DelayToOff = 0,
                TamperDelayToOn = 0,
                InputNumber = (byte) inputNumber
            };
            return newInput;
        }

        public InputState GetActualStates(Input input)
        {
            return CCUConfigurationHandler.Singleton.GetCurrentInputState(input);
        }

        public InputState GetActualStatesByGuid(Guid idInput)
        {
            var input = GetById(idInput);
            if (input != null)
                return CCUConfigurationHandler.Singleton.GetCurrentInputState(input);
            return InputState.Unknown;
        }

        public bool IsActivated(Input input)
        {
            return CCUConfigurationHandler.Singleton.InputIsActivated(input);
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            //ccu
            //dcu
            //doors environment
            //AAInput

            try
            {
                var input = GetById(idObj);
                if (input == null)
                    return null;

                var result = new List<AOrmObject>();
                if (input.CCU != null)
                    result.Add(CCUs.Singleton.GetById(input.CCU.IdCCU));

                if (input.DCU != null)
                    result.Add(DCUs.Singleton.GetById(input.DCU.IdDCU));

                var de = DoorEnvironments.Singleton.GetInputDoorEnvironment(input.IdInput);
                if (de != null)
                    result.Add(de);

                var onOffInpputs = UsedLikeOnOffObject(input);
                if (onOffInpputs != null)
                    foreach (var onOffInput in onOffInpputs)
                        result.Add(GetById(onOffInput.IdInput));

                var onOffOutput = Outputs.Singleton.UsedLikeOnOffObject(input);
                if (onOffOutput != null)
                    foreach (var output in onOffOutput)
                        result.Add(Outputs.Singleton.GetById(output.IdOutput));

                var onOffCardReader = CardReaders.Singleton.UsedLikeOnOffObject(input);

                if (onOffCardReader != null)
                    foreach (var cardReader in onOffCardReader)
                        result.Add(CardReaders.Singleton.GetById(cardReader.IdCardReader));

                var listAlarmAreaAAInputs = AAInputs.Singleton.GetAlarmAreaByInput(input);
                if (listAlarmAreaAAInputs != null)
                    foreach (var alarmArea in listAlarmAreaAAInputs)
                        result.Add(AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea));

                var listAlarmArea = AlarmAreas.Singleton.UsedInputInAlarmAreas(input);
                if (listAlarmArea != null)
                    foreach (var alarmArea in listAlarmArea)
                        result.Add(AlarmAreas.Singleton.GetById(alarmArea.IdAlarmArea));

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

        public ICollection<Input> UsedLikeOnOffObject(AOnOffObject onOffObj)
        {
            try
            {
                var type = Assembly.CreateQualifiedName(onOffObj.GetType().Assembly.GetName().Name, onOffObj.GetType().FullName);
                var idOnOffObj = onOffObj.GetId() as Guid?;

                return SelectLinq<Input>(input => input.OnOffObjectId == idOnOffObj &&
                    input.OnOffObjectType == type);
            }
            catch
            {
                return null;
            }
        }

        public ICollection<Input> GetInputsForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null) return null;
                return SelectLinq<Input>(input => input.AlarmPresentationGroup == pg ||
                    input.TamperAlarmPresentationGroup == pg);
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
            ICollection<Input> linqResult;

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
                        ? SelectLinq<Input>(i => i.Name.IndexOf(name) >= 0)
                        : SelectLinq<Input>(
                            i => i.Name.IndexOf(name) >= 0 || i.NickName.IndexOf(name) >= 0
                                 || i.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(input => input.CCU).ThenBy(input => input.DCU).ThenBy(input => input.InputNumber).ToList();
                IList<Input> listInputs = linqResult.ToList();
                ArrayList.Adapter((IList)listInputs).Sort();
                foreach (var dp in listInputs)
                {
                    resultList.Add(GetById(dp.IdInput));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<Input> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<Input>(
                        i => i.FullTextSearchString.ToLower().Contains(name.ToLower())
                             || i.NickName.ToLower().Contains(name.ToLower())
                             || i.Description.ToLower().Contains(name.ToLower()));
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<Input>(
                        input => input.FullTextSearchString.ToLower().Contains(name.ToLower()) ||
                                 input.NickName.ToLower().Contains(name.ToLower())).ToList();

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(IEnumerable<Input> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                //linqResult = linqResult.OrderBy(input => input.CCU).ThenBy(input => input.DCU).ThenBy(input => input.InputNumber).ToList();
                IList<Input> listInputs = linqResult.ToList();
                ArrayList.Adapter((IList)listInputs).Sort();
                foreach (var dp in listInputs)
                {
                    resultList.Add(GetById(dp.IdInput));
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<InputShort> ShortSelectByCriteria(ICollection<FilterSettings> filterSettings, out Exception error)
        {
            var listInput = SelectByCriteria(filterSettings, out error);
            ICollection<InputShort> result = new List<InputShort>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    var shortInput = new InputShort(input)
                    {
                        State = GetActualStates(input)
                    };
                    result.Add(shortInput);
                }
            }

            return result.OrderBy(input => input.FullName).ToList();
        }

        public ICollection<InputShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings)
        {
            ICollection<Input> listInput = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            ICollection<InputShort> result = new List<InputShort>();
            if (listInput != null)
            {
                foreach (Input input in listInput)
                {
                    InputShort shortInput = new InputShort(input)
                    {
                        State = GetActualStates(input)
                    };
                    result.Add(shortInput);
                }
            }

            return result.OrderBy(input => input.FullName).ToList();
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listInput = List(out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    listInputModifyObj.Add(new InputModifyObj(input));
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsFromCCU(Guid idCCU, out Exception error)
        {
            var listInput = List(out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    var inputCCUGuid = Guid.Empty;

                    if (input.DCU != null)
                    {
                        if (input.DCU.CCU != null)
                        {
                            inputCCUGuid = input.DCU.CCU.IdCCU;
                        }
                    }
                    else if (input.CCU != null)
                    {
                        inputCCUGuid = input.CCU.IdCCU;
                    }

                    if (inputCCUGuid == idCCU)
                    {
                        listInputModifyObj.Add(new InputModifyObj(input));
                    }
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
        }

        /// <summary>
        /// Return inputs from CCU that are not used in the door environments
        /// </summary>
        /// <param name="idCCU"></param>
        /// <param name="idAlarmArea"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public IList<IModifyObject> ListModifyObjectsFromCCUNotUsedInDoorEnvironments(Guid idCCU, Guid idAlarmArea, out Exception error)
        {
            error = null;
            ICollection<Input> listInput;

            if (idCCU != Guid.Empty)
            {
                listInput = SelectLinq<Input>(input => (input.CCU != null && input.CCU.IdCCU == idCCU) ||
                    (input.DCU != null && input.DCU.CCU != null && input.DCU.CCU.IdCCU == idCCU));
            }
            else
            {
                listInput = List(out error);
            }

            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    if (input != null && !DoorEnvironments.Singleton.IsInputInDoorEnvironments(input) &&
                        !AlarmAreas.Singleton.IsInputInAlarmAreas(input, idAlarmArea))
                    {
                        listInputModifyObj.Add(new InputModifyObj(input));
                    }
                }

                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }

            return listInputModifyObj;
        }

        public IList<IModifyObject> ListModifyObjectsForOtputControlByObject(Guid idOutput, out Exception error)
        {
            var onlyFromOutputCCU = true;
            var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
            var outputCCUGuid = Outputs.Singleton.GetParentCCU(idOutput);
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

            var listInput = List(out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    if (onlyFromOutputCCU)
                    {
                        var actInputIdCCU = GetParentCCU(input.IdInput);
                        if (actInputIdCCU == outputCCUGuid)
                            listInputModifyObj.Add(new InputModifyObj(input));
                    }
                    else
                    {
                        var actInputIdCCU = GetParentCCU(input.IdInput);
                        var firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(actInputIdCCU);
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
                            listInputModifyObj.Add(new InputModifyObj(input));
                    }
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
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

            var listInput = List(out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    if (onlyFromAlarmAreaCCU)
                    {
                        var actInputIdCCU = GetParentCCU(input.IdInput);
                        if (actInputIdCCU == ccu.IdCCU)
                            listInputModifyObj.Add(new InputModifyObj(input));
                    }
                    else
                    {
                        var actInputIdCCU = GetParentCCU(input.IdInput);
                        var firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(actInputIdCCU);
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
                            listInputModifyObj.Add(new InputModifyObj(input));
                    }
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
        }

        public bool InputFromTheSameCCUOrInterCCUComunicationEnabledOutputControlObject(Guid idOutput, Guid idAddinInput)
        {
            if (Outputs.Singleton.GetParentCCU(idOutput) != GetParentCCU(idAddinInput))
            {
                var minimalFW = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION;
                var outputCCUGuid = Outputs.Singleton.GetParentCCU(idOutput);
                var firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(outputCCUGuid);
                Version FW = null;
                if (!string.IsNullOrEmpty(firmwareVersion))
                {
                    try
                    {
                        FW = new Version(firmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (FW == null || FW < minimalFW)
                {
                    return false;
                }

                var inputCCUGuid = Singleton.GetParentCCU(idAddinInput);
                firmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(inputCCUGuid);
                FW = null;
                if (!string.IsNullOrEmpty(firmwareVersion))
                {
                    try
                    {
                        FW = new Version(firmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (FW == null || FW < minimalFW)
                {
                    return false;
                }
            }

            return true;
        }

        public bool InputFromTheSameCCUOrInterCCUComunicationEnabledAlarmAreaActivationObject(Guid idAlarmArea, Guid idAddindInput)
        {
            var ccu = AlarmAreas.Singleton.GetImplicitCCUForAlarmArea(idAlarmArea);
            if (ccu == null)
                return true;
            if (ccu.IdCCU != GetParentCCU(idAddindInput))
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
                if (alarmAreaFW == null || alarmAreaFW >= minimalFW)
                {
                    return false;
                }

                var inputCCUGuid = Singleton.GetParentCCU(idAddindInput);
                var inputFirmwareVersion = CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(inputCCUGuid);
                Version inputFW = null;
                if (!string.IsNullOrEmpty(inputFirmwareVersion))
                {
                    try
                    {
                        inputFW = new Version(inputFirmwareVersion.Split(' ')[0]);
                    }
                    catch { }
                }
                if (inputFW == null || inputFW < minimalFW)
                {
                    return false;
                }
            }

            return true;
        }

        public IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listInput = SelectByCriteria(filterSettings, out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    listInputModifyObj.Add(new InputModifyObj(input));
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
        }

        public IList<IModifyObject> ModifyObjectsSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error, Guid guidImplicitCCU)
        {
            if (guidImplicitCCU == Guid.Empty)
            {
                return ModifyObjectsSelectByCriteria(filterSettings, out error);
            }
            var listInput = SelectByCriteria(filterSettings, out error);
            var listInputModifyObj = new List<IModifyObject>();
            if (listInput != null)
            {
                foreach (var input in listInput)
                {
                    if (input != null)
                    {
                        var ccus = new List<Guid>();
                        GetParentCCU(ccus, input.IdInput);
                        if (ccus.Contains(guidImplicitCCU))
                            listInputModifyObj.Add(new InputModifyObj(input));
                    }
                }
                listInputModifyObj = listInputModifyObj.OrderBy(input => input.ToString()).ToList();
            }
            return listInputModifyObj;
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idInput)
        {
            var input = GetById(idInput);
            if (ccus != null && input != null)
            {
                if (input.CCU != null)
                {
                    CCUs.Singleton.GetParentCCU(ccus, input.CCU.IdCCU);
                }
                else if (input.DCU != null)
                {
                    DCUs.Singleton.GetParentCCU(ccus, input.DCU.IdDCU);
                }
            }
        }

        public Guid GetParentCCU(Guid idInput)
        {
            var input = GetById(idInput);
            if (input != null)
            {
                if (input.CCU != null)
                {
                    return input.CCU.IdCCU;
                }
                if (input.DCU != null)
                {
                    return input.DCU.CCU.IdCCU;
                }
            }

            return Guid.Empty;
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idInput)
        {
            var objects = new List<AOrmObject>();

            var input = GetById(idInput);
            if (input != null)
            {
                ReadOnOffObject(input);
                if (input.OnOffObject != null
                    && DataReplicationManager.OnOffObjectShouldBeStoredOnCcu(
                        guidCCU,
                        input.OnOffObject))
                {
                    objects.Add(input.OnOffObject);
                }
            }

            return objects;
        }

        public bool ExistInputWithIndex(byte index, DCU dcu)
        {
            //ICollection<Input> selectInput =  SelectLinq<Input>(input => input.InputNumber == index && 
            //    input.DCU == dcu);
            var selectInput = SelectLinq<Input>(input => input.DCU == dcu);
            if (selectInput != null)
            {
                foreach (var input in selectInput)
                {
                    if (input.InputNumber == index)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ExistCcuInputWithIndex(byte index, CCU ccu)
        {
            var selectInput = SelectLinq<Input>(input => input.CCU == ccu);
            if (selectInput != null)
            {
                foreach (var input in selectInput)
                {
                    if (input.InputNumber == index)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void RunInputEditChanged(ARemotingCallbackHandler remoteHandler, object[] objInput)
        {
            if (objInput == null || objInput.Length != 2) return;

            if (remoteHandler is InputEditChangedHandler)
                (remoteHandler as InputEditChangedHandler).RunEvent((Guid)objInput[0], (Guid)objInput[1]);
        }

        public override bool CanCreateObject()
        {
            return false;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.Input; }
        }

        public ICollection<Input> SelectByCriteriaFullInputs(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings)
        {
            var inputsFromDatabase = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            var result = new List<Input>();

            if (inputsFromDatabase == null)
                return result;

            result.AddRange(
                inputsFromDatabase
                    .Select(
                        input =>
                            GetObjectById(input.IdInput))
                    .Where(
                        fullInput =>
                            fullInput != null));

            return result.OrderBy(fullInput => fullInput.FullName).ToList();
        }
    }
}
