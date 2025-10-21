using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Alarms;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Threads;

using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.Server;
using RequiredLicenceProperties = Contal.Cgp.NCAS.Globals.RequiredLicenceProperties;
using System.Net.NetworkInformation;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class CCUs :
        ANcasBaseOrmTableWithAlarmInstruction<CCUs, CCU>, 
        ICCUs
    {
        private CCUs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<CCU>())
        {
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(CCU ccu)
        {
            var allCcuCardReadersId = new HashSet<Guid>();

            if (ccu.CardReaders != null)
                foreach (var cardReader in ccu.CardReaders)
                {
                    allCcuCardReadersId.Add(cardReader.IdCardReader);
                    yield return cardReader;
                }

            var dcus = ccu.DCUs;

            if (dcus != null)
                foreach (var dcu in dcus)
                {
                    if (dcu.CardReaders != null)
                    {
                        foreach (var cardReader in dcu.CardReaders)
                        {
                            allCcuCardReadersId.Add(cardReader.IdCardReader);
                        }
                    }

                    yield return dcu;
                }

            var doorEnvironments = ccu.DoorEnvironments;

            if (doorEnvironments != null)
                foreach (var doorEnvironment in doorEnvironments)
                    yield return doorEnvironment;

            var inputs = ccu.Inputs;

            if (inputs != null)
                foreach (var input in inputs)
                    yield return input;

            var outputs = ccu.Outputs;

            if (outputs != null)
                foreach (var output in outputs)
                    yield return output;

            if (ccu.CardReaders != null)
            {
                foreach (var multiDoor in MultiDoors.Singleton.GetMultiDoorsForCardReaders(allCcuCardReadersId))
                {
                    yield return multiDoor;
                }
            }

            if (ccu.AlarmCcuCatUnreachablePresentationGroup != null)
                yield return ccu.AlarmCcuCatUnreachablePresentationGroup;

            if (ccu.AlarmCcuTransferToArcTimedOutPresentationGroup != null)
                yield return ccu.AlarmCcuTransferToArcTimedOutPresentationGroup;

            if (ccu.AlarmTransmitter != null)
                yield return ccu.AlarmTransmitter;

            if (ccu.CcuAlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>();

                foreach (var ccuAlarmArc in ccu.CcuAlarmArcs)
                {
                    if (!addedAlarmArcIds.Add(ccuAlarmArc.IdAlarmArc))
                        continue;

                    yield return ccuAlarmArc.AlarmArc;
                }
            }
        }

        protected override IModifyObject CreateModifyObject(CCU ormbObject)
        {
            return new CcuModifyObj(ormbObject);
        }

        protected override bool AddCriteriaSpecial(ref ICriteria c, FilterSettings filterSetting)
        {
            if (filterSetting.Column == CCU.COLUMNNAME)
            {
                //when filtering by CCU name this code adds column ip address and column mac address to search
                c = c.Add(Restrictions.Or(Restrictions.Like(CCU.COLUMNNAME, filterSetting.Value as string, MatchMode.Anywhere),
                Restrictions.Or(Restrictions.Like(CCU.COLUMNIPADDRESS, filterSetting.Value as string,
                MatchMode.Anywhere), Restrictions.Like(CCU.COLUMNMACADDRESS, filterSetting.Value as string, MatchMode.Anywhere))));
                return true;
            }
            return false;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CCUS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CcusInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CCUS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CcusInsertDeletePerform), login);
        }

        public DateTime? GetCurrentCCUTime(Guid guid)
        {
            return CCUConfigurationHandler.Singleton.GetCurrentCCUTime(guid);
        }

        protected override IEnumerable<CCU> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<CCU>(
                ccu =>
                    ccu.LocalAlarmInstruction != null
                    && ccu.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterInsert(CCU ccu)
        {
            if (ccu == null)
                return;

            base.AfterInsert(ccu);

            NCASServer.Singleton.GetAlarmsQueue().RegisterAlarmsOwner(
                ccu.IdCCU,
                new CcuAlarmsOwner(ccu.IdCCU));

            if (ccu.CcuAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        ccu.IdCCU,
                        ObjectType.CCU), 
                    ccu.CcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(CCU ccu)
        {
            var alarmObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    ccu.IdCCU,
                    ObjectType.CCU),
                1);

            var alarmAreas = CCUConfigurationHandler.Singleton.GetAlarmAreasOfImplicitCcu(ccu.IdCCU);

            if (alarmAreas != null)
            {
                alarmObjects = alarmAreas.Aggregate(
                    alarmObjects,
                    (current, alarmArea) =>
                        current.Concat(AlarmAreas.Singleton.GetAlarmObjects(alarmArea as AlarmArea)));
            }

            if (ccu.DCUs != null)
            {
                alarmObjects = ccu.DCUs.Aggregate(
                    alarmObjects,
                    (current, dcu) =>
                        current.Concat(DCUs.Singleton.GetAlarmObjects(dcu)));
            }
            
            if (ccu.Inputs != null)
            {
                alarmObjects = ccu.Inputs.Aggregate(
                    alarmObjects,
                    (current, input) =>
                        current.Concat(Inputs.Singleton.GetAlarmObjects(input)));
            }

            if (ccu.Outputs != null)
            {
                alarmObjects = ccu.Outputs.Aggregate(
                    alarmObjects,
                    (current, output) =>
                        current.Concat(Outputs.Singleton.GetAlarmObjects(output)));
            }

            if (ccu.DoorEnvironments != null)
            {
                alarmObjects = ccu.DoorEnvironments.Aggregate(
                    alarmObjects,
                    (current, doorEnvironment) =>
                        current.Concat(DoorEnvironments.Singleton.GetAlarmObjects(doorEnvironment)));
            }

            if (ccu.CardReaders != null)
            {
                alarmObjects = ccu.CardReaders.Aggregate(
                    alarmObjects,
                    (current, cardReader) =>
                        current.Concat(CardReaders.Singleton.GetAlarmObjects(cardReader)));
            }

            return alarmObjects;
        }

        public override void AfterUpdate(CCU newCcu, CCU oldCcuBeforUpdate)
        {
            if (newCcu == null)
                return;

            base.AfterUpdate(
                newCcu,
                oldCcuBeforUpdate);

            if (newCcu.AlarmCcuCatUnreachablePresentationGroup != null &&
                !newCcu.AlarmCcuCatUnreachablePresentationGroup.Compare(
                    oldCcuBeforUpdate.AlarmCcuCatUnreachablePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(
                    AlarmType.Ccu_CatUnreachable,
                    ObjectType.CCU,
                    newCcu.IdCCU);
            }

            if (newCcu.AlarmCcuTransferToArcTimedOutPresentationGroup != null &&
                !newCcu.AlarmCcuTransferToArcTimedOutPresentationGroup.Compare(
                    oldCcuBeforUpdate.AlarmCcuTransferToArcTimedOutPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(
                    AlarmType.Ccu_TransferToArcTimedOut,
                    ObjectType.CCU,
                    newCcu.IdCCU);
            }

            if (newCcu.CcuAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        newCcu.IdCCU,
                        ObjectType.CCU),
                    newCcu.CcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        newCcu.IdCCU,
                        ObjectType.CCU));
        }

        public override void AfterDelete(CCU ccu)
        {
            if (ccu == null)
                return;

            base.AfterDelete(ccu);

            NCASServer.Singleton.GetAlarmsQueue().UnregisterExternalAlarmsOwner(
                ccu.IdCCU);

            CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                new IdAndObjectType(
                    ccu.IdCCU,
                    ObjectType.CCU));
        }

        public void ConfigureSpecificAlarmArcs()
        {
            var ccus = List();

            if (ccus == null)
                return;

            foreach (var ccu in ccus)
            {
                if (ccu.CcuAlarmArcs != null)
                    CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU),
                        ccu.CcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
            }
        }

        public override void CUDSpecial(CCU ccu, ObjectDatabaseAction objectDatabaseAction)
        {
            if (ccu != null)
            {
                CCUConfigurationHandler.Singleton.ConnectCCU(ccu.IdCCU, false);

                if (objectDatabaseAction == ObjectDatabaseAction.Delete)
                {
                    DataReplicationManager.Singleton.DeleteObjectFroCcus(
                        new IdAndObjectType(
                            ccu.GetId(),
                            ccu.GetObjectType()));
                }
                else
                {
                    DataReplicationManager.Singleton.SendModifiedObjectToCcus(ccu);
                    CCUConfigurationHandler.Singleton.StartStopSendingTimeFromServer(ccu.IdCCU);
                }
            }
        }

        public void GeneralNtpAddressAkn()
        {
            var listCcu = List();
            foreach (var ccu in listCcu)
            {
                if (ccu.InheritGeneralNtpSettings && ccu.IsConfigured)
                {
                    Update(ccu);
                }
            }
        }

        public bool ExistsPathOrFile(Guid ccuGuid, string fullName)
        {
            return 
                !string.IsNullOrEmpty(fullName) && 
                CCUConfigurationHandler.Singleton.ExistsPathOrFile(ccuGuid, fullName);
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(CCU.COLUMNNAME, true));
        }

        protected override void LoadObjectsInRelationship(CCU obj)
        {
            if (obj.DCUs != null)
            {
                IList<DCU> list = new List<DCU>();

                foreach (var dcu in obj.DCUs)
                {
                    list.Add(DCUs.Singleton.GetById(dcu.IdDCU));
                }

                obj.DCUs.Clear();
                foreach (var dcu in list)
                    obj.DCUs.Add(dcu);
            }

            if (obj.Inputs != null)
            {
                IList<Input> list = new List<Input>();
                foreach (var input in obj.Inputs)
                {
                    list.Add(Inputs.Singleton.GetById(input.IdInput));
                }
                list = list.OrderBy(input => input.InputNumber).ToList();

                obj.Inputs.Clear();
                foreach (var input in list)
                    obj.Inputs.Add(input);
            }

            if (obj.Outputs != null)
            {
                IList<Output> list = new List<Output>();
                foreach (var output in obj.Outputs)
                {
                    list.Add(Outputs.Singleton.GetById(output.IdOutput));
                }
                list = list.OrderBy(output => output.OutputNumber).ToList();

                obj.Outputs.Clear();
                foreach (var output in list)
                    obj.Outputs.Add(output);
            }

            if (obj.DoorEnvironments != null)
            {
                IList<DoorEnvironment> list = new List<DoorEnvironment>();
                foreach (var doorEnvironment in obj.DoorEnvironments)
                {
                    list.Add(DoorEnvironments.Singleton.GetById(doorEnvironment.IdDoorEnvironment));
                }
                list = list.OrderBy(doorEnvironment => doorEnvironment.Number).ToList();

                obj.DoorEnvironments.Clear();
                foreach (var doorEnvironment in list)
                    obj.DoorEnvironments.Add(doorEnvironment);
            }

            if (obj.CardReaders != null)
            {
                IList<CardReader> list = new List<CardReader>();

                foreach (var cardReader in obj.CardReaders)
                {
                    list.Add(CardReaders.Singleton.GetById(cardReader.IdCardReader));
                }

                obj.CardReaders.Clear();
                foreach (var cardReader in list)
                    obj.CardReaders.Add(cardReader);
            }

            if (obj.WatchdogOutput != null)
            {
                obj.WatchdogOutput = Outputs.Singleton.GetById(obj.WatchdogOutput.IdOutput);
            }

            if (obj.OutputTamper != null)
                obj.OutputTamper = Outputs.Singleton.GetById(obj.OutputTamper.IdOutput);

            if (obj.OutputPrimaryPowerMissing != null)
                obj.OutputPrimaryPowerMissing = Outputs.Singleton.GetById(obj.OutputPrimaryPowerMissing.IdOutput);

            if (obj.OutputBatteryIsLow != null)
                obj.OutputBatteryIsLow = Outputs.Singleton.GetById(obj.OutputBatteryIsLow.IdOutput);

            if (obj.OutputUpsOutputFuse != null)
                obj.OutputUpsOutputFuse = Outputs.Singleton.GetById(obj.OutputUpsOutputFuse.IdOutput);

            if (obj.OutputUpsBatteryFault != null)
                obj.OutputUpsBatteryFault = Outputs.Singleton.GetById(obj.OutputUpsBatteryFault.IdOutput);

            if (obj.OutputUpsBatteryFuse != null)
                obj.OutputUpsBatteryFuse = Outputs.Singleton.GetById(obj.OutputUpsBatteryFuse.IdOutput);

            if (obj.OutputUpsOvertemperature != null)
                obj.OutputUpsOvertemperature = Outputs.Singleton.GetById(obj.OutputUpsOvertemperature.IdOutput);

            if (obj.OutputUpsTamperSabotage != null)
                obj.OutputUpsTamperSabotage = Outputs.Singleton.GetById(obj.OutputUpsTamperSabotage.IdOutput);

            if (obj.OutputFuseOnExtensionBoard != null)
                obj.OutputFuseOnExtensionBoard = Outputs.Singleton.GetById(obj.OutputFuseOnExtensionBoard.IdOutput);

            if (obj.AlarmCcuCatUnreachablePresentationGroup != null)
                obj.AlarmCcuCatUnreachablePresentationGroup = PresentationGroups.Singleton.GetById(
                    obj.AlarmCcuCatUnreachablePresentationGroup.IdGroup);

            if (obj.AlarmCcuTransferToArcTimedOutPresentationGroup != null)
                obj.AlarmCcuTransferToArcTimedOutPresentationGroup = PresentationGroups.Singleton.GetById(
                    obj.AlarmCcuTransferToArcTimedOutPresentationGroup.IdGroup);

            if (obj.AlarmTransmitter != null)
                obj.AlarmTransmitter = AlarmTransmitters.Singleton.GetById(obj.AlarmTransmitter.IdAlarmTransmitter);

            if (obj.CcuAlarmArcs != null)
            {
                var ccuAlarmArcs = new LinkedList<CcuAlarmArc>();

                foreach (var ccuAlarmArc in obj.CcuAlarmArcs)
                {
                    ccuAlarmArcs.AddLast(
                        CcuAlarmArcs.Singleton.GetById(
                            ccuAlarmArc.IdCcuAlarmArc));
                }

                obj.CcuAlarmArcs.Clear();

                foreach (var ccuAlarmArc in ccuAlarmArcs)
                {
                    obj.CcuAlarmArcs.Add(ccuAlarmArc);
                }
            }
        }

        public override bool IsReferencedSubObjects(CCU ormObject)
        {
            if (ormObject != null)
            {
                ormObject = GetById(ormObject.IdCCU);

                if (ormObject != null)
                {
                    if (ormObject.DCUs != null && ormObject.DCUs.Count > 0)
                    {
                        foreach (var dcu in ormObject.DCUs)
                        {
                            if (dcu != null)
                            {
                                var actDcu = DCUs.Singleton.GetById(dcu.IdDCU);
                                if (actDcu != null)
                                {
                                    var referencedObjects = DCUs.Singleton.GetReferencedObjectsAllPlugins(actDcu.IdDCU);
                                    if ((referencedObjects != null && referencedObjects.Count > 0 && !DCUs.Singleton.DeleteIfReferenced(actDcu.IdDCU, referencedObjects)) || DCUs.Singleton.IsReferencedSubObjects(actDcu))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }

                    if (ormObject.DoorEnvironments != null && ormObject.DoorEnvironments.Count > 0)
                    {
                        foreach (var doorEnvironment in ormObject.DoorEnvironments)
                        {
                            if (doorEnvironment != null)
                            {
                                var referencedObjects = DoorEnvironments.Singleton.GetReferencedObjectsAllPlugins(doorEnvironment.IdDoorEnvironment);
                                if (referencedObjects != null && referencedObjects.Count > 0 && !DoorEnvironments.Singleton.DeleteIfReferenced(doorEnvironment.IdDoorEnvironment, referencedObjects))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    if (ormObject.CardReaders != null && ormObject.CardReaders.Count > 0)
                    {
                        foreach (var cardReader in ormObject.CardReaders)
                        {
                            if (cardReader != null)
                            {
                                var referencedObjects = CardReaders.Singleton.GetReferencedObjectsAllPlugins(cardReader.IdCardReader);
                                if (referencedObjects != null && referencedObjects.Count > 0 && !CardReaders.Singleton.DeleteIfReferenced(cardReader.IdCardReader, referencedObjects))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        protected override void DeleteSubObjects(object ormObject, ISession session)
        {
            var ccu = ormObject as CCU;
            if (ccu != null)
            {
                if (ccu.DCUs != null && ccu.DCUs.Count > 0)
                {
                    var actCCU = GetObjectForEdit(ccu.IdCCU);
                    while (actCCU != null && actCCU.DCUs != null && actCCU.DCUs.Count > 0)
                    {
                        actCCU.DCUs.Remove(actCCU.DCUs.ElementAt(0));
                        UpdateOnlyInDatabase(actCCU);
                        EditEnd(actCCU);
                        actCCU = GetObjectForEdit(ccu.IdCCU);
                    }

                    if (actCCU != null)
                        EditEnd(actCCU);
                }

                ccu.DCUs = null;
            }
        }

        public void CreateOwnInputs(Guid ccuId, int[] inputs, string name)
        {
            if (ccuId == Guid.Empty || inputs == null || inputs.Length == 0)
                return;

            var ccu = GetById(ccuId);
            if (ccu == null)
                return;

            var existsInputs = new HashSet<int>();
            if (ccu.Inputs != null)
            {
                foreach (var input in ccu.Inputs)
                {
                    existsInputs.Add(input.InputNumber);
                }
            }
            else
            {
                ccu.Inputs = new List<Input>();
            }

            foreach (var intputNumber in inputs)
            {
                if (existsInputs.Contains(intputNumber))
                    continue;

                string newName;
                string nickName = name + (intputNumber + 1).ToString("D2");

                if (Support.EnableParentInFullName)
                    newName = name + (intputNumber + 1).ToString("D2");
                else
                    newName = ccu.Name + StringConstants.SLASHWITHSPACES + name + (intputNumber + 1).ToString("D2");

                var input = Inputs.Singleton.CreateNewOne(newName, nickName, intputNumber);
                input.CCU = ccu;
                Inputs.Singleton.Insert(ref input);
            }
        }

        public void CreateOwnOutputs(Guid ccuId, int[] outputs, string name)
        {
            if (ccuId == Guid.Empty || outputs == null || outputs.Length == 0)
                return;

            var ccu = GetById(ccuId);
            if (ccu == null)
                return;

            var existsOutputs = new HashSet<int>();
            if (ccu.Outputs != null)
            {
                foreach (var output in ccu.Outputs)
                {
                    existsOutputs.Add(output.OutputNumber);
                }
            }
            else
            {
                ccu.Outputs = new List<Output>();
            }

            foreach (var outputNumber in outputs)
            {
                if (existsOutputs.Contains(outputNumber))
                    continue;

                string newName;
                if (Support.EnableParentInFullName)
                    newName = name + (outputNumber + 1).ToString("D2");
                else
                    newName = ccu.Name + StringConstants.SLASHWITHSPACES + name + (outputNumber + 1).ToString("D2");

                var output = Outputs.Singleton.CreateNewOne(newName, outputNumber);
                output.CCU = ccu;
                Outputs.Singleton.Insert(ref output);
            }
        }

        public void CreateDoorEnvironments(Guid guidCCU, int count, string name, MainBoardVariant mainBoardType)
        {
            var ccu = Singleton.GetById(guidCCU);

            if (ccu == null)
                return;

            switch (mainBoardType)
            {
                case MainBoardVariant.CCU40:
                {
                    string localisedName;
                    object licenseValue;

                    if (NCASServer.Singleton.GetLicencePropertyInfo(
                        RequiredLicenceProperties.CCU40MaxDsm.ToString(),
                        null,
                        out localisedName,
                        out licenseValue))
                    {
                        var countByLicense = (int) licenseValue;
                        if (countByLicense >= 0 &&
                            count > countByLicense)
                        {
                            count = countByLicense;
                        }
                    }

                    break;
                }
                case MainBoardVariant.CCU12:
                {
                    string localisedName;
                    object licenseValue;

                    if (NCASServer.Singleton.GetLicencePropertyInfo(
                        RequiredLicenceProperties.CAT12CEMaxDsm.ToString(),
                        null,
                        out localisedName,
                        out licenseValue))
                    {
                        var countByLicense = (int)licenseValue;
                        if (countByLicense >= 0 &&
                            count > countByLicense)
                        {
                            count = countByLicense;
                        }
                    }

                    break;
                }
                case MainBoardVariant.CCU05:
                {
                    string localisedName;
                    object licenseValue;

                    if (NCASServer.Singleton.GetLicencePropertyInfo(
                        RequiredLicenceProperties.CCU05MaxDsm.ToString(),
                        null,
                        out localisedName,
                        out licenseValue))
                    {
                        var countByLicense = (int)licenseValue;
                        if (countByLicense >= 0 &&
                            count > countByLicense)
                        {
                            count = countByLicense;
                        }
                    }

                    break;
                }
                case MainBoardVariant.CAT12CE:
                {
                    string localisedName;
                    object licenseValue;

                    if (NCASServer.Singleton.GetLicencePropertyInfo(
                        RequiredLicenceProperties.CAT12CEMaxDsm.ToString(),
                        null,
                        out localisedName,
                        out licenseValue))
                    {
                        var countByLicense = (int)licenseValue;
                        if (countByLicense >= 0 &&
                            count > countByLicense)
                        {
                            count = countByLicense;
                        }
                    }

                    break;
                }
            }

            if (count == 0 || ccu.DoorEnvironments.Count == count)
                return;

            if (ccu.DoorEnvironments == null || ccu.DoorEnvironments.Count == 0)
            {
                ccu = Singleton.GetObjectForEdit(ccu.IdCCU);
                if (ccu == null)
                    return;

                if (ccu.DoorEnvironments == null)
                {
                    ccu.DoorEnvironments = new List<DoorEnvironment>();
                }

                for (var i = 0; i < count; i++)
                {
                    ccu.DoorEnvironments.Add(DoorEnvironments.Singleton.CreateNew(name, i));
                }

                UpdateOnlyInDatabase(ccu);
                EditEnd(ccu);

                return;
            }

            if (ccu.DoorEnvironments.Count < count)
            {
                ccu = Singleton.GetObjectForEdit(ccu.IdCCU);
                if (ccu == null)
                    return;

                for (var i = ccu.DoorEnvironments.Count; i < count; i++)
                {
                    ccu.DoorEnvironments.Add(DoorEnvironments.Singleton.CreateNew(name, i));
                }

                UpdateOnlyInDatabase(ccu);
                EditEnd(ccu);
            }
        }

        public void GetActualInputStates(CCU ccu)
        {
            if (ccu.Inputs != null)
            {
                foreach (var input in ccu.Inputs)
                {
                    input.SetState(Inputs.Singleton.GetActualStates(input));
                }
            }
        }

        public CCUOnlineState GetCCUState(Guid id)
        {
            return CCUConfigurationHandler.Singleton.GetCCUState(id);
        }

        public bool Unconfigure(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.Unconfigure(ccuGuid);
        }

        public ConfigureResult ConfigureForThisServer(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.ConfigureForThisServer(ccuGuid);
        }

        public ConfigureResult ForceReconfiguration(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.ForceReconfiguration(ccuGuid);
        }

        public CCUConfigurationState GetCCUConfiguredState(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetCCUConfigureState(ccuGuid);
        }

        public CCUConfigurationState GetActualCCUConfiguredState(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetActualCCUConfiguredState(ccuGuid);
        }

        public MainBoardVariant GetCCUMainBoardType(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetCcuMainBoardType(ccuGuid);
        }

        public bool StopUpgradeMode(Guid ccuGuid, bool upgraded)
        {
            return CCUConfigurationHandler.Singleton.StopUpgradeMode(ccuGuid, upgraded);
        }

        public bool StartUpgradeMode(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.StartUpgradeMode(ccuGuid);
        }

        public bool ResetCCU(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.Reset(ccuGuid);
        }

        public bool SoftResetCCU(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.SoftReset(ccuGuid);
        }

        public IList<DoorEnvironment> GetDoorEnvironments(CCU ccu)
        {
            if (ccu.DoorEnvironments != null)
            {
                IList<DoorEnvironment> list = new List<DoorEnvironment>();
                foreach (var doorEnvironment in ccu.DoorEnvironments)
                {
                    list.Add(DoorEnvironments.Singleton.GetById(doorEnvironment.IdDoorEnvironment));
                }
                list = list.OrderBy(doorEnvironment => doorEnvironment.Number).ToList();

                return list;
            }

            return null;
        }

        public CCU GetCCUFormIpAddress(string ipAddress)
        {
            var ccus = Singleton.SelectLinq<CCU>(ccu => ccu.IPAddress == ipAddress);

            if (ccus != null && ccus.Count > 0)
            {
                return ccus.ElementAt(0);
            }

            return null;
        }

        public void DoCCUsLookUp(Guid clientID)
        {
            CCUConfigurationHandler.Singleton.DoCCUsLookUp(clientID);
        }

        public IPSetting GetIpSettings(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetIpSettings(ccuGuid);
        }

        public string SetIpSettings(Guid ccuGuid, IPSetting ipSetting)
        {
            return CCUConfigurationHandler.Singleton.SetIpSettings(ccuGuid, ipSetting);
        }

        public bool IsCCU0(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.IsCCU0(ccuGuid);
        }

        public bool IsCCUUpgrader(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.IsCCUUpgrader(ccuGuid);
        }

        public bool IsCat12Combo(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.IsCat12Combo(ccuGuid);
        }

        public bool HasCat12ComboLicence(Guid idCCU)
        {
            return ComboLicenceManager.Singleton.HasLicence(idCCU);
        }

        public int GetFreeCat12ComboLicenceCount()
        {
            return ComboLicenceManager.Singleton.FreeLicenceCount;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            return idObj is Guid
                ? CCUConfigurationHandler.Singleton
                    .GetAlarmAreasOfImplicitCcu((Guid)idObj)
                    .ToList()
                : null;
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            var ccu = GetById(id);
            if (ccu != null && ccu.CardReaders != null && ccu.CardReaders.Count > 0)
            {
                foreach (var cardReader in ccu.CardReaders)
                {
                    var crReferencedObjects = CardReaders.Singleton.GetReferencedObjectsAllPlugins(cardReader.IdCardReader);
                    if (crReferencedObjects != null && crReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in crReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !IsOwnedDoorEnvironment(ccu, ormObject))
                                return false;
                        }
                    }
                }
            }

            if (ccu != null && ccu.Inputs != null && ccu.Inputs.Count > 0)
            {
                foreach (var input in ccu.Inputs)
                {
                    var inputReferencedObjects = Inputs.Singleton.GetReferencedObjectsAllPlugins(input.IdInput);
                    if (inputReferencedObjects != null && inputReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in inputReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !IsOwnedDoorEnvironment(ccu, ormObject) && !IsOwnedOutput(ccu, ormObject))
                                return false;
                        }
                    }
                }
            }

            if (ccu != null && ccu.Outputs != null && ccu.Outputs.Count > 0)
            {
                foreach (var output in ccu.Outputs)
                {
                    var outputReferencedObjects = Outputs.Singleton.GetReferencedObjectsAllPlugins(output.IdOutput);
                    if (outputReferencedObjects != null && outputReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in outputReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !IsOwnedDoorEnvironment(ccu, ormObject))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool IsOwnedDoorEnvironment(CCU ccu, AOrmObject ormObject)
        {
            if (ormObject is DoorEnvironment && ccu != null)
            {
                var doorEnvironment = ormObject as DoorEnvironment;
                if (doorEnvironment.CCU != null && ccu.Compare(doorEnvironment.CCU))
                    return true;
            }

            return false;
        }

        private static bool IsOwnedOutput(CCU ccu, AOrmObject ormObject)
        {
            var output = ormObject as Output;

            if (output != null)
                if (ccu != null && output.CCU != null && ccu.Compare(output.CCU))
                    return true;

            return false;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<CCU> linqResult;

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
                        ? SelectLinq<CCU>(ccu => ccu.Name.IndexOf(name, StringComparison.Ordinal) >= 0)
                        : SelectLinq<CCU>(ccu => ccu.Name.IndexOf(name, StringComparison.Ordinal) >= 02 
                            || ccu.Description.IndexOf(name, StringComparison.Ordinal) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(ccu => ccu.Name).ToList();
                foreach (var dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<CCU> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = new LinkedList<CCU>(
                    SelectLinq<CCU>(ccu =>
                        ccu.FullTextSearchString.ToLower().Contains(name.ToLower()) ||
                        ccu.Description.ToLower().Contains(name.ToLower())));
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<CCU>(ccu => ccu.FullTextSearchString.ToLower().Contains(name.ToLower()));

            return ReturnAsListOrmObject(linqResult);
        }

        private static IList<AOrmObject> ReturnAsListOrmObject(ICollection<CCU> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(ccu => ccu.Name).ToList();
                foreach (var dp in linqResult)
                {
                    resultList.Add(dp);
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public string GetFirmwareVersion(Guid giudCCU)
        {
            try
            {
                if (giudCCU == Guid.Empty) return null;
                return CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(giudCCU);
            }
            catch
            {
                return null;
            }
        }

        internal bool StopUnpack(Guid guidCCU)
        {
            return CCUConfigurationHandler.Singleton.StopUnpack(guidCCU);
        }

        public ICollection<CCUShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCcu = SelectByCriteria(filterSettings, out error);
            ICollection<CCUShort> result = new List<CCUShort>();
            if (listCcu != null)
            {
                foreach (var ccu in listCcu)
                {
                    var newCcuShort = new CCUShort(ccu)
                    {
                        OnlineState = GetCCUState(ccu.IdCCU),
                        ConfigurationState = GetCCUConfiguredState(ccu.IdCCU)
                    };
                    var mbv = GetCCUMainBoardType(ccu.IdCCU);
                    switch (mbv)
                    {
                        case MainBoardVariant.CCU0_ECHELON:
                            newCcuShort.MainboardType = "CCU (Echelon)";
                            break;
                        case MainBoardVariant.CCU0_RS485:
                            newCcuShort.MainboardType = "CCU (RS485)";
                            break;
                        case MainBoardVariant.CAT12CE:
                            newCcuShort.MainboardType = IsCat12Combo(ccu.IdCCU) 
                                ? @"CCU12 Combo" 
                                : @"CCU12";
                            break;
                        default:
                            newCcuShort.MainboardType = mbv.ToString();
                            break;
                    }

                    result.Add(newCcuShort);
                }
            }
            return result;
        }

        public IList<CcuListObj> GetListObj(out Exception error)
        {
            var listCcu = List(out error);
            if (listCcu == null) return null;

            IList<CcuListObj> result = new List<CcuListObj>();
            foreach (var ccu in listCcu)
            {
                result.Add(new CcuListObj(ccu));
            }
            return result.OrderBy(ccu => ccu.ToString()).ToList();
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listCcu = List(out error);
            IList<IModifyObject> listCcuModifyObj = null;
            if (listCcu != null)
            {
                listCcuModifyObj = new List<IModifyObject>();
                foreach (var ccu in listCcu)
                {
                    listCcuModifyObj.Add(new CcuModifyObj(ccu));
                }
                listCcuModifyObj = listCcuModifyObj.OrderBy(ccu => ccu.ToString()).ToList();
            }
            return listCcuModifyObj;
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idCCU)
        {
            if (ccus != null)
            {
                if (ccus.Count == 0 || !ccus.Contains(idCCU))
                {
                    ccus.Add(idCCU);
                }
            }
        }

        public int GetCeUpgradeActionResult(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetCEUpgradeActionResult(ccuGuid);
        }

        public int[] CommunicationStatistic(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.CommunicationStatistic(ccuGuid);
        }

        public void ResetServerSended(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetServerSended(ccuGuid);
        }

        public void ResetServerReceived(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetServerReceived(ccuGuid);
        }

        public void ResetServerDeserializeError(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetServerDeserializeError(ccuGuid);
        }

        public void ResetServerReceivedError(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetServerReceivedError(ccuGuid);
        }

        public void ResetCcuSended(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuSended(ccuGuid);
        }

        public void ResetCcuReceived(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuReceived(ccuGuid);
        }

        public void ResetCcuDeserializeError(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuDeserializeError(ccuGuid);
        }

        public void ResetCcuReceivedError(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuReceivedError(ccuGuid);
        }

        public void ResetCommunicationStatistic(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuCommunicationStatistic(ccuGuid);
            CCUConfigurationHandler.Singleton.ResetCommunicationStatistic(ccuGuid);
        }

        public void ResetServerMsgRetry(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetServerMsgRetry(ccuGuid);
        }
        public void ResetCcuMsgRetry(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuMsgRetry(ccuGuid);
        }

        public object[] GetCcuStartsCount(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.GetCcuStartsCount(ccuGuid);
        }

        public void ResetCcuStartCounter(Guid ccuGuid)
        {
            CCUConfigurationHandler.Singleton.ResetCcuStartCounter(ccuGuid);
        }

        public void RequestDcuMemoryLoad(Guid ccuGuid, byte logicalAddress)
        {
            CCUConfigurationHandler.Singleton.RequestDcuMemoryLoad(ccuGuid, logicalAddress);
        }

        public string WinCEImageVersion(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.WinCECurrentImageVersion(ccuGuid);
        }

        public CcuConfigurationOptions EnableConfigureThisCcu(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.DetermineConfigureOption(ccuGuid);
        }

        public bool ValidConfigurePassword(Guid ccuGuid, string password)
        {
            return CCUConfigurationHandler.Singleton.CompareCcuConfigurationPassword(ccuGuid, password);
        }

        public bool HasCcuConfigurationPassword(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.IsSetCcuConfigurationPassword(ccuGuid);
        }

        public void NewConfigurePassword(Guid ccuGuid, string password)
        {
            CCUConfigurationHandler.Singleton.SetCcuNewConfigurePassword(ccuGuid, password);
        }

        public bool IsActualWinCeImage(Guid ccuGuid, string winCeFile)
        {
            return CCUConfigurationHandler.Singleton.CcuHasSameWinCeImage(ccuGuid, winCeFile);
        }

        public bool HasAccessToChangeCcuPassword()
        {
            if (GeneralOptions.Singleton.CcuConfigurationToServerByPassword)
            {
                return true;
            }

            return false;
        }

        public string[] CoprocessorBuildNumberStatistics(Guid ccuGuid)
        {
            return CCUConfigurationHandler.Singleton.CoprocessorBuildNumberStatistics(ccuGuid);
        }

        public IList<CardReader> ActualCcuCardReaders(Guid ccuGuid)
        {
            IList<CardReader> resultCardReader = new List<CardReader>();
            var ccu = GetById(ccuGuid);
            if (ccu != null)
            {
                if (ccu.CardReaders != null)
                {
                    foreach (var cr in ccu.CardReaders)
                    {
                        resultCardReader.Add(CardReaders.Singleton.GetById(cr.IdCardReader));
                    }
                }

                if (ccu.DCUs != null)
                {
                    foreach (var dcu in ccu.DCUs)
                    {
                        if (dcu != null)
                        {
                            foreach (var cr in dcu.CardReaders)
                            {
                                resultCardReader.Add(CardReaders.Singleton.GetById(cr.IdCardReader));
                            }
                        }
                    }
                }
            }

            return resultCardReader;
        }

        public string ResultSimulationCardSwiped(
            Guid idCcu,
            ObjectType objectType,
            Guid idObject,
            string cardNumber,
            string pin,
            int pinLength)
        {
            return CCUConfigurationHandler.Singleton.ResultSimulationCardSwiped(
                idCcu,
                objectType,
                idObject,
                cardNumber,
                pin,
                pinLength);
        }

        public State GetTimeZoneDailyPlanState(Guid idCCU, ObjectType objectType, Guid objectGuid)
        {
            return CCUConfigurationHandler.Singleton.GetTimeZonesDailyPlansStateFromCCU(idCCU, objectType, objectGuid);
        }

        public void SendUpsMonitorData(Guid idCcu)
        {
            CCUConfigurationHandler.Singleton.SendUpsMonitorData(idCcu);
        }

        public void StopSendUpsMonitorData(Guid idCcu)
        {
            CCUConfigurationHandler.Singleton.StopSendUpsMonitorData(idCcu);
        }

        public bool GetOtherCCUStatistics(Guid idCcu,
            out int threadsCount,
            out int flashFreeSpace, out int flashSize,
            out bool sdCardPresent, out int sdCardFreeSpace, out int sdCardSize,
            out int freeMemory, out int totalMemory, out int memoryLoad)
        {
            return CCUConfigurationHandler.Singleton.GetOtherCCUStatistics(idCcu,
                out threadsCount,
                out flashFreeSpace, out flashSize,
                out sdCardPresent, out sdCardFreeSpace, out sdCardSize,
                out freeMemory, out totalMemory, out memoryLoad);
        }

        public IList<DcuTestRoutineDataGridObj> GetDcuTestStates(Guid guidCcu)
        {
            try
            {
                var ccu = GetById(guidCcu);
                if (ccu != null)
                {
                    var DcuTestSates = CCUConfigurationHandler.Singleton.ObtainDcuRunningTestStates(ccu.IdCCU);
                    if (DcuTestSates != null)
                    {
                        IList<DcuTestRoutineDataGridObj> result = new List<DcuTestRoutineDataGridObj>();
                        foreach (var rtDcu in DcuTestSates)
                        {
                            var dcuRunningTest = rtDcu as DcuRunningTest;

                            if (dcuRunningTest != null)
                            {
                                var dcu = DCUs.Singleton.GetById(dcuRunningTest._idDcu);
                                if (dcu != null)
                                    result.Add(new DcuTestRoutineDataGridObj(dcuRunningTest, dcu));
                            }
                        }
                        return result;
                    }
                }
            }
            catch
            {

            }
            return null;
        }

        public void SetDcuTest(Guid guidCcu, object[] dcuTest)
        {
            if (guidCcu != Guid.Empty && dcuTest != null)
            {
                SafeThread<Guid, object[]>.StartThread(SendToCcuDcuTestCfn, guidCcu, dcuTest);
            }
        }

        private static void SendToCcuDcuTestCfn(Guid guidCcu, object[] dcuTest)
        {
            foreach (var obj in dcuTest)
            {
                CCUConfigurationHandler.Singleton.SendDcuRoutinTestConfiguration(guidCcu, obj);
            }

        }

        public bool SetTimeManually(Guid guidCCU, DateTime utcDateTime)
        {
            return CCUConfigurationHandler.Singleton.SetTimeManually(guidCCU, utcDateTime);
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid idCCU)
        {
            var objects = new List<AOrmObject>();

            var ccu = GetById(idCCU);
            if (ccu != null)
            {
                if (ccu.ObjBlockAlarmTamperObjectType != null && ccu.ObjBlockAlarmTamperId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmTamperId.Value, ccu.ObjBlockAlarmTamperObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmPrimaryPowerMissingObjectType != null && ccu.ObjBlockAlarmPrimaryPowerMissingId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmPrimaryPowerMissingId.Value, ccu.ObjBlockAlarmPrimaryPowerMissingObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmBatteryIsLowObjectType != null && ccu.ObjBlockAlarmBatteryIsLowId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmBatteryIsLowId.Value, ccu.ObjBlockAlarmBatteryIsLowObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmUpsOutputFuseObjectType != null && ccu.ObjBlockAlarmUpsOutputFuseId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmUpsOutputFuseId.Value, ccu.ObjBlockAlarmUpsOutputFuseObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmUpsBatteryFaultObjectType != null && ccu.ObjBlockAlarmUpsBatteryFaultId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmUpsBatteryFaultId.Value, ccu.ObjBlockAlarmUpsBatteryFaultObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmUpsBatteryFuseObjectType != null && ccu.ObjBlockAlarmUpsBatteryFuseId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmUpsBatteryFuseId.Value, ccu.ObjBlockAlarmUpsBatteryFuseObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmUpsOvertemperatureObjectType != null && ccu.ObjBlockAlarmUpsOvertemperatureId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmUpsOvertemperatureId.Value, ccu.ObjBlockAlarmUpsOvertemperatureObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmUpsTamperSabotageObjectType != null && ccu.ObjBlockAlarmUpsTamperSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmUpsTamperSabotageId.Value, ccu.ObjBlockAlarmUpsTamperSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.ObjBlockAlarmFuseOnExtensionBoardObjectType != null && ccu.ObjBlockAlarmFuseOnExtensionBoardId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(ccu.ObjBlockAlarmFuseOnExtensionBoardId.Value, ccu.ObjBlockAlarmFuseOnExtensionBoardObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (ccu.AlarmTransmitter != null)
                    objects.Add(ccu.AlarmTransmitter);

                if (ccu.CcuAlarmArcs != null)
                {
                    objects.AddRange(ccu.CcuAlarmArcs.Select(
                        ccuAlarmArc =>
                            ccuAlarmArc.AlarmArc)
                        .Cast<AOrmObject>());
                }
            }

            return objects;
        }

        public void ResetCCUCommandTimeouts(Guid guidCCU)
        {
            CCUConfigurationHandler.Singleton.ResetCCUCommandTimeouts(guidCCU);
        }

        public bool ShowDeleteEvents()
        {
            return GeneralOptions.Singleton.ShowHiddenFeature(CgpServerGlobals.DELETE_EVENTS_FEATURE_NAME);
        }

        public bool RunGcCollect(Guid idCcu)
        {
            return CCUConfigurationHandler.Singleton.RunGcCollect(idCcu);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.CCU; }
        }

        public override bool Insert(ref CCU ormObject, out Exception error)
        {
            if (!base.Insert(ref ormObject, out error))
                return false;

            InitLicenceInDb(ormObject);

            return true;
        }

        public override bool Insert(ref CCU ormObject)
        {
            if (!base.Insert(ref ormObject))
                return false;

            InitLicenceInDb(ormObject);

            return true;
        }

        private void InitLicenceInDb(CCU ormObject)
        {
            // Try to save object to db. Retry is required because user should change object between insert and update
            Exception exception;
            int i = 5;
            do
            {
                CCU obj = null;
                exception = null;
                try
                {
                    obj = GetObjectForEdit(ormObject.IdCCU);
                    obj.Cat12ComboLicence = ComboLicenceManager.Singleton
                        .GetInitialValueForDb(obj.IdCCU);
                    Update(obj);
                    EditEnd(obj);
                }
                catch (Exception ex)
                {
                    exception = ex;

                    if (obj != null)
                        EditEnd(obj);
                }
            } while (exception != null
                     && --i > 0);
        }

        public ICollection<CCU> GetCcusForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null)
                    return null;

                return SelectLinq<CCU>(
                    ccu =>
                        (ccu.AlarmCcuCatUnreachablePresentationGroup != null
                         && ccu.AlarmCcuCatUnreachablePresentationGroup.IdGroup == pg.IdGroup)
                        || (ccu.AlarmCcuTransferToArcTimedOutPresentationGroup != null
                            && ccu.AlarmCcuTransferToArcTimedOutPresentationGroup.IdGroup == pg.IdGroup));
            }
            catch
            {
                return null;
            }
        }

        public bool PingIpAddress(string ipAddress, int count)
        {
            var pinger = new Ping();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var reply = pinger.Send(ipAddress, 300);

                    if (reply.Status == IPStatus.Success)
                        return true;
                }

                return false;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public bool RenameObjects(
            Guid idCcu, 
            string newCcuName, 
            ICollection<IModifyObject> objectsToRename)
        {
            var ccu = GetObjectById(idCcu);

            if (ccu == null)
                return false;

            foreach (var objectToRename in objectsToRename)
            {
                bool editAllowed;
                Exception ex;

                switch (objectToRename.GetOrmObjectType)
                {
                    case ObjectType.DCU:

                        var dcuToRename =
                            DCUs.Singleton.GetObjectForEditById(objectToRename.GetId, out editAllowed);

                        if (editAllowed
                            && dcuToRename != null)
                        {
                            dcuToRename.Name = dcuToRename.Name.Replace(ccu.Name, newCcuName);
                            DCUs.Singleton.Update(dcuToRename, out ex);
                            DCUs.Singleton.EditEnd(dcuToRename);
                        }

                        break;

                    case ObjectType.CardReader:

                        var crToRename =
                            CardReaders.Singleton.GetObjectForEditById(objectToRename.GetId, out editAllowed);

                        if (editAllowed
                            && crToRename != null)
                        {
                            crToRename.Name = crToRename.Name.Replace(ccu.Name, newCcuName);
                            CardReaders.Singleton.Update(crToRename, out ex);
                            CardReaders.Singleton.EditEnd(crToRename);
                        }

                        break;

                    case ObjectType.Output:

                        var outputToRename =
                            Outputs.Singleton.GetObjectForEditById(objectToRename.GetId, out editAllowed);

                        if (editAllowed
                            && outputToRename != null)
                        {
                            outputToRename.Name = outputToRename.Name.Replace(ccu.Name, newCcuName);
                            Outputs.Singleton.Update(outputToRename, out ex);
                            Outputs.Singleton.EditEnd(outputToRename);
                        }

                        break;

                    case ObjectType.Input:

                        var inputToRename =
                            Inputs.Singleton.GetObjectForEditById(objectToRename.GetId, out editAllowed);

                        if (editAllowed
                            && inputToRename != null)
                        {
                            inputToRename.Name = inputToRename.Name.Replace(ccu.Name, newCcuName);
                            Inputs.Singleton.Update(inputToRename, out ex);
                            Inputs.Singleton.EditEnd(inputToRename);
                        }

                        break;
                }
            }

            return true;
        }

        public ICollection<IModifyObject> GetObjectsToRename(Guid idCcu)
        {
            var ccu = GetObjectById(idCcu);

            if (ccu == null)
                return null;

            string compareString = ccu.Name + " ";
            var objectsToRename = new List<AOrmObject>();

            objectsToRename.AddRange(ccu.DCUs.Where(dcu => dcu.Name.Contains(compareString)).Cast<AOrmObject>());
            objectsToRename.AddRange(ccu.CardReaders.Where(cr => cr.Name.Contains(compareString)).Cast<AOrmObject>());
            objectsToRename.AddRange(ccu.Inputs.Where(input => input.Name.Contains(compareString)).Cast<AOrmObject>());
            objectsToRename.AddRange(ccu.Outputs.Where(output => output.Name.Contains(compareString)).Cast<AOrmObject>());

            foreach (var dcu in ccu.DCUs)
            {
                var loadedDcu = DCUs.Singleton.GetObjectById(dcu.IdDCU);

                objectsToRename.AddRange(loadedDcu.CardReaders.Where(cr => cr.Name.Contains(compareString)).Cast<AOrmObject>());
                objectsToRename.AddRange(loadedDcu.Inputs.Where(input => input.Name.Contains(compareString)).Cast<AOrmObject>());
                objectsToRename.AddRange(loadedDcu.Outputs.Where(output => output.Name.Contains(compareString)).Cast<AOrmObject>());
            }

            return objectsToRename.Select(item => item.CreateModifyObject()).ToList();
        }

        public int GetNewIndex()
        {
            var ccus = List();

            if (ccus == null)
                return 1;

            var ccuIndexes = new HashSet<int>(
                ccus.Select(
                    ccu =>
                        ccu.IndexCCU));

            for (var newIndex = 1; newIndex < 1000; newIndex++)
            {
                if (!ccuIndexes.Contains(newIndex))
                    return newIndex;
            }

            return 1;
        }

        public bool MakeLogDump(Guid idCcu)
        {
            return CCUConfigurationHandler.Singleton.GetDebugFiles(idCcu);
        }

        public byte[] GetDebugFilesFromCcu(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return null;

            string filePath = string.Format(
                "{0}{1}.zip",
                CCUConfigurationHandler.CCU_DUMPS_DIRECTORY,
                ipAddress);

            if (!File.Exists(filePath))
                return null;

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch
            {
                return null;
            }
        }

        public bool StopCKM(Guid idCcu)
        {
            return CCUConfigurationHandler.Singleton.StopCKM(idCcu);
        }

        public bool StartCKM(Guid idCcu, bool isImplicity)
        {
            return CCUConfigurationHandler.Singleton.StartCKM(idCcu, isImplicity);
        }

        public bool RunProccess(Guid idCcu, string cmd)
        {
            return CCUConfigurationHandler.Singleton.RunProccess(idCcu, cmd);
        }
    }
}



