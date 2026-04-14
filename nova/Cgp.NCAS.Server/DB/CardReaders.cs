using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.Globals;


using TimeZone = Contal.Cgp.Server.Beans.TimeZone;
using CRHWVersion = Contal.Drivers.CardReader.CRHWVersion;
using Contal.Cgp.NCAS.Server.Alarms;
using System.Data;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class CardReaders :
        ANcasBaseOrmTableWithAlarmInstruction<CardReaders, CardReader>, 
        ICardReaders
    {
        private CardReaders()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<CardReader>())
        {
        }

        public override AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            var cardReader = ormObject as CardReader;

            if (cardReader == null)
                return null;

            var dcu = cardReader.DCU;

            return 
                dcu != null
                    ? (AOrmObject)dcu
                    : cardReader.CCU;
        }

        protected override IModifyObject CreateModifyObject(CardReader ormbObject)
        {
            return new CardReaderModifyObj(ormbObject);
        }

        protected override IEnumerable<AOrmObject> GetDirectReferencesInternal(CardReader cardReader)
        {
            ReadOnOffObject(cardReader);
            yield return cardReader.OnOffObject;

            if (cardReader.SecurityDailyPlan != null)
                yield return cardReader.SecurityDailyPlan;

            if (cardReader.SecurityTimeZone != null)
                yield return cardReader.SecurityTimeZone;

            if (cardReader.SecurityDailyPlanForEnterToMenu != null)
                yield return cardReader.SecurityDailyPlanForEnterToMenu;

            if (cardReader.SecurityTimeZoneForEnterToMenu != null)
                yield return cardReader.SecurityTimeZoneForEnterToMenu;

            if (cardReader.CardReaderAlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>();

                foreach (var cardReaderAlarmArc in cardReader.CardReaderAlarmArcs)
                {
                    if (!addedAlarmArcIds.Add(cardReaderAlarmArc.IdAlarmArc))
                        continue;

                    yield return cardReaderAlarmArc.AlarmArc;
                }
            }
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CARD_READERS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CardReadersInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.CARD_READERS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccess(AccessNCAS.CardReadersInsertDeletePerform), login);
        }

        public override void CUDSpecial(CardReader cardReader, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        cardReader.GetId(),
                        cardReader.GetObjectType()));
            }
            else if (cardReader != null)
            {
                if (objectDatabaseAction == ObjectDatabaseAction.Insert
                    && cardReader.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetObjectForEdit(cardReader.DCU.IdDCU);
                    DCUs.Singleton.Update(dcu);
                    DCUs.Singleton.EditEnd(dcu);

                    return;
                }

                DataReplicationManager.Singleton.SendModifiedObjectToCcus(cardReader);
            }
        }

        protected override IEnumerable<CardReader> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<CardReader>(
                cardReader =>
                    cardReader.LocalAlarmInstruction != null
                    && cardReader.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterInsert(CardReader cardReader)
        {
            if (cardReader == null)
                return;

            base.AfterInsert(cardReader);
            
            if (cardReader.CardReaderAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        cardReader.IdCardReader,
                        ObjectType.CardReader),
                    cardReader.CardReaderAlarmArcs.Cast<IAlarmArcForAlarmType>());
        }

        public override void AfterUpdate(CardReader newCardReader, CardReader oldCardReaderBeforeUpdate)
        {
            if (newCardReader == null)
                return;

            base.AfterUpdate(
                newCardReader,
                oldCardReaderBeforeUpdate);

            CCUAlarms.ChangeSettingsForAlarmCROffline(newCardReader);

            if (newCardReader.CardReaderAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        newCardReader.IdCardReader,
                        ObjectType.CardReader),
                    newCardReader.CardReaderAlarmArcs.Cast<IAlarmArcForAlarmType>());
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        newCardReader.IdCardReader,
                        ObjectType.CardReader));
        }

        public IEnumerable<IdAndObjectType> GetAlarmObjects(CardReader cardReader)
        {
            return Enumerable.Repeat(
                new IdAndObjectType(
                    cardReader.IdCardReader,
                    ObjectType.CardReader),
                1);
        }

        public override void AfterDelete(CardReader cardReader)
        {
            if (cardReader == null)
                return;

            base.AfterDelete(cardReader);

            NCASServer.Singleton.GetAlarmsQueue().RemoveAlarmsForAlarmObjects(
                GetAlarmObjects(cardReader));

            CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                new IdAndObjectType(
                    cardReader.IdCardReader,
                    ObjectType.CardReader));

            ConsecutiveEvents.Singleton.CleanConsecutiveEvents(cardReader.IdCardReader);
        }

        public void ConfigureSpecificAlarmArcs()
        {
            var cardReaders = List();

            if (cardReaders == null)
                return;

            foreach (var cardReader in cardReaders)
            {
                if (cardReader.CardReaderAlarmArcs != null)
                    CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                        new IdAndObjectType(
                            cardReader.IdCardReader,
                            ObjectType.CardReader),
                        cardReader.CardReaderAlarmArcs.Cast<IAlarmArcForAlarmType>());
            }
        }

        protected override void LoadObjectsInRelationshipGetById(CardReader obj)
        {
            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
            }

            if (obj.DCU != null)
            {
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);
            }
        }

        protected override void LoadObjectsInRelationship(CardReader obj)
        {
            if (obj.SecurityTimeZone != null)
            {
                obj.SecurityTimeZone = SecurityTimeZones.Singleton.GetById(obj.SecurityTimeZone.IdSecurityTimeZone);
            }

            if (obj.SecurityDailyPlan != null)
            {
                obj.SecurityDailyPlan = SecurityDailyPlans.Singleton.GetById(obj.SecurityDailyPlan.IdSecurityDailyPlan);
            }

            if (obj.SecurityDailyPlanForEnterToMenu != null)
                obj.SecurityDailyPlanForEnterToMenu =
                    SecurityDailyPlans.Singleton.GetById(obj.SecurityDailyPlanForEnterToMenu.IdSecurityDailyPlan);

            if (obj.SecurityTimeZoneForEnterToMenu != null)
                obj.SecurityTimeZoneForEnterToMenu =
                    SecurityTimeZones.Singleton.GetById(obj.SecurityTimeZoneForEnterToMenu.IdSecurityTimeZone);

            if (obj.DCU != null)
            {
                obj.DCU = DCUs.Singleton.GetById(obj.DCU.IdDCU);
            }

            if (obj.CCU != null)
            {
                obj.CCU = CCUs.Singleton.GetById(obj.CCU.IdCCU);
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

            if (obj.SpecialOutputForOffline != null)
            {
                obj.SpecialOutputForOffline = Outputs.Singleton.GetById(obj.SpecialOutputForOffline.IdOutput);
            }

            if (obj.SpecialOutputForTamper != null)
            {
                obj.SpecialOutputForTamper = Outputs.Singleton.GetById(obj.SpecialOutputForTamper.IdOutput);
            }

            if (obj.CardReaderAlarmArcs != null)
            {
                var cardReaderAlarmArcs = new LinkedList<CardReaderAlarmArc>();

                foreach (var cardReaderAlarmArc in obj.CardReaderAlarmArcs)
                {
                    cardReaderAlarmArcs.AddLast(
                        CardReaderAlarmArcs.Singleton.GetById(
                            cardReaderAlarmArc.IdCardReaderAlarmArc));
                }

                obj.CardReaderAlarmArcs.Clear();

                foreach (var cardReaderAlarmArc in cardReaderAlarmArcs)
                {
                    obj.CardReaderAlarmArcs.Add(cardReaderAlarmArc);
                }
            }
        }

        protected override void SaveOnOffObjects(CardReader ormObject)
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

        protected override void ReadOnOffObject(CardReader obj)
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
                    obj.OnOffObject = Outputs.Singleton.GetById(obj.OnOffObjectId);
                }
            }
        }

        public OnlineState GetOnlineStates(Guid guidCardReader)
        {
            return CCUConfigurationHandler.Singleton.GetCardReaderOnlineState(guidCardReader);
        }

        public string GetLastCard(Guid guidCardReader)
        {
            return CCUConfigurationHandler.Singleton.CardReaderGetLastCard(guidCardReader);
        }

        public string GetProtocolVersion(CardReader cardReader)
        {
            if (cardReader == null)
                return string.Empty;

            return CCUConfigurationHandler.Singleton.CardReaderProtocolVersion(cardReader.IdCardReader);
        }

        public string GetFirmwareVersion(CardReader cardReader)
        {
            if (cardReader == null)
                return string.Empty;

            return CCUConfigurationHandler.Singleton.CardReaderFirmwareVersion(cardReader.IdCardReader);
        }

        public string GetHardwareVersion(CardReader cardReader)
        {
            if (cardReader == null)
                return string.Empty;

            return CCUConfigurationHandler.Singleton.CardReaderHardwareVersion(cardReader.IdCardReader);
        }

        public bool? GetHasKeyboard(Guid cardReaderId)
        {
            var cardReader = GetById(cardReaderId);
            if (cardReader == null)
                return false;

            return Contal.Drivers.CardReader.CRConstants.GetHasKeyboard((CRHWVersion)cardReader.CardReaderHardware);
        }

        public bool? GetHasDisplay(Guid cardReaderId)
        {
            var cardReader = GetById(cardReaderId);
            if (cardReader == null)
                return false;

            return Contal.Drivers.CardReader.CRConstants.GetHasDisplay((CRHWVersion)cardReader.CardReaderHardware);
        }

        


        public string GetProtocolMajor(CardReader cardReader)
        {
            if (cardReader == null)
                return string.Empty;

            return CCUConfigurationHandler.Singleton.CardReaderProtocolMajor(cardReader.IdCardReader);
        }

        public CardReaderSceneType GetCardReaderCommand(Guid guidCardReader)
        {
            return CCUConfigurationHandler.Singleton.GetCardReaderCommand(guidCardReader);
        }

        public bool IsUsedInDoorEnvironment(CardReader cardReader)
        {
            try
            {
                if (cardReader == null) return false;

                Exception error;
                IList<FilterSettings> filterCR = new List<FilterSettings>();
                var filterSetting = new FilterSettings(DoorEnvironment.COLUMNCARDREADEREXTERNAL, cardReader, ComparerModes.EQUALL);
                filterCR.Add(filterSetting);

                var doorEnvironments = 
                    DoorEnvironments.Singleton.SelectByCriteria(filterCR, out error);

                if (doorEnvironments != null &&
                    doorEnvironments.Count > 0)
                {
                    return true;
                }

                filterCR.Clear();
                filterSetting = new FilterSettings(DoorEnvironment.COLUMNCARDREADERINTERNAL, cardReader, ComparerModes.EQUALL);
                filterCR.Add(filterSetting);

                doorEnvironments = DoorEnvironments.Singleton.SelectByCriteria(filterCR, out error);

                if (doorEnvironments != null &&
                    doorEnvironments.Count > 0)
                {
                    return true;
                }

                var multiDoors = MultiDoors.Singleton.GetMultiDoorsForCardReader(cardReader.IdCardReader);

                return multiDoors != null && multiDoors.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool IsUsedInDoorEnvironmentByGuid(Guid idCardReader)
        {
            var cardReader = GetById(idCardReader);
            return IsUsedInDoorEnvironment(cardReader);
        }

        public ICollection<CardReader> GetCcuCardReaders(
            Guid idCCU,
            out Exception err)
        {
            err = null;
            try
            {
                return 
                    SelectLinq<CardReader>(
                        cardReader => 
                            cardReader.CCU != null && cardReader.CCU.IdCCU == idCCU || 
                            cardReader.DCU != null && cardReader.DCU.CCU != null && cardReader.DCU.CCU.IdCCU == idCCU);
            }
            catch (Exception exception)
            {
                err = exception;
                return null;
            }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            try
            {
                var cardReader = GetById(idObj);
                if (cardReader == null)
                    return null;

                var result = new List<AOrmObject>();
                if (cardReader.CCU != null)
                {
                    var ccu = CCUs.Singleton.GetById(cardReader.CCU.IdCCU);
                    result.Add(ccu);
                }
                if (cardReader.DCU != null)
                {
                    var dcu = DCUs.Singleton.GetById(cardReader.DCU.IdDCU);
                    result.Add(dcu);
                }

                var doorEnvironmentCR = DoorEnvironments.Singleton.GetDoorEnvironmentForCardReader(cardReader.IdCardReader);
                if (doorEnvironmentCR != null)
                {
                    var doorEnvironment = DoorEnvironments.Singleton.GetById(doorEnvironmentCR.IdDoorEnvironment);
                    result.Add(doorEnvironment);
                }

                var usedInAcl = ACLSettings.Singleton.UsedLikeCardReaderObject(cardReader.IdCardReader,
                    ObjectType.CardReader);
                if (usedInAcl != null)
                {
                    foreach (var acl in usedInAcl)
                    {
                        var outAcl = AccessControlLists.Singleton.GetById(acl.IdAccessControlList);
                        result.Add(outAcl);
                    }
                }

                var usedInAz = AccessZones.Singleton.UsedLikeCardReaderObject(cardReader.IdCardReader,
                    ObjectType.CardReader);
                if (usedInAz != null)
                {
                    foreach (var person in usedInAz)
                    {
                        var outPerson = Persons.Singleton.GetById(person.IdPerson);
                        result.Add(outPerson);
                    }
                }

                if (cardReader.AACardReaders != null)
                {
                    foreach (var aaCR in cardReader.AACardReaders)
                    {
                        var outAlarmArea = AlarmAreas.Singleton.GetById(aaCR.AlarmArea.IdAlarmArea);
                        result.Add(outAlarmArea);
                    }
                }

                var antiPassBackZones =
                    AntiPassBackZones.Singleton.GetAntiPassBackZonesForCardReader(cardReader);

                if (antiPassBackZones != null)
                    foreach (var antiPassBackZoneProxy in antiPassBackZones)
                    {
                        var antiPassBackZone =
                            AntiPassBackZones.Singleton
                                .GetById(antiPassBackZoneProxy.IdAntiPassBackZone);

                        result.Add(antiPassBackZone);
                    }

                var multiDoors = MultiDoors.Singleton.GetMultiDoorsForCardReader(cardReader.IdCardReader);

                if (multiDoors != null)
                {
                    result.AddRange(
                        multiDoors
                            .Select(
                                mutliDoorProxy =>
                                    MultiDoors.Singleton.GetById(mutliDoorProxy.IdMultiDoor))
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

        public ICollection<CardReader> UsedLikeOnOffObject(AOnOffObject onOffObj)
        {
            try
            {
                var type = onOffObj.GetType();

                var typeName = 
                    Assembly.CreateQualifiedName(
                        type.Assembly.GetName().Name, 
                        type.FullName);

                var idOnOffObj = onOffObj.GetId() as Guid?;

                return SelectLinq<CardReader>(
                    cardReader => 
                        cardReader.OnOffObjectId == idOnOffObj &&
                        cardReader.OnOffObjectType == typeName);
            }
            catch
            {
                return null;
            }
        }

        public IList<CardReader> IsReferencedSecurityDailyPlan(Guid idSecurityDailyPlan)
        {
            try
            {
                var cardReaders = SelectLinq<CardReader>(
                    cr =>
                        (cr.SecurityDailyPlan != null
                         && cr.SecurityDailyPlan.IdSecurityDailyPlan == idSecurityDailyPlan)
                        || (cr.SecurityDailyPlanForEnterToMenu != null
                            && cr.SecurityDailyPlanForEnterToMenu.IdSecurityDailyPlan == idSecurityDailyPlan));
                
                if (cardReaders != null && cardReaders.Count > 0)
                {
                    return cardReaders.ToList();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public IList<CardReader> IsReferencedSecurityTimeZone(Guid idSecurityTimeZone)
        {
            try
            {
                var cardReaders = SelectLinq<CardReader>(
                    cr =>
                        (cr.SecurityTimeZone != null
                         && cr.SecurityTimeZone.IdSecurityTimeZone == idSecurityTimeZone)
                        || (cr.SecurityTimeZoneForEnterToMenu != null
                            && cr.SecurityTimeZoneForEnterToMenu.IdSecurityTimeZone == idSecurityTimeZone));

                if (cardReaders != null && cardReaders.Count > 0)
                {
                    return cardReaders.ToList();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public bool IsCardReaderInDoorEnvironment(Guid idCardReader)
        {
            try
            {
                var doorEnvironments = SelectLinq<DoorEnvironment>(de => de.CardReaderExternal.IdCardReader == idCardReader || de.CardReaderInternal.IdCardReader == idCardReader);
                if (doorEnvironments != null && doorEnvironments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool DeleteIfReferenced(object id, IList<AOrmObject> referencedObjects)
        {
            if (referencedObjects == null || referencedObjects.Count == 0)
                return true;

            foreach (var ormObject in referencedObjects)
            {
                if (!(ormObject is CCU) && !(ormObject is DCU) && !(ormObject is DoorEnvironment))
                    return false;
            }

            return true;
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            ICollection<CardReader> linqResult;

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
                        ? SelectLinq<CardReader>(cr => cr.Name.IndexOf(name) >= 0)
                        : SelectLinq<CardReader>(cr => cr.Name.IndexOf(name) >= 0 || cr.Description.IndexOf(name) >= 0);
            }

            if (linqResult != null)
            {
                linqResult = linqResult.OrderBy(cr => cr.Name).ToList();
                foreach (var cr in linqResult)
                {
                    resultList.Add(GetById(cr.IdCardReader));
                }
            }
            return resultList;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<CardReader> linqResult = null;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult = SelectLinq<CardReader>(cr => cr.Name.IndexOf(name) >= 0 || cr.Description.IndexOf(name) >= 0);
            }

            return ReturnAsListOrmObject(linqResult);
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            ICollection<CardReader> linqResult = 
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<CardReader>(cr => cr.Name.IndexOf(name) >= 0);

            return ReturnAsListOrmObject(linqResult);
        }

        private IList<AOrmObject> ReturnAsListOrmObject(IEnumerable<CardReader> linqResult)
        {
            IList<AOrmObject> resultList = new List<AOrmObject>();
            if (linqResult != null)
            {
                foreach (var cr in linqResult)
                {
                    resultList.Add(GetById(cr.IdCardReader));
                }
                resultList = resultList.OrderBy(cr => cr.ToString()).ToList();
            }
            return resultList;
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public ICollection<CardReaderShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings, 
            out Exception error)
        {
            var listCardReader = SelectByCriteria(filterSettings, out error);
            ICollection<CardReaderShort> result = new List<CardReaderShort>();
            if (listCardReader != null)
            {
                foreach (var cr in listCardReader)
                {
                    var shortCardReader = new CardReaderShort(cr)
                    {
                        OnlineState = GetOnlineStates(cr.IdCardReader),
                        UsedInDoorEnvironment = IsUsedInDoorEnvironment(cr),
                        CardReaderSceneType = GetCardReaderCommand(cr.IdCardReader),
                        IsBlocked = GetBlockedState(cr.IdCardReader),
                        SLForEnterToMenu = cr.SLForEnterToMenu.HasValue ? cr.SLForEnterToMenu.Value : (byte)0xFF
                    };
                    result.Add(shortCardReader);
                }
            }
            return result.OrderBy(scr => scr.FullName).ToList();
        }

        public ICollection<CardReaderShort> ShortSelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings)
        {
            var listCardReader = SelectByCriteria(out error, filterJoinOperator, filterSettings);
            ICollection<CardReaderShort> result = new List<CardReaderShort>();
            if (listCardReader != null)
            {
                foreach (var cr in listCardReader)
                {
                    var shortCardReader = new CardReaderShort(cr)
                    {
                        OnlineState = GetOnlineStates(cr.IdCardReader),
                        UsedInDoorEnvironment = IsUsedInDoorEnvironment(cr),
                        CardReaderSceneType = GetCardReaderCommand(cr.IdCardReader),
                        IsBlocked = GetBlockedState(cr.IdCardReader),
                        SLForEnterToMenu = cr.SLForEnterToMenu.HasValue ? cr.SLForEnterToMenu.Value : (byte)0xFF
                    };
                    result.Add(shortCardReader);
                }
            }
            return result.OrderBy(scr => scr.FullName).ToList();
        }

        public IList<IModifyObject> ListModifyObjects(bool allowedCardReadersAssignegToMultiDoors, out Exception error)
        {
            IEnumerable<CardReader> listCardReaders = List(out error);
            if (listCardReaders == null)
                return null;

            if (!allowedCardReadersAssignegToMultiDoors)
                listCardReaders = listCardReaders
                    .Where(
                        cardReader =>
                            !MultiDoors.Singleton.IsCardReaderUsedInMultiDoor(cardReader.IdCardReader));

            return
                listCardReaders
                    .Select(cardReader => new CardReaderModifyObj(cardReader))
                    .OrderBy(cardReaderModifyObj => cardReaderModifyObj.ToString())
                    .Cast<IModifyObject>()
                    .ToList();
        }

        public IList<IModifyObject> ListModifyObjects(
            bool allowedCardReadersAssignegToMultiDoors,
            out Exception error,
            Guid guidImplicitCCU)
        {
            if (guidImplicitCCU == Guid.Empty)
            {
                return ListModifyObjects(allowedCardReadersAssignegToMultiDoors, out error);
            }

            IEnumerable<CardReader> listCardReaders = List(out error);
            if (listCardReaders == null)
                return null;

            listCardReaders = listCardReaders
                .Where(
                    cardReader =>
                    {
                        var ccu = GetParentCcu(cardReader);
                        return ccu != null && ccu.IdCCU == guidImplicitCCU;
                    });

            if (!allowedCardReadersAssignegToMultiDoors)
                listCardReaders = listCardReaders
                    .Where(
                        cardReader =>
                            !MultiDoors.Singleton.IsCardReaderUsedInMultiDoor(cardReader.IdCardReader));

            return
                listCardReaders
                    .Where(
                        cardReader =>
                        {
                            var ccu = GetParentCcu(cardReader);
                            return ccu != null && ccu.IdCCU == guidImplicitCCU;
                        })
                    .Select(cardReader => new CardReaderModifyObj(cardReader))
                    .OrderBy(cardReaderModifyObj => cardReaderModifyObj.ToString())
                    .Cast<IModifyObject>()
                    .ToList();
        }

        public Guid GetParentCCU(Guid idCardreader)
        {
            var ccu = GetParentCcu(GetById(idCardreader));
            
            return ccu != null ? ccu.IdCCU : Guid.Empty;
        }

        public CCU GetParentCcu(CardReader cardReader)
        {
            if (cardReader == null)
                return null;

            return cardReader.CCU ?? (cardReader.DCU != null ? cardReader.DCU.CCU : null);
        }

        public void GetParentCCU(ICollection<Guid> ccus, Guid idCardreader)
        {
            var cardreader = GetById(idCardreader);
            if (ccus != null && cardreader != null)
            {
                if (cardreader.CCU != null)
                {
                    CCUs.Singleton.GetParentCCU(ccus, cardreader.CCU.IdCCU);
                }
                else if (cardreader.DCU != null)
                {
                    DCUs.Singleton.GetParentCCU(ccus, cardreader.DCU.IdDCU);
                }
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidCCU, Guid idCardreader)
        {
            var objects = new List<AOrmObject>();

            var cardreader = GetById(idCardreader);
            if (cardreader != null)
            {
                CCU ccu = 
                    cardreader.DCU != null 
                        ? cardreader.DCU.CCU
                        : cardreader.CCU;

                if (ccu != null && ccu.IdCCU == guidCCU)
                {
                    if (cardreader.OnOffObject != null)
                    {
                        objects.Add(cardreader.OnOffObject);
                    }
                    if (cardreader.SecurityDailyPlan != null)
                    {
                        objects.Add(cardreader.SecurityDailyPlan);
                    }
                    if (cardreader.SecurityTimeZone != null)
                    {
                        objects.Add(cardreader.SecurityTimeZone);
                    }
                    
                    if (cardreader.SecurityDailyPlanForEnterToMenu != null)
                        objects.Add(cardreader.SecurityDailyPlanForEnterToMenu);
                    
                    if (cardreader.SecurityTimeZoneForEnterToMenu != null)
                        objects.Add(cardreader.SecurityTimeZoneForEnterToMenu);

                    if (cardreader.AACardReaders != null && cardreader.AACardReaders.Count > 0)
                    {
                        foreach (var aaCardReader in cardreader.AACardReaders)
                        {
                            if (aaCardReader != null)
                            {
                                objects.Add(aaCardReader);
                            }
                        }
                    }

                    if (cardreader.ObjBlockAlarmOfflineObjectType != null && cardreader.ObjBlockAlarmOfflineId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                            cardreader.ObjBlockAlarmOfflineId.Value,
                            cardreader.ObjBlockAlarmOfflineObjectType.Value);

                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmTamperObjectType != null && cardreader.ObjBlockAlarmTamperId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmTamperId.Value, cardreader.ObjBlockAlarmTamperObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmAccessDeniedObjectType != null && cardreader.ObjBlockAlarmAccessDeniedId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmAccessDeniedId.Value, cardreader.ObjBlockAlarmAccessDeniedObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmUnknownCardObjectType != null && cardreader.ObjBlockAlarmUnknownCardId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmUnknownCardId.Value, cardreader.ObjBlockAlarmUnknownCardObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmCardBlockedOrInactiveObjectType != null && cardreader.ObjBlockAlarmCardBlockedOrInactiveId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmCardBlockedOrInactiveId.Value, cardreader.ObjBlockAlarmCardBlockedOrInactiveObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmInvalidPinObjectType != null && cardreader.ObjBlockAlarmInvalidPinId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmInvalidPinId.Value, cardreader.ObjBlockAlarmInvalidPinObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmInvalidGinObjectType != null && cardreader.ObjBlockAlarmInvalidGinId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmInvalidGinId.Value, cardreader.ObjBlockAlarmInvalidGinObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmInvalidEmergencyCodeObjectType != null && cardreader.ObjBlockAlarmInvalidEmergencyCodeId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmInvalidEmergencyCodeId.Value, cardreader.ObjBlockAlarmInvalidEmergencyCodeObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmAccessPermittedObjectType != null && cardreader.ObjBlockAlarmAccessPermittedId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.ObjBlockAlarmAccessPermittedId.Value, cardreader.ObjBlockAlarmAccessPermittedObjectType.Value);
                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType != null
                        && cardreader.ObjBlockAlarmInvalidGinRetriesLimitReachedId != null)
                    {
                        var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                            cardreader.ObjBlockAlarmInvalidGinRetriesLimitReachedId.Value,
                            cardreader.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType.Value);

                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.FunctionKey1 != null && cardreader.FunctionKey1.IdTimeZoneOrDailyPlan != Guid.Empty)
                    {
                        AOrmObject blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.FunctionKey1.IdTimeZoneOrDailyPlan,
                            cardreader.FunctionKey1.IsUsedTimeZone ? (byte)ObjectType.TimeZone : (byte)ObjectType.DailyPlan);

                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.FunctionKey2 != null && cardreader.FunctionKey2.IdTimeZoneOrDailyPlan != Guid.Empty)
                    {
                        AOrmObject blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(cardreader.FunctionKey2.IdTimeZoneOrDailyPlan,
                            cardreader.FunctionKey2.IsUsedTimeZone ? (byte)ObjectType.TimeZone : (byte)ObjectType.DailyPlan);

                        if (blockingObject != null)
                        {
                            objects.Add(blockingObject);
                        }
                    }

                    if (cardreader.CardReaderAlarmArcs != null)
                    {
                        objects.AddRange(cardreader.CardReaderAlarmArcs.Select(
                            cardReaderAlarmArc =>
                                cardReaderAlarmArc.AlarmArc)
                            .Cast<AOrmObject>());
                    }
                }
            }

            return objects;
        }

        public IList<IModifyObject> GetSpecialOutputs(Guid idCCU)
        {
            var ccu = CCUs.Singleton.GetById(idCCU);
            if (ccu != null)
            {
                var outputs = Outputs.Singleton.GetOutputsFromCCUAndItsDCUs(ccu);
                outputs = Outputs.Singleton.FilterOutputsFromActivators(outputs, null, Guid.Empty);

                if (outputs != null && outputs.Count > 0)
                {
                    var outputsModifyObjects = new List<IModifyObject>();
                    foreach (var output in outputs)
                    {
                        outputsModifyObjects.Add(new OutputModifyObj(output));
                    }

                    return outputsModifyObjects;
                }
            }

            return null;
        }

        public void Reset(Guid cardReaderId)
        {
            var cardReader = GetById(cardReaderId);
            if (cardReader != null)
            {
                var ccuId = Guid.Empty;
                if (cardReader.CCU != null)
                {
                    ccuId = cardReader.CCU.IdCCU;
                }
                else if (cardReader.DCU != null && cardReader.DCU.CCU != null)
                {
                    ccuId = cardReader.DCU.CCU.IdCCU;
                }

                if (ccuId != Guid.Empty)
                    CCUConfigurationHandler.Singleton.ResetCardReader(ccuId, cardReader.IdCardReader);
            }
        }

        public static string GetNewCardReaderName(
            byte address,
            bool enableParentInFullName,
            CCU ccu,
            DCU dcu)
        {
            var result = new StringBuilder();

            if (!enableParentInFullName)
            {
                if (ccu == null && dcu != null)
                    ccu = dcu.CCU;

                if (ccu != null)
                {
                    result.Append(ccu.Name);
                    result.Append(StringConstants.SLASHWITHSPACES);
                }

                if (dcu != null)
                {
                    var dcuName = dcu.Name;
                    var slash = StringConstants.SLASH[0];

                    if (dcuName.Contains(slash))
                        dcuName = dcuName.Split(slash)[1];

                    result.Append(dcuName);
                    result.Append(StringConstants.SLASHWITHSPACES);
                }
            }

            result.Append("CR");
            result.Append(address);

            return result.ToString();
        }

        public void GetActualSecurityLevel(Guid cardReaderId, out SecurityLevel? actualSecurityLeve,
            out SecurityLevel4SLDP? securityLevelStzSdp)
        {
            actualSecurityLeve = null;
            securityLevelStzSdp = null;

            var cardReader = GetById(cardReaderId);
            if (cardReader == null)
                return;

            actualSecurityLeve = (SecurityLevel)cardReader.SecurityLevel;

            if (cardReader.SecurityLevel != (byte) SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
            {
                return;
            }

            if (cardReader.SecurityDailyPlan != null)
            {
                securityLevelStzSdp =
                    (SecurityLevel4SLDP)
                        SecurityTimeAxis.Singleton.GetActualStatusSecurityDP(
                            cardReader.SecurityDailyPlan.IdSecurityDailyPlan);
            }

            if (cardReader.SecurityTimeZone != null)
            {
                securityLevelStzSdp =
                    (SecurityLevel4SLDP)
                        SecurityTimeAxis.Singleton.GetActualStatusSecurityTZ(
                            cardReader.SecurityTimeZone.IdSecurityTimeZone);
            }
        }

        public ICollection<IModifyObject> ModifyObjectsSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception err)
        {
            var cardReaders = 
                SelectByCriteria(
                    filterSettings,
                    out err);

            return
                new LinkedList<IModifyObject>(
                    cardReaders
                        .Select(cardReader => new CardReaderModifyObj(cardReader))
                        .OrderBy(cardReader => cardReader.ToString())
                        .Cast<IModifyObject>());
        }

        public ICollection<IModifyObject> GetAPBZAssignableCRModifyObjects(
            Guid guidCCU,
            out IDictionary<Guid, bool> isCardReaderFromMinimalDe,
            out Exception err)
        {
            isCardReaderFromMinimalDe = new Dictionary<Guid, bool>();

            try
            {
                err = null;

                var doorEnvironments =
                    SelectLinq<DoorEnvironment>(
                        doorEn =>
                            doorEn.ActuatorsDoorEnvironment.HasValue);

                var result = new LinkedList<IModifyObject>();

                foreach (var doorEnvironment in doorEnvironments)
                {
                    var cardReaders =
                        Enumerable
                            .Repeat(
                                doorEnvironment.CardReaderExternal,
                                1)
                            .Concat(
                                Enumerable.Repeat(
                                    doorEnvironment.CardReaderInternal,
                                    1))
                            .Where(
                                cardReader =>
                                    cardReader != null
                                    && (guidCCU == Guid.Empty
                                        || (cardReader.CCU != null && cardReader.CCU.IdCCU == guidCCU)
                                        || (cardReader.DCU != null && cardReader.DCU.CCU != null
                                            && cardReader.DCU.CCU.IdCCU == guidCCU)));

                    foreach (var cardReader in cardReaders)
                    {
                        result.AddLast(new CardReaderModifyObj(cardReader));

                        isCardReaderFromMinimalDe.Add(
                            cardReader.IdCardReader,
                            doorEnvironment.ActuatorsDoorEnvironment == (byte) DoorEnviromentType.Minimal);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                err = e;
                return null;
            }
        }

        public bool GetBlockedState(Guid idCardReader)
        {
            return CCUConfigurationHandler.Singleton.GetBlockedState(idCardReader);
        }

        public void Unblock(
            Guid idCcu,
            Guid idCardReader)
        {
            CCUConfigurationHandler.Singleton.UnblockCardReader(
                idCcu,
                idCardReader);
        }

        public void Unblock(
            Guid idCardReader)
        {
            var idCcu = GetParentCCU(idCardReader);

            if (idCcu.Equals(Guid.Empty))
                return;

            CCUConfigurationHandler.Singleton.UnblockCardReader(
                idCcu,
                idCardReader);
        }

        public override bool CanCreateObject()
        {
            return false;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.CardReader; }
        }

        public bool IsFromMinimalDe(Guid idCardReader)
        {
            var doorEnvironments =
                SelectLinq<DoorEnvironment>(
                    doorEn =>
                        doorEn.ActuatorsDoorEnvironment.HasValue
                        && doorEn.ActuatorsDoorEnvironment == (byte) DoorEnviromentType.Minimal
                        && ((doorEn.CardReaderInternal != null
                             && doorEn.CardReaderInternal.IdCardReader == idCardReader)
                            || (doorEn.CardReaderExternal != null
                                && doorEn.CardReaderExternal.IdCardReader == idCardReader)));

            return doorEnvironments != null && doorEnvironments.Count > 0;
        }

        public bool CodeAlreadyUsed(string codeHashValue)
        {
            var allCardReaders = List();

            return allCardReaders != null
                   && allCardReaders.Any(
                       cardReader =>
                           codeHashValue == cardReader.GIN
                           || codeHashValue == cardReader.GinForEnterToMenu
                           || (cardReader.FunctionKey1 != null
                               && cardReader.FunctionKey1.GIN == codeHashValue)
                           || (cardReader.FunctionKey2 != null
                               && cardReader.FunctionKey2.GIN == codeHashValue));
        }

        public override bool CheckData(CardReader ormObject, out Exception error)
        {
            if (!string.IsNullOrEmpty(ormObject.GIN))
            {
                if (Persons.Singleton.PersonalCodeAlreadyUsed(ormObject.GIN))
                {
                    error = new IwQuick.SqlUniqueException(CardReader.COLUMNGIN);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(ormObject.GinForEnterToMenu))
            {
                if (Persons.Singleton.PersonalCodeAlreadyUsed(ormObject.GinForEnterToMenu))
                {
                    error = new IwQuick.SqlUniqueException(CardReader.COLUMN_GIN_FOR_ENTER_TO_MENU);
                    return false;
                }
            }

            if (ormObject.FunctionKey1 != null && !string.IsNullOrEmpty(ormObject.FunctionKey1.GIN))
            {
                if (Persons.Singleton.PersonalCodeAlreadyUsed(ormObject.FunctionKey1.GIN))
                {
                    error = new IwQuick.SqlUniqueException(CardReader.COLUMNFUNCTIONKEY1);
                    return false;
                }
            }

            if (ormObject.FunctionKey2 != null && !string.IsNullOrEmpty(ormObject.FunctionKey2.GIN))
            {
                if (Persons.Singleton.PersonalCodeAlreadyUsed(ormObject.FunctionKey2.GIN))
                {
                    error = new IwQuick.SqlUniqueException(CardReader.COLUMNFUNCTIONKEY2);
                    return false;
                }
            }

            return base.CheckData(ormObject, out error);
        }
    }
}
