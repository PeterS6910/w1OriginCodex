using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Sys;
using NHibernate;
using NHibernate.Criterion;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Server.Alarms;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class DCUs :
        ANcasBaseOrmTableWithAlarmInstruction<DCUs, DCU>, 
        IDCUs
    {
        private class CudPreparation
            : CudPreparationForObjectWithVersion<DCU>
        {
            protected override IEnumerable<AOrmObjectWithVersion> GetSubObjects(DCU obj)
            {
                if (obj.DoorEnvironments != null)
                    return obj.DoorEnvironments;

                return Enumerable.Empty<AOrmObjectWithVersion>();
            }
        }

        private DCUs()
            : base(
                  null,
                  new CudPreparation())
        {
        }

        public override bool Insert(ref DCU ormObject, out Exception error)
        {
            if (!base.Insert(ref ormObject, out error))
                return false;

            if (ormObject.InputsCount != null)
                CreateOwnInputs(ormObject, ormObject.InputsCount.Value, "Input");

            if (ormObject.OutputsCount != null)
                CreateOwnOutputs(ormObject, ormObject.OutputsCount.Value, "Output");

            return true;
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(DCU dcu)
        {
            var cardReaders = dcu.CardReaders;

            if (cardReaders != null)
                foreach (var cardReader in cardReaders)
                    yield return cardReader;

            var doorEnvironments = dcu.DoorEnvironments;

            if (doorEnvironments != null)
                foreach (var doorEnvironment in doorEnvironments)
                    yield return doorEnvironment;

            var inputs = dcu.Inputs;

            if (inputs != null)
                foreach (var input in inputs)
                    yield return input;

            var outputs = dcu.Outputs;

            if (outputs != null)
                foreach (var output in outputs)
                    yield return output;

            yield return dcu.OfflinePresentationGroup;
            yield return dcu.TamperSabotagePresentationGroup;

            if (dcu.DcuAlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>();

                foreach (var dcuAlarmArc in dcu.DcuAlarmArcs)
                {
                    if (!addedAlarmArcIds.Add(dcuAlarmArc.IdAlarmArc))
                        continue;

                    yield return dcuAlarmArc.AlarmArc;
                }
            }
        }

        protected override IModifyObject CreateModifyObject(DCU ormbObject)
        {
            return new DcuModifyObj(ormbObject);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.DCUS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.DcusInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.DCUS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.DcusInsertDeletePerform), login);
        }

        public override void CUDSpecial(DCU dcu, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        dcu.GetId(),
                        dcu.GetObjectType()));
            }
            else if (dcu != null)
            {
                try
                {
                    DataReplicationManager.Singleton.SendModifiedObjectToCcus(dcu);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        protected override IEnumerable<DCU> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<DCU>(
                dcu =>
                    dcu.LocalAlarmInstruction != null
                    && dcu.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterInsert(DCU dcu)
        {
            if (dcu == null)
                return;

            base.AfterInsert(dcu);

            if (dcu.DcuAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        dcu.IdDCU,
                        ObjectType.DCU),
                    dcu.DcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
        }

        public override void AfterUpdate(DCU newDcu, DCU oldDcuBeforeUpdate)
        {
            if (newDcu != null)
            {
                base.AfterUpdate(
                    newDcu,
                    oldDcuBeforeUpdate);

                CCUAlarms.Singleton.ChangeSettingsForAlarmDCUOffline(newDcu);

                if (newDcu.OfflinePresentationGroup != null &&
                    !newDcu.OfflinePresentationGroup.Compare(
                        oldDcuBeforeUpdate.OfflinePresentationGroup))
                {
                    NCASServer.Singleton.PresentationGroupChanged(
                        AlarmType.DCU_Offline,
                        ObjectType.DCU,
                        newDcu.IdDCU);
                }

                if (newDcu.TamperSabotagePresentationGroup != null &&
                    !newDcu.TamperSabotagePresentationGroup.Compare(
                        oldDcuBeforeUpdate.TamperSabotagePresentationGroup))
                {
                    NCASServer.Singleton.PresentationGroupChanged(
                        AlarmType.DCU_TamperSabotage,
                        ObjectType.DCU,
                        newDcu.IdDCU);
                }

                if (newDcu.DcuAlarmArcs != null)
                    CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                        new IdAndObjectType(
                            newDcu.IdDCU,
                            ObjectType.DCU),
                        newDcu.DcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
                else
                    CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                        new IdAndObjectType(
                            newDcu.IdDCU,
                            ObjectType.DCU));
            }
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(DCU dcu)
        {
            var alarmObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    dcu.IdDCU,
                    ObjectType.DCU),
                1);

            if (dcu.Inputs != null)
            {
                alarmObjects = dcu.Inputs.Aggregate(
                    alarmObjects,
                    (current, input) =>
                        current.Concat(Inputs.Singleton.GetAlarmObjects(input)));
            }

            if (dcu.Outputs != null)
            {
                alarmObjects = dcu.Outputs.Aggregate(
                    alarmObjects,
                    (current, output) =>
                        current.Concat(Outputs.Singleton.GetAlarmObjects(output)));
            }

            if (dcu.DoorEnvironments != null)
            {
                alarmObjects = dcu.DoorEnvironments.Aggregate(
                    alarmObjects,
                    (current, doorEnvironment) =>
                        current.Concat(DoorEnvironments.Singleton.GetAlarmObjects(doorEnvironment)));
            }

            if (dcu.CardReaders != null)
            {
                alarmObjects = dcu.CardReaders.Aggregate(
                    alarmObjects,
                    (current, cardReader) =>
                        current.Concat(CardReaders.Singleton.GetAlarmObjects(cardReader)));
            }

            return alarmObjects;
        }

        public override void AfterDelete(DCU dcu)
        {
            if (dcu == null)
                return;

            base.AfterDelete(dcu);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(dcu));

            CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                new IdAndObjectType(
                    dcu.IdDCU,
                    ObjectType.DCU));
        }

        public void ConfigureSpecificAlarmArcs()
        {
            var dcus = List();

            if (dcus == null)
                return;

            foreach (var dcu in dcus)
            {
                if (dcu.DcuAlarmArcs != null)
                    CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                        new IdAndObjectType(
                            dcu.IdDCU,
                            ObjectType.DCU),
                        dcu.DcuAlarmArcs.Cast<IAlarmArcForAlarmType>());
            }
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(DCU.COLUMNCCU, false));
            c.AddOrder(new Order(DCU.COLUMNLOGICALADDRESS, true));
        }

        protected override void LoadObjectsInRelationshipGetById(DCU obj)
        {
            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
            }
        }

        protected override void LoadObjectsInRelationship(DCU obj)
        {
            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
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

            if (obj.Inputs != null)
            {
                IList<Input> list = new List<Input>();

                foreach (var input in obj.Inputs)
                {
                    list.Add(Inputs.Singleton.GetById(input.IdInput));
                }

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

                obj.DoorEnvironments.Clear();
                foreach (var doorEnvironment in list)
                    obj.DoorEnvironments.Add(doorEnvironment);
            }

            if (obj.OfflinePresentationGroup != null)
            {
                obj.OfflinePresentationGroup = PresentationGroups.Singleton.GetById(
                    obj.OfflinePresentationGroup.IdGroup);
            }

            if (obj.TamperSabotagePresentationGroup != null)
            {
                obj.TamperSabotagePresentationGroup = PresentationGroups.Singleton.GetById(
                    obj.TamperSabotagePresentationGroup.IdGroup);
            }

            if (obj.DcuSabotageOutput != null)
            {
                obj.DcuSabotageOutput = Outputs.Singleton.GetById(obj.DcuSabotageOutput.IdOutput);
            }

            if (obj.DcuOfflineOutput != null)
            {
                obj.DcuOfflineOutput = Outputs.Singleton.GetById(obj.DcuOfflineOutput.IdOutput);
            }

            if (obj.DcuInputsSabotageOutput != null)
            {
                obj.DcuInputsSabotageOutput = Outputs.Singleton.GetById(obj.DcuInputsSabotageOutput.IdOutput);
            }

            if (obj.DcuAlarmArcs != null)
            {
                var dcuAlarmArcs = new LinkedList<DcuAlarmArc>();

                foreach (var dcuAlarmArc in obj.DcuAlarmArcs)
                {
                    dcuAlarmArcs.AddLast(
                        DcuAlarmArcs.Singleton.GetById(
                            dcuAlarmArc.IdDcuAlarmArc));
                }

                obj.DcuAlarmArcs.Clear();

                foreach (var dcuAlarmArc in dcuAlarmArcs)
                {
                    obj.DcuAlarmArcs.Add(dcuAlarmArc);
                }
            }
        }

        public override bool IsReferencedSubObjects(DCU ormObject)
        {
            if (ormObject != null)
            {
                ormObject = GetById(ormObject.IdDCU);

                if (ormObject != null)
                {
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

        public bool CheckLogicalAddress(DCU dcu)
        {
            IList<FilterSettings> filterSettings = new List<FilterSettings>();
            var filterSetting = new FilterSettings(DCU.COLUMNIDDCU, dcu.IdDCU, ComparerModes.NOTEQUALL);
            filterSettings.Add(filterSetting);
            filterSetting = new FilterSettings(DCU.COLUMNCCU, dcu.CCU, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);
            filterSetting = new FilterSettings(DCU.COLUMNLOGICALADDRESS, dcu.LogicalAddress, ComparerModes.EQUALL);
            filterSettings.Add(filterSetting);

            var dcus = SelectByCriteria(filterSettings);

            if (dcus != null && dcus.Count > 0)
            {
                return false;
            }

            return true;
        }

        public bool CreateOwnInputs(DCU obj, int count, string name)
        {
            var dcu = Singleton.GetById(obj.IdDCU);
            ICollection<Input> myInputs = dcu.Inputs;

            if (dcu.InputsCount != null)
            {
                var runUppdate = false;

                myInputs = dcu.Inputs;
                dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                if (dcu.Inputs == null)
                {
                    dcu.Inputs = new List<Input>();
                }
                else
                {
                    dcu.Inputs.Clear();
                }
                var supossedInpust = new bool[count];

                foreach (var input in myInputs)
                {
                    supossedInpust[input.InputNumber] = true;
                    dcu.Inputs.Add(input);
                }

                for (var i = 1; i <= count; i++)
                {
                    if (!supossedInpust[i - 1])
                    {
                        var newName = string.Empty;
                        var nickName = name + i.ToString("D2");
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }
                        dcu.Inputs.Add(Inputs.Singleton.CreateNewOne(newName, nickName, i - 1));
                        runUppdate = true;
                    }
                }

                dcu.InputsCount = null;

                return 
                    runUppdate
                        ? Update(dcu) 
                        : UpdateOnlyInDatabase(dcu);
            }

            if (myInputs == null || myInputs.Count != count)
            {
                if (myInputs == null || (myInputs.Count == 0))
                {
                    dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                    if (myInputs == null)
                    {
                        dcu.Inputs = new List<Input>();
                    }
                    for (var i = 1; i <= count; i++)
                    {
                        var newName = string.Empty;
                        var nickName = name + i.ToString("D2");
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }

                        dcu.Inputs.Add(Inputs.Singleton.CreateNewOne(newName, nickName, i - 1));
                    }
                    return Update(dcu);
                }
                if (myInputs.Count < count)
                {
                    dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                    for (var i = (myInputs.Count + 1); i <= count; i++)
                    {
                        var newName = string.Empty;
                        var nickName = name + i.ToString("D2");
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }

                        dcu.Inputs.Add(Inputs.Singleton.CreateNewOne(newName, nickName, i - 1));
                    }
                    return Update(dcu);
                }
            }

            return true;
        }

        public bool CreateOwnOutputs(DCU obj, int count, string name)
        {
            var dcu = Singleton.GetById(obj.IdDCU);
            ICollection<Output> myOutputs = dcu.Outputs;

            if (dcu.OutputsCount != null)
            {
                var runUppdate = false;

                dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                if (dcu.Outputs == null)
                {
                    dcu.Outputs = new List<Output>();
                }
                else
                {
                    dcu.Outputs.Clear();
                }
                var supossedOutput = new bool[count];

                foreach (var output in myOutputs)
                {
                    supossedOutput[output.OutputNumber] = true;
                    dcu.Outputs.Add(output);
                }

                for (var i = 1; i <= count; i++)
                {
                    if (!supossedOutput[i - 1])
                    {
                        var newName = string.Empty;
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }

                        dcu.Outputs.Add(Outputs.Singleton.CreateNewOne(newName, i - 1));
                        runUppdate = true;
                    }
                }

                dcu.OutputsCount = null;
                return 
                    runUppdate 
                        ? Update(dcu) 
                        : UpdateOnlyInDatabase(dcu);
            }

            if (myOutputs == null || myOutputs.Count != count)
            {
                if (myOutputs == null || (myOutputs.Count == 0))
                {
                    dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                    if (myOutputs == null)
                    {
                        dcu.Outputs = new List<Output>();
                    }
                    for (var i = 1; i <= count; i++)
                    {
                        var newName = string.Empty;
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }

                        dcu.Outputs.Add(Outputs.Singleton.CreateNewOne(newName, i - 1));
                    }
                    return Update(dcu);
                }
                if (myOutputs.Count < count)
                {
                    dcu = Singleton.GetObjectForEdit(dcu.IdDCU);
                    for (int i = (byte)(myOutputs.Count + 1); i <= count; i++)
                    {
                        var newName = string.Empty;
                        if (Support.EnableParentInFullName)
                            newName = name + i.ToString("D2");
                        else
                        {
                            if (dcu.CCU != null)
                                newName = dcu.CCU.Name + StringConstants.SLASHWITHSPACES;
                            if (dcu.Name.Contains(StringConstants.SLASH[0]))
                                newName += dcu.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                            else
                                newName += dcu.Name + StringConstants.SLASHWITHSPACES + name + i.ToString("D2");
                        }

                        dcu.Outputs.Add(Outputs.Singleton.CreateNewOne(newName, i - 1));
                    }
                    return Update(dcu);
                }
            }

            return true;
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var dcu = GetById(idObj);
                if (dcu == null)
                    return null;

                var result = new List<AOrmObject>();
                if (dcu.CCU != null)
                {
                    var ccu = CCUs.Singleton.GetById(dcu.CCU.IdCCU);
                    result.Add(ccu);
                }

                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(dcu.IdDCU, ObjectType.DCU);
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.Add(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(dcu.IdDCU, ObjectType.DCU);
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

        public OnlineState GetOnlineStates(DCU dcu)
        {
            return CCUConfigurationHandler.Singleton.GetDCUOnlineState(dcu);
        }

        public OnlineState GetOnlineStates(Guid idDcu)
        {
            var dcu = GetById(idDcu);
            return CCUConfigurationHandler.Singleton.GetDCUOnlineState(dcu);
        }

        public string GetPhysicalAddress(DCU dcu)
        {
            return CCUConfigurationHandler.Singleton.GetDCUPhysicalAddress(dcu);
        }

        public string GetFirmwareVersion(Guid guidDcu)
        {
            var dcu = GetById(guidDcu);
            if (dcu != null)
            {
                return CCUConfigurationHandler.Singleton.GetDCUFirmwareVersion(dcu);
            }
            return string.Empty;
        }

        public State GetInputsSabotageState(Guid dcuId)
        {
            return CCUConfigurationHandler.Singleton.GetDcuInputsSabotageState(GetById(dcuId));
        }

        public int GetDCUUpgradePercentage(Guid guidCCU, byte logicalAddressDCU)
        {
            return CCUConfigurationHandler.Singleton.GetDCUUpgradePercentage(guidCCU, logicalAddressDCU);
        }

        public bool Reset(Guid ccuGuid, byte dcuLogicalAddress)
        {
            return CCUConfigurationHandler.Singleton.ResetDcu(ccuGuid, dcuLogicalAddress);
        }

        public ICollection<IModifyObject> GetObjectsToRename(Guid idDcu)
        {
            var loadedDcu = GetObjectById(idDcu);

            if (loadedDcu == null)
                return null;

            string compareString = loadedDcu.Name + " ";
            var objectsToRename = new List<AOrmObject>();

            objectsToRename.AddRange(loadedDcu.CardReaders.Where(cr => cr.Name.Contains(compareString)).Cast<AOrmObject>());
            objectsToRename.AddRange(loadedDcu.Inputs.Where(input => input.Name.Contains(compareString)).Cast<AOrmObject>());
            objectsToRename.AddRange(loadedDcu.Outputs.Where(output => output.Name.Contains(compareString)).Cast<AOrmObject>());

            return objectsToRename.Select(item => item.CreateModifyObject()).ToList();
        }

        public ICollection<DCUShort> SvDcuSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var dcuList = SelectByCriteria(filterSettings, out error);
            ICollection<DCUShort> resultList = new List<DCUShort>();

            if (dcuList != null)
            {
                foreach (var dcu in dcuList)
                {
                    var dcuShort = new DCUShort(dcu)
                    {
                        OnlineState = GetOnlineStates(dcu.IdDCU)
                    };
                    dcuShort.StringOnlineState = dcuShort.OnlineState.ToString();
                    if (dcu.DoorEnvironments != null && dcu.DoorEnvironments.Count > 0)
                    {
                        dcuShort.StateDoorEnvironment = DoorEnvironments.Singleton.GetDoorEnvironmentState(dcu.DoorEnvironments.ElementAt(0));
                        dcuShort.StringDoorEnvironmentState = dcuShort.StateDoorEnvironment.ToString();
                    }
                    resultList.Add(dcuShort);
                }
            }

            return resultList.OrderBy(shortDcu => shortDcu.FullName).ToList();
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            if (referencedObjects != null && referencedObjects.Count > 0)
            {
                foreach (var ormObject in referencedObjects)
                {
                    if (!(ormObject is CCU))
                        return false;
                }
            }

            var dcu = GetById(id);
            if (dcu != null && dcu.CardReaders != null && dcu.CardReaders.Count > 0)
            {
                foreach (var cardReader in dcu.CardReaders)
                {
                    var crReferencedObjects = CardReaders.Singleton.GetReferencedObjectsAllPlugins(cardReader.IdCardReader);
                    if (crReferencedObjects != null && crReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in crReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !(ormObject is DCU) && !IsOwnedDoorEnvironment(dcu, ormObject))
                                return false;
                        }
                    }
                }
            }

            if (dcu != null && dcu.Inputs != null && dcu.Inputs.Count > 0)
            {
                foreach (var input in dcu.Inputs)
                {
                    var inputReferencedObjects = Inputs.Singleton.GetReferencedObjectsAllPlugins(input.IdInput);
                    if (inputReferencedObjects != null && inputReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in inputReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !(ormObject is DCU) && !IsOwnedDoorEnvironment(dcu, ormObject) && !IsOwnedOutput(dcu, ormObject))
                                return false;
                        }
                    }
                }
            }

            if (dcu != null && dcu.Outputs != null && dcu.Outputs.Count > 0)
            {
                foreach (var output in dcu.Outputs)
                {
                    var outputReferencedObjects = Outputs.Singleton.GetReferencedObjectsAllPlugins(output.IdOutput);
                    if (outputReferencedObjects != null && outputReferencedObjects.Count > 0)
                    {
                        foreach (var ormObject in outputReferencedObjects)
                        {
                            if (!(ormObject is CCU) && !(ormObject is DCU) && !IsOwnedDoorEnvironment(dcu, ormObject))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool IsOwnedDoorEnvironment(DCU dcu, AOrmObject ormObject)
        {
            var doorEnvironment = ormObject as DoorEnvironment;

            if (doorEnvironment != null && dcu != null)
            {
                if (doorEnvironment.DCU != null && dcu.Compare(doorEnvironment.DCU))
                    return true;
            }

            return false;
        }

        private static bool IsOwnedOutput(DCU dcu, AOrmObject ormObject)
        {
            var output = ormObject as Output;

            if (output != null && dcu != null)
                if (output.DCU != null && dcu.Compare(output.DCU))
                    return true;

            return false;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<DCU> linqResult;

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
                        ? SelectLinq<DCU>(dcu => dcu.Name.IndexOf(name) >= 0)
                        : SelectLinq<DCU>(dcu => dcu.Name.IndexOf(name) >= 0 || dcu.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(dcu => dcu.Name).ToList();
                foreach (var dcu in linqResult)
                {
                    resultList.Add(GetById(dcu.IdDCU));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<DCU> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<DCU>(
                        dcu => 
                            dcu.Name.IndexOf(name) >= 0 || 
                            dcu.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult = 
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<DCU>(dcu => dcu.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(ICollection<DCU> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(dcu => dcu.Name).ToList();
                foreach (var dcu in linqResult)
                {
                    resultList.Add(GetById(dcu.IdDCU));
                }
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public IList<DcuListObj> GetListObj(out Exception error)
        {
            var listDcu = List(out error);
            if (listDcu == null) return null;

            IList<DcuListObj> result = new List<DcuListObj>();
            foreach (var dcu in listDcu)
            {
                result.Add(new DcuListObj(dcu));
            }
            return result.OrderBy(dcu => dcu.Name).ToList();
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listDcu = List(out error);
            IList<IModifyObject> listDcuModifyObj = null;
            if (listDcu != null)
            {
                listDcuModifyObj = new List<IModifyObject>();
                foreach (var dcu in listDcu)
                {
                    listDcuModifyObj.Add(new DcuModifyObj(dcu));
                }
                listDcuModifyObj = listDcuModifyObj.OrderBy(dcu => dcu.ToString()).ToList();
            }
            return listDcuModifyObj;
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idDCU)
        {
            var dcu = GetById(idDCU);
            if (ccus != null && dcu != null && dcu.CCU != null)
            {
                CCUs.Singleton.GetParentCCU(ccus, dcu.CCU.IdCCU);
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idDCU)
        {
            var objects = new List<AOrmObject>();

            var dcu = GetById(idDCU);
            if (dcu != null)
            {
                if (dcu.CardReaders != null)
                {
                    foreach (var item in dcu.CardReaders)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }
                if (dcu.DoorEnvironments != null)
                {
                    foreach (var item in dcu.DoorEnvironments)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }
                if (dcu.Inputs != null)
                {
                    foreach (var item in dcu.Inputs)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }
                if (dcu.Outputs != null)
                {
                    foreach (var item in dcu.Outputs)
                    {
                        if (item != null)
                        {
                            objects.Add(item);
                        }
                    }
                }

                if (dcu.ObjBlockAlarmOfflineObjectType != null && dcu.ObjBlockAlarmOfflineId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        dcu.ObjBlockAlarmOfflineId.Value,
                        dcu.ObjBlockAlarmOfflineObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (dcu.ObjBlockAlarmTamperObjectType != null && dcu.ObjBlockAlarmTamperId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(dcu.ObjBlockAlarmTamperId.Value, dcu.ObjBlockAlarmTamperObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (dcu.DcuAlarmArcs != null)
                {
                    objects.AddRange(dcu.DcuAlarmArcs.Select(
                        dcuAlarmArc =>
                            dcuAlarmArc.AlarmArc)
                        .Cast<AOrmObject>());
                }
            }

            return objects;
        }

        public DCU GetDcuByLogicalAddressAndCcu(byte logicalAddress, CCU parentCcu)
        {
            var linqResult = SelectLinq<DCU>(dcu => dcu.CCU == parentCcu);

            if (linqResult != null)
            {
                foreach (var dcu in linqResult)
                {
                    if (dcu.LogicalAddress == logicalAddress)
                    {
                        return dcu;
                    }
                }
            }
            return null;
        }

        public static void RemoveDCUsWithNullReferenceForCCU()
        {
            ISession session = NhHelper.Singleton.GetSession();
            var sqlCommand = string.Empty;
            sqlCommand += "delete from CentralNameRegister where CkUnique in (select IdCardReader from CardReader where DCU in (select IdDCU from DCU where CCU is NULL)) ";
            sqlCommand += "delete from CardReader where DCU in (select IdDCU from DCU where CCU is NULL) ";

            sqlCommand += "delete from CentralNameRegister where CkUnique in (select IdDoorEnvironment from DoorEnvironment where DCU in (select IdDCU from DCU where CCU is NULL)) ";
            sqlCommand += "delete from DoorEnvironment where DCU in (select IdDCU from DCU where CCU is NULL) ";

            sqlCommand += "delete from CentralNameRegister where CkUnique in (select IdInput from Input where DCU in (select IdDCU from DCU where CCU is NULL)) ";
            sqlCommand += "delete from Input where DCU in (select IdDCU from DCU where CCU is NULL) ";

            sqlCommand += "delete from CentralNameRegister where CkUnique in (select IdOutput from Output where DCU in (select IdDCU from DCU where CCU is NULL)) ";
            sqlCommand += "delete from Output where DCU in (select IdDCU from DCU where CCU is NULL) ";

            sqlCommand += "delete from CentralNameRegister where CkUnique in (select IdDCU from DCU where CCU is NULL) ";
            sqlCommand += "delete from DCU where CCU is NULL";
            IQuery sQuery = session.CreateSQLQuery(sqlCommand);
            sQuery.ExecuteUpdate();
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var dcu = ormObject as DCU;

            return 
                dcu != null 
                    ? dcu.CCU
                    : null;
        }

        public override bool CanCreateObject()
        {
            return false;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.DCU; }
        }

        public ICollection<DCU> GetDcusForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null)
                    return null;

                return SelectLinq<DCU>(
                    dcu =>
                        (dcu.OfflinePresentationGroup != null
                         && dcu.OfflinePresentationGroup.IdGroup == pg.IdGroup)
                        || (dcu.TamperSabotagePresentationGroup != null
                            && dcu.TamperSabotagePresentationGroup.IdGroup == pg.IdGroup));
            }
            catch
            {
                return null;
            }
        }

        public DCU GetByLogicalAddress(
            Guid idCcu,
            byte dcuLogicalAddress)
        {
            var dcus = SelectLinq<DCU>(
                dcu =>
                    dcu.CCU != null
                    && dcu.CCU.IdCCU == idCcu
                    && dcu.LogicalAddress == dcuLogicalAddress);

            if (dcus == null)
                return null;

            return dcus.FirstOrDefault();
        }
    }
}