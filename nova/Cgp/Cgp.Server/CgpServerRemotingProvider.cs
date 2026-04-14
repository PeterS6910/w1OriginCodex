using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.StructuredSiteEvaluator;
using Contal.IwQuick.Net;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Server.DB;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Sys.Microsoft;
using Contal.LwDhcp;
using System.Net;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Microsoft.Win32;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.Cgp.Globals;
using Contal.SLA.Client.Interfaces;
using Contal.IwQuick.Threads;

using NHibernate.Mapping;
using Contal.Cgp.NCAS.RemotingCommon;

namespace Contal.Cgp.Server
{
    /// <summary>
    /// provides gateway for ORM based objects and operations between CGP Server and client
    /// </summary>
    public class CgpServerRemotingProvider : ARemotingService, ICgpServerRemotingProvider
    {
        public CgpServerRemotingProvider()
        {
            base.SetRemotingAuthentication(new LoginAuthentication());
        }

        private static volatile CgpServerRemotingProvider _singleton = null;
        private static object _syncRoot = new object();

        public static CgpServerRemotingProvider Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CgpServerRemotingProvider();
                    }

                return _singleton;
            }
        }


        string ICgpServerRemotingProvider.ToString()
        {
            return base.ToString();
        }

        public IPersons Persons
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.Persons.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ILogins Logins
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.Logins.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICardPairs CardPairs
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CardPairs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IPresentationGroups PresentationGroups
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.PresentationGroups.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IPresentationFormatters PresentationFormatters
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.PresentationFormatters.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IEventlogs Eventlogs
        {
            get
            {
                try
                {
                    return DB.Eventlogs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }
        public IConsecutiveEvent ConsecutiveEvents
        {
            get
            {
                try
                {
                    return DB.ConsecutiveEvents.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ISystemEvents SystemEvents
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.SystemEvents.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICisNGs CisNGs
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CisNGs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICisNGGroups CisNGGroups
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CisNGGroups.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICards Cards
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.Cards.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICardSystems CardSystems
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CardSystems.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICars Cars
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.Cars.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICarCards CarCards
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CarCards.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICentralNameRegisters CentralNameRegisters
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CentralNameRegisters.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IDailyPlans DailyPlans
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.DailyPlans.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IDayIntervals DayIntervals
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.DayIntervals.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IServerGenaralOptionsProvider ServerGenaralOptionsProvider
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return ServerGeneralOptionsProvider.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ITimeZones TimeZones
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.TimeZones.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ITimeZoneDateSettings TimeZoneDateSettings
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.TimeZoneDateSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IDayTypes DayTypes
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.DayTypes.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICalendars Calendars
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.Calendars.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICalendarDateSettings CalendarDateSettings
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CalendarDateSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICustomAccessControls CustomAccessControls
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return CustomAccesControls.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IUserFoldersSutructures UserFoldersSutructures
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return UserFoldersStructures.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IServerGeneralOptionsDBs ServerGeneralOptionsDBs
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.ServerGeneralOptionsDBs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAlarmPrioritiesDbs AlarmPrioritiesDbs
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.AlarmPrioritiesDbs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICSVImportSchemas CSVImportSchemas
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.CSVImportSchemas.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IGlobalAlarmInstructions GlobalAlarmInstructions
        {
            get
            {
                try
                {
                    base.ValidateSession();
                    return DB.GlobalAlarmInstructions.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICardTemplates CardTemplates
        {
            get
            {
                base.ValidateSession();
                return DB.CardTemplates.Singleton;
            }
        }

        public ILoginGroups LoginGroups
        {
            get
            {
                ValidateSession();
                return DB.LoginGroups.Singleton;
            }
        }

        public IStructuredSubSites StructuredSubSites
        {
            get
            {
                ValidateSession();
                return DB.StructuredSubSites.Singleton;
            }
        }

        public uint DatabaseLoginCount
        {
            get { return uint.Parse(DB.Logins.Singleton.List().Count.ToString()); }
        }

        public bool DemoLicence
        {
            get { return CgpServer.Singleton.DemoLicense; }
        }

        public bool ClientConnect(string friendlyName, Guid clientID)
        {
#if !DEBUG
            string localisedName = string.Empty;
            object value = null;
            if (!CgpServer.Singleton.DemoLicense
                &&
                !CgpServer.Singleton.GetLicencePropertyInfoFromServerAndPlugins(RequiredLicenceProperties.ConnectionCount.ToString(), out localisedName, out value))
                return false;

            int allowedConnectionCount = 0;
            if (!CgpServer.Singleton.DemoLicense
                &&
                !int.TryParse(value.ToString(), out allowedConnectionCount))
                return false;

            if (CgpServer.Singleton.DemoLicense || (CgpServer.Singleton.ConnectedClients < allowedConnectionCount))
            {
                if (!CgpServer.Singleton.AddConnectedClient(clientID))
                    return false;
#endif
            string clientParameters;
            IPAddress ip = (IPAddress)CallContext.GetData("ClientIP");
            EventlogParameter parameter = new EventlogParameter(EventlogParameter.TYPECLIENTIP, ip.ToString());
            IList<EventlogParameter> parameterList = new List<EventlogParameter>();
            parameterList.Add(parameter);
            clientParameters = "with IP " + ip.ToString();

            if (!string.IsNullOrEmpty(friendlyName))
            {
                parameter = new EventlogParameter(EventlogParameter.TYPECLIENTFRIENDLYNAME, friendlyName);
                parameterList.Add(parameter);
                clientParameters += " and FRIENDLYNAME " + friendlyName;
            }

            DB.Eventlogs.Singleton.InsertEvent(Eventlog.TYPECLIENTCONNECT, GetType().Assembly.GetName().Name, null,
                "Client " + clientParameters + " is connected", parameterList);

            if (string.IsNullOrEmpty(friendlyName))
            {
                SystemEventPerformer.ReportSystemEventStatic(SystemEvent.CLIENT_CONNECTED,
                    " IP Address:" + ip.ToString());
            }
            else
            {
                SystemEventPerformer.ReportSystemEventStatic(SystemEvent.CLIENT_CONNECTED,
                    " IP Address:" + ip.ToString() + " Friendly name:" + friendlyName);
            }
            return true;
#if !DEBUG
            }
            else
            {
                return false;
            }
#endif
        }

        public void SendInfoClientConnected(Guid clientID)
        {
            CgpServer.Singleton.ClientIsConnected(clientID);
        }

        public void ClientDisconnect(string friendlyName, Guid clientID)
        {
#if !DEBUG
            CgpServer.Singleton.RemoveFromConnectedClients(clientID);
#endif
            string strClientParameters;
            IPAddress ip = (IPAddress)CallContext.GetData("ClientIP");
            EventlogParameter parameter = new EventlogParameter(EventlogParameter.TYPECLIENTIP, ip.ToString());
            IList<EventlogParameter> parameterList = new List<EventlogParameter>();
            parameterList.Add(parameter);
            strClientParameters = "with IP " + ip.ToString();

            if (!string.IsNullOrEmpty(friendlyName))
            {
                parameter = new EventlogParameter(EventlogParameter.TYPECLIENTFRIENDLYNAME, friendlyName);
                parameterList.Add(parameter);
                strClientParameters += " and FRIENDLYNAME " + friendlyName;
            }

            DB.Eventlogs.Singleton.InsertEvent(Eventlog.TYPECLIENTDISCONNECT, GetType().Assembly.GetName().Name, null, "Client " + strClientParameters + " is disconnected", parameterList);

            if (string.IsNullOrEmpty(friendlyName))
            {
                SystemEventPerformer.ReportSystemEventStatic(SystemEvent.CLIENT_DISCONNECTED, " IP Address:" + ip.ToString());
            }
            else
            {
                SystemEventPerformer.ReportSystemEventStatic(SystemEvent.CLIENT_DISCONNECTED, " IP Address:" + ip.ToString() + " Friendly name:" + friendlyName);
            }
        }

        public byte GetDailyPlanActualStatus(Guid idDailyPlan)
        {
            return TimeAxis.Singleton.GetActualStatusDP(idDailyPlan);
        }

        public byte GetTimeZoneActualStatus(Guid idTimeZone)
        {
            return TimeAxis.Singleton.GetActualStatusTZ(idTimeZone);
        }

        public void StopServer(out Exception error)
        {
            ValidateSession();
            error = null;

            if (!AccessChecker.HasAccessControl(
                    BaseAccess.GetAccess(LoginAccess.GeneralOptionsServerControlAdmin),
                    AccessChecker.GetActualLogin()))
            {
                error = new AccessDeniedException();
            }

            CgpServer.Singleton.StopProcessing();
            CgpServer.Singleton.StopServiceIfRunning();
        }

        public string Version
        {
            get { return CgpServer.Singleton.Version; }
        }

        public bool HasAccess(Access access)
        {
            try
            {
                ValidateSession();
            }
            catch (Exception)
            {
                return false;
            }

            return AccessChecker.HasAccessControl(access, AccessChecker.GetActualLogin());
        }

        public bool HasAccess(List<Access> accesses)
        {
            try
            {
                ValidateSession();
            }
            catch (Exception)
            {
                return false;
            }

            return AccessChecker.HasAccessControl(accesses, AccessChecker.GetActualLogin());
        }

        public bool HasAccessView(ObjectType objectType, object id)
        {
            try
            {
                ValidateSession();
            }
            catch (Exception)
            {
                return false;
            }

            return HasAccessView(objectType, id, AccessChecker.GetActualLogin());
        }

        public bool HasAccessView(ObjectType objectType, object id, string loginUserName)
        {
            try
            {
                ValidateSession();
            }
            catch (Exception)
            {
                return false;
            }

            return HasAccessView(objectType, id, DB.Logins.Singleton.GetLoginByUserName(loginUserName));
        }

        private bool HasAccessView(ObjectType objectType, object id, Login login)
        {
            if (login == null)
                return false;

            var tableOrm = GetTableOrmForObjectType(objectType);
            if (tableOrm == null)
                return false;

            return tableOrm.HasAccessViewForObject(tableOrm.GetObjectById(id), login);
        }

        public bool HasObjectParent(AOrmObject ormObject)
        {
            if (ormObject == null)
                return false;

            var tableOrm = GetTableOrmForObjectType(ormObject.GetObjectType());
            if (tableOrm == null)
                return false;

            return tableOrm.GetObjectParent(ormObject) != null;
        }

        public bool IsObjectTypeRelevantForStructuredSites(ObjectType objectType)
        {
            return GlobalEvaluator.IsObjectTypeRelevantForStructuredSites(objectType);
        }

        public bool Unauthenticate(out Exception error)
        {
            error = null;
            var login = AccessChecker.GetActualLogin();

            if (!DB.Logins.Singleton.GetAccessControl(login.IdLogin, BaseAccess.GetAccess(LoginAccess.UnlockClientAplicationPerform)) &&
                !DB.Logins.Singleton.GetAccessControl(login.IdLogin, BaseAccess.GetAccess(LoginAccess.SuperAdmin)))
            {
                error = new AccessDeniedException();
                return false;
            }

            Unauthenticate();

            return true;
        }

        public ICollection<ServerAlarmCore> GetAlarms()
        {
            var result = DB.StructuredSubSites.Singleton.GetAlarmsForLogin(AlarmsManager.Singleton.GetAlarms());
            if (result != null)
            {
                var cnrDic = new Dictionary<Guid, CentralNameRegister>();
                ICollection<CentralNameRegister> cnr = DB.CentralNameRegisters.Singleton.List();
                if (cnr == null)
                    return null;

                foreach (CentralNameRegister register in cnr)
                {
                    cnrDic.Add(register.Id, register);
                }

                var ojectsNameCache = new Dictionary<IdAndObjectType, string>();

                foreach (var alarm in result)
                {
                    var relatedObjects = alarm.RelatedObjects;

                    if (relatedObjects == null)
                        continue;

                    var listTp = new List<Eventlogs.ObjTypePriority>();
                    var s = new StringBuilder();

                    foreach (var relatedObject in relatedObjects)
                    {
                        var id = (Guid)relatedObject.Id;

                        var specificName = alarm.GetSpecificObjectName(relatedObject);

                        if (!string.IsNullOrEmpty(specificName))
                        {
                            listTp.Add(new Eventlogs.ObjTypePriority(
                                specificName,
                                ObjTypeHelper.GetObjectTypePriority(
                                    (byte)relatedObject.ObjectType)));

                            continue;
                        }

                        if (cnrDic.ContainsKey(id))
                            listTp.Add(
                                new Eventlogs.ObjTypePriority(
                                    cnrDic[id],
                                    ojectsNameCache));
                    }

                    if (alarm.Alarm.AlarmKey.ReferencedAlarmId != Guid.Empty)
                    {
                        var referencedServerAlarm = AlarmsManager.Singleton.FindServerAlarmByIdServerAlarm(
                            new IdServerAlarm(
                                alarm.IdServerAlarm.IdOwner,
                                alarm.Alarm.AlarmKey.ReferencedAlarmId));

                        if (referencedServerAlarm != null)
                        {
                            var referencedServerAlarmProxy = referencedServerAlarm.ServerAlarmCore;

                            listTp.Add(new Eventlogs.ObjTypePriority(
                                referencedServerAlarmProxy.Name,
                                0));

                            alarm.ReferencedServerAlarm = referencedServerAlarmProxy;
                        }
                    }

                    foreach (Eventlogs.ObjTypePriority priorityObj in listTp)
                    {
                        if (s.Length != 0)
                            s.Append(", ");
                        s.Append(priorityObj.Name);
                    }
                    alarm.ParentsString = s.ToString();

                    s = new StringBuilder();
                    for (int i = listTp.Count - 1; i >= 0; i--)
                    {
                        if (s.Length != 0)
                            s.Append(", ");
                        s.Append(listTp[i].Name);
                    }
                    alarm.ParentsStringReversed = s.ToString();

                    alarm.HasAlarmInstructions = HasAlarmInstructions(alarm);
                }
            }

            return result;
        }

        private bool HasAlarmInstructions(ServerAlarmCore serverAlarm)
        {
            var alarmRelatedObjects = serverAlarm.RelatedObjects;

            if (alarmRelatedObjects == null)
                return false;

            var alarmType = serverAlarm.Alarm.AlarmKey.AlarmType;

            ObjectType? closestParentObject =
                AlarmsManager.Singleton.GetClosestParentObject(alarmType);

            ObjectType? secondClosestParentObject =
                AlarmsManager.Singleton.GetSecondClosestParentObject(alarmType);

            foreach (var alarmRelatedObject in alarmRelatedObjects)
            {
                if (alarmRelatedObject != null
                    && closestParentObject != ObjectType.NotSupport
                    && (closestParentObject == ObjectType.AllObjectTypes
                        || alarmRelatedObject.ObjectType.Equals(closestParentObject)
                        || alarmRelatedObject.ObjectType.Equals(secondClosestParentObject)))
                {
                    if (HasLocalAlarmInstruction(alarmRelatedObject)
                        || GlobalAlarmInstructions.ExistsGlobalAlarmInstructionsForObject(
                            alarmRelatedObject.ObjectType,
                            alarmRelatedObject.Id.ToString()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasLocalAlarmInstruction(IdAndObjectType idAndObjectType)
        {
            var tableOrm = GetTableOrmForObjectType(idAndObjectType.ObjectType)
                as IBaserOrmTableWithAlarmInstruction;

            if (tableOrm == null)
                return false;

            return tableOrm.HasLocalAlarmInstruction(idAndObjectType.Id);
        }

        public ServerAlarmCore AcknowledgeAlarmState(IdServerAlarm idServerAlarm)
        {
            var acknowledgedAlarm = AlarmsManager.Singleton.AcknowledgeAlarm(idServerAlarm);

            return acknowledgedAlarm != null
                ? acknowledgedAlarm.ServerAlarmCore
                : null;
        }

        public void ConfirmAlarmsState(IEnumerable<IdServerAlarm> idsServerAlarm)
        {
            SafeThread.StartThread(() => AlarmsManager.Singleton.AcknowledgeAlarm(idsServerAlarm));
        }

        public void BlockUnblockAlarms(IEnumerable<IdServerAlarm> idsServerAlarm, bool block)
        {
            SafeThread<List<Guid>, bool>.StartThread(
                AlarmsManager.Singleton.BlockUnblockAlarms,
                idsServerAlarm,
                block);
        }

        public ServerAlarmCore BlockAlarm(IdServerAlarm idServerAlarm)
        {
            var blockedAlarm = AlarmsManager.Singleton.BlockAlarm(idServerAlarm);

            return blockedAlarm != null
                ? blockedAlarm.ServerAlarmCore
                : null;
        }

        public ServerAlarmCore UnblockAlarm(IdServerAlarm idServerAlarm)
        {
            var unblockedAlarm = AlarmsManager.Singleton.UnblockAlarm(idServerAlarm);

            return unblockedAlarm != null
                ? unblockedAlarm.ServerAlarmCore
                : null;
        }

        public ICollection<TableTypeSettings> GetSearchableObjectTypes(ICollection<string> plugins)
        {
            var listTypes = new LinkedList<TableTypeSettings>();
            foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
            {
                var tableOrm = GetTableOrmForObjectType(objectType);
                if (tableOrm != null &&
                    tableOrm.ImplementsSearchFunctions() &&
                    (string.IsNullOrEmpty(tableOrm.GetPluginName()) || plugins.Contains(tableOrm.GetPluginName())) &&
                    tableOrm.HasAccessView())
                {
                    listTypes.AddLast(new TableTypeSettings(
                        objectType,
                        tableOrm.GetPluginName(),
                        tableOrm.CanCreateObject()));
                }
            }

            return listTypes;
        }

        public IEnumerable<AOrmObject> SearchObjects(string name, ObjectType? objectType,
            ICollection<string> plugins)
        {
            if (objectType != null)
            {
                var searchObjects = DoSearchObjects(true, name, objectType.Value, plugins);
                return searchObjects == null ? null : new LinkedList<AOrmObject>(searchObjects);
            }

            IEnumerable<AOrmObject> result = null;

            foreach (ObjectType actObjectType in Enum.GetValues(typeof(ObjectType)))
            {
                var searchObjects = DoSearchObjects(true, name, actObjectType, plugins);
                if (searchObjects == null)
                    continue;

                if (result == null)
                {
                    result = searchObjects;
                    continue;
                }

                result = result.Concat(searchObjects);
            }

            return result == null ? null : new LinkedList<AOrmObject>(result);
        }

        private IEnumerable<AOrmObject> DoSearchObjects(
            bool isNullObjectType,
            string name,
            ObjectType objectType,
            ICollection<string> plugins)
        {
            return
                SearchObjects(
                    objectType,
                    plugins,
                    tableOrm =>
                        tableOrm.GetSearchList(
                            name,
                            isNullObjectType || string.IsNullOrEmpty(name)));
        }

        private IEnumerable<AOrmObject> SearchObjects(
            ObjectType objectType,
            ICollection<string> plugins,
            Func<ITableORM, IEnumerable<AOrmObject>> searchObjects)
        {
            if (searchObjects == null)
                return null;

            var tableOrm = GetTableOrmForObjectType(objectType);
            if (tableOrm != null &&
                tableOrm.ImplementsSearchFunctions() &&
                (string.IsNullOrEmpty(tableOrm.GetPluginName()) || plugins.Contains(tableOrm.GetPluginName())))
            {
                var ormObjects = searchObjects(tableOrm);
                if (ormObjects != null)
                {
                    return ormObjects.Where(ormObject => HasAccessView(objectType, ormObject.GetId()));
                }
            }

            return null;
        }

        public IEnumerable<AOrmObject> GetAllOrmObjectsOfObjectType(
            ObjectType? objectType,
            List<string> plugins)
        {
            return SearchObjects(null, objectType, plugins);
        }

        #region Database Search

        public IEnumerable<AOrmObject> GetFultextSearchResult(string name, List<string> plugins)
        {
            IEnumerable<AOrmObject> result = null;

            foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
            {
                var searchObjects =
                    SearchObjects(
                        objectType,
                        plugins,
                        tableOrm => tableOrm.FulltextSearch(name));

                if (searchObjects == null)
                    continue;

                if (result == null)
                {
                    result = searchObjects;
                    continue;
                }

                result = result.Concat(searchObjects);
            }

            return result == null ? null : new LinkedList<AOrmObject>(result);
        }

        public IEnumerable<AOrmObject> GetParameterSearchResult(string name, ObjectType? objectType, ICollection<string> plugins)
        {
            if (objectType != null)
            {
                var searchObjects = DoGetParameterSearchResult(name, objectType.Value, plugins);
                return searchObjects == null ? null : new LinkedList<AOrmObject>(searchObjects);
            }

            IEnumerable<AOrmObject> result = null;

            foreach (ObjectType actObjectType in Enum.GetValues(typeof(ObjectType)))
            {
                var searchObjects = DoGetParameterSearchResult(name, actObjectType, plugins);

                if (searchObjects == null)
                    continue;

                if (result == null)
                {
                    result = searchObjects;
                    continue;
                }

                result = result.Concat(searchObjects);
            }

            return result == null ? null : new LinkedList<AOrmObject>(result);
        }

        private IEnumerable<AOrmObject> DoGetParameterSearchResult(
            string name,
            ObjectType objectType,
            ICollection<string> plugins)
        {
            return
                SearchObjects(
                    objectType,
                    plugins,
                    tableOrm => tableOrm.ParametricSearch(name));
        }

        public IList<UserFoldersStructure> GetFolderStructureSearchResult(string text)
        {
            return UserFoldersStructures.Singleton.FolderStructureSearch(text);
        }

        #endregion

        public bool TestSendMailPresentationGroup(PresentationGroup pg, string message, out Exception error)
        {
            PerformPresentationGroup ppg = new PerformPresentationGroup(pg);
            return PerformPresentationProcessor.Singleton.ProcessSendTestMail(ppg, message, out error);
        }

        public string GetLicencePath()
        {
            return LicenseHelper.Singleton.LicenceBlock == null ? "" : LicenseHelper.Singleton.Path;
        }

        public DateTime GetLicenceExpirationDate()
        {
            return LicenseHelper.Singleton.LicenceBlock == null ? DateTime.MinValue : LicenseHelper.Singleton.LicenceBlock.ExpDate;
        }

        public bool CheckTimetecLicense()
        {
#if !DEBUG
            string localisedName;
            object licenseValue;
            if (!GetLicensePropertyInfo("TimetecMaster", out localisedName, out licenseValue) || !(bool)licenseValue)
            {
                return false;
            }
#endif
            return true;
        }

        public Dictionary<string, string> GetLicenceProperies()
        {
            return LicenseHelper.Singleton.LicenceBlock == null ? new Dictionary<string, string>() : LicenseHelper.Singleton.LicenceBlock.GetLicenceBlockProperties();
        }

        public bool GetLicensePropertyInfo(string propertyName, out string localisedName, out object value)
        {
            return CgpServer.Singleton.GetLicencePropertyInfoFromServerAndPlugins(propertyName, out localisedName, out value);
        }

        public DHCPGroups GetServerDHCPGroups()
        {
            return CgpServer.Singleton.DhcpServer.Load(QuickPath.AssemblyStartupPath + CgpServer.Singleton.DHCPServerFile) ? CgpServer.Singleton.DhcpServer.DHCPGroups : new DHCPGroups();
        }

        public string GetServerIpAddress(IPAddress networkAddress, IPAddress networkMask)
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (IPHelper.IsSameNetwork4(networkAddress.ToString(), networkMask.ToString(), addr.Address.ToString()))
                        {
                            return addr.Address.ToString();
                        }
                    }
                }
                return string.Empty;
            }
            catch
            {
                return null;
            }
        }

        public void SetDHCPGroups(DHCPGroups dhcpGroups)
        {
            CgpServer.Singleton.DhcpServer.DHCPGroups = dhcpGroups;
            CgpServer.Singleton.DhcpServer.Save(QuickPath.AssemblyStartupPath + CgpServer.Singleton.DHCPServerFile);
        }

        public bool RemoveDHCPGroup(DHCPGroup dhcpGroup)
        {
            bool result = CgpServer.Singleton.DhcpServer.DHCPGroups.Delete(dhcpGroup);
            CgpServer.Singleton.DhcpServer.Save(QuickPath.AssemblyStartupPath + CgpServer.Singleton.DHCPServerFile);
            return result;
        }

        /// <summary>
        /// Starts DHCP and sets AutoStartDHCP property
        /// </summary>
        /// <param name="auto"></param>
        /// <exception cref="DoesNotExistException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="IOException"></exception>
        public void AutoStartDHCP(bool auto)
        {
            try
            {
                CgpServer.Singleton.AutoStartDHCP = auto;
                GeneralOptions generalOptions = GeneralOptions.Singleton;
                generalOptions.AutoStartDHCP = auto;
                generalOptions.SaveAutoStartDHCP();
            }
            catch (Exception ex)
            {

            }
        }

        public int GenerateLicenceRequestFile(Dictionary<int, string> propertyNames, Dictionary<int, string> propertyValues,
            string commonName, string CustomerName, string customerAddress, string contactFirstName, string contactSecondName,
            string contactPhone, string contactEmail, string publisherName, string publisherKey, string filePath)
        {
            try
            {
                List<ISLAPropertyRecord> propertyRecords = LicenceRequest.Singleton.CreatePropertyRecords(propertyNames, propertyValues);

                ISLACustomerRecord customerRecord = LicenceRequest.Singleton.CreateCustomerRecord(commonName, CustomerName,
                    customerAddress, contactFirstName, contactSecondName, contactPhone, contactEmail);

                ISLAPublisher publisher = LicenceRequest.Singleton.CreatePublisher(publisherName, publisherKey, filePath);

                if (propertyRecords != null && customerRecord != null && publisher != null)
                {
                    return LicenceRequest.Singleton.CreateLicenceRequestFile(customerRecord, publisher, propertyRecords);
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public bool InsertLicenceFile(string fileName, byte[] fileBytes, out bool isValid, out bool needRestart)
        {
            string filePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            isValid = false;
            needRestart = false;
            string actualLicencePath = LicenseHelper.Singleton.Path;

            try
            {
                File.WriteAllBytes(filePath, fileBytes);
                LicenseHelper.Singleton.CheckLicence(filePath);
                isValid = LicenseHelper.Singleton.IsValid;

                if (!isValid)
                {
                    try
                    {
                        LicenseHelper.Singleton.CheckLicence(actualLicencePath);
                        File.Delete(filePath);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                    return true;
                }

                LicenseHelper.Singleton.SetServerAndPluginsRequiredLicenceProperties();
                var rk = RegistryHelper.GetOrAddKey(CgpServerGlobals.REGISTRY_GENERAL_SETTINGS);
                rk.SetValue(CgpServerGlobals.CGP_SERVER_LICENCE_PATH, filePath, RegistryValueKind.String);
                needRestart = CgpServer.Singleton.NeedRestart;

                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                HandledExceptionAdapter.Examine(ex);
                return false;
            }
        }

        public bool LoadLicenceRequestFields(out string publisherName, out string publisherKey, out string commonName,
            out Dictionary<int, string> propertyNames,
            out Dictionary<int, string> propertyTypes, out Dictionary<int, string> propertyValues,
            out Dictionary<int, string> propertyDescriptions)
        {
            return LicenceRequest.Singleton.LoadPublisherAndDescriptiveFile(out publisherName, out publisherKey, out commonName,
                out propertyNames, out propertyTypes, out propertyValues, out propertyDescriptions);
        }

        public bool GetAutoDHCPStatus()
        {
            return CgpServer.Singleton.AutoStartDHCP;
        }

        public bool StartDHCP(bool toStart)
        {
            return CgpServer.Singleton.StartDHCP(toStart);
        }

        public bool GetDHCPRunningState()
        {
            return CgpServer.Singleton.DhcpServer.IsRunning;
        }

        public DateTime GetServerDateTime()
        {
            return DateTime.Now;
        }

        public TimeZoneInfo GetServerTimeZoneInfo()
        {
            return TimeZoneInfo.Local;
        }

        public DateTime GetServerDemoStartTime()
        {
            return CgpServer.Singleton.DemoStartTime;
        }

        public double GetServerDemoMaxTime()
        {
            return LicenseHelper.DEMO_MINUTES_WORKING * 60 * 1000;
        }

        public bool ForceDatabaseBackup(string path)
        {
            return DatabaseBackup.Singleton.ForceDatabaseBackup(path);
        }

        public bool ForceEventlogCleaning(int expiratedDays, int maxRecords)
        {
            return EventlogExpiration.Singleton.ForceEventlogExpitationCleaning(expiratedDays, maxRecords);
        }

        public string DatabaseBackupFileName()
        {
            return DatabaseBackup.Singleton.BackupFileName;
        }

        public int GetSessionTimeout()
        {
            return RemotingSessionHandler.Singleton.SessionTimeout;
        }

        public Exception ClientAuthenticate(ARemotingAuthenticationParameters parameters)
        {
            try
            {
                Authenticate(parameters);
                return null;
            }
            catch (Exception error)
            {
                return error;
            }
        }

        public Exception ClientAutenticateWithCard(string card, out string loginName)
        {
            try
            {
                LoginAuthenticationParameters lap = DB.Logins.Singleton.GetAuthParamFromCard(card);
                if (lap != null)
                {
                    loginName = lap.LoginUsername;
                    Authenticate(lap);
                }
                else
                {
                    loginName = string.Empty;
                    throw (new InvalidSecureEntityException(LoginAuthenticationParameters.WRONG_CARD, card));
                }
                return null;
            }
            catch (Exception error)
            {
                loginName = string.Empty;
                return error;
            }
        }

        public bool ReturnIsPinCardLoginRequiered()
        {
            if (GeneralOptions.Singleton.RequirePINCardLogin != null)
            {
                return (bool)GeneralOptions.Singleton.RequirePINCardLogin;
            }
            else
            {
                return false;
            }
        }

        public byte GetCardPinLengthByFullCardNumber(string cardNumber)
        {
            try
            {
                Card card = DB.Cards.Singleton.GetCardByFullNumber(cardNumber);
                if (card == null || (card.State != (byte)CardState.Active && card.State != (byte)CardState.HybridActive))
                {
                    return 0;
                }
                else
                {
                    return card.PinLength;
                }
            }
            catch
            {
                return 0;
            }
        }

        public bool IsValidPinForCard(string cardNumber, string hashedPin)
        {
            try
            {
                Card card = DB.Cards.Singleton.GetCardByFullNumber(cardNumber);
                if (card == null)
                {
                    return false;
                }
                else
                {
                    return (card.Pin == hashedPin);
                }
            }
            catch
            {
                return false;
            }
        }

        public string GetLoginLanguage()
        {
            Login login = AccessChecker.GetActualLogin();
            if (login == null || login.ClientLanguage == null)
                return string.Empty;
            return login.ClientLanguage;
        }

        public void SetLoginLanguage(string language)
        {
            Login login = AccessChecker.GetActualLogin();
            if (login == null)
                return;
            login = DB.Logins.Singleton.GetObjectForEdit(login.IdLogin);
            if (login == null)
                return;
            login.ClientLanguage = language;
            DB.Logins.Singleton.Update(login);
        }

        //public Login GetLogin(string username)
        //{
        //    if (username == null || username == string.Empty)
        //        return null;
        //    return DB.Logins.Singleton.GetLoginByUserName(username);
        //}

        public void SaveUserOpenedWindows(List<UserOpenedWindow> userOpenedWindows, string userName)
        {
            Login login = DB.Logins.Singleton.GetLoginByUserName(userName);

            if (login == null)
                return;

            login = DB.Logins.Singleton.GetObjectForEdit(login.IdLogin);
            login.UserOpenedWindows = userOpenedWindows;
            DB.Logins.Singleton.Update(login);
            DB.Logins.Singleton.EditEnd(login);
        }

        //public void SaveRecentMnuItems(string username, Dictionary<string, int> _recentMenuItems)
        //{
        //    if (username == null || username == string.Empty)
        //        return;
        //    Login login = DB.Logins.Singleton.GetById(username);
        //    if (login == null)
        //        return;
        //    login = DB.Logins.Singleton.GetObjectForEdit(login.Username);
        //    login.QuickMenu = _recentMenuItems;
        //    DB.Logins.Singleton.Update(login);
        //    DB.Logins.Singleton.EditEnd(login);
        //}

        public ICollection<UserOpenedWindow> GetUserOpenedWindows()
        {
            Login login = AccessChecker.GetActualLogin();
            if (login == null)
                return null;
            return login.UserOpenedWindows;
        }

        public bool NovaClientUpgradeAvailable(Version callerVersion, out Version newVersion)
        {
            return CgpServer.Singleton.ClientUpgradeAvailable(callerVersion, out newVersion);
        }

        public byte[] GetClientUpgradeData(Version upgradeversion, int fromFilePosition)
        {
            return CgpServer.Singleton.GetClientUpgradeData(upgradeversion, fromFilePosition);
        }

        public long GetClientUpgradeFileLength(Version upgradeVersion)
        {
            return CgpServer.Singleton.GetClientUpgradeFileLength(upgradeVersion);
        }

        public byte[] GetLastEventlogReport(int fromFilePosition)
        {
            return CgpServer.Singleton.GetLastEventlogReport(fromFilePosition);
        }

        public bool IsLastEventlogReportAvailable()
        {
            return CgpServer.Singleton.IsLastEventlogReportAvailable();
        }

        public void CleanLastEventlogReport()
        {
            CgpServer.Singleton.CleanLastEventlogReport();
        }

        public Dictionary<string, Type> GetAllRequiredLicenceProperties()
        {
            return CgpServer.Singleton.GetRequiredLicencePropertiesServerAndAllPlugins();
        }

        public bool GetLicencePropertyLocalisedName(string propertyName, string language, out string localisedPropertyName)
        {
            return CgpServer.Singleton.GetLocalisedLicencePropertyNameServerAndAllPlugins(propertyName, language, out localisedPropertyName);
        }

        private bool _generating = false;
        public bool GenerateCards(
            string fullPrefix,
            byte cardSystem,
            int numberDigits,
            decimal fromNumber,
            decimal toNumber,
            string pin,
            CardState cardState,
            int? idStructuredSubSite)
        {
            if (_generating == true)
                return false;
            _generating = true;
            try
            {
                CardGenerationSettings settings = new CardGenerationSettings(
                    fullPrefix,
                    cardSystem,
                    numberDigits,
                    fromNumber,
                    toNumber,
                    pin,
                    cardState,
                    idStructuredSubSite);

                SafeThread<CardGenerationSettings>.StartThread(
                    GenerateCardsThread,
                    settings);
            }
            catch
            {
                _generating = false;
            }
            return true;
        }

        private void GenerateCardsThread(CardGenerationSettings settings)
        {
            try
            {
                int insertedCount = 0;
                CardSystem cs = DB.CardSystems.Singleton.GetCardSystemFromNumber(settings.CardSystem);
                for (decimal i = settings.FromNumber; i <= settings.ToNumber; i++)
                {
                    try
                    {
                        var numberString = i.ToString();
                        numberString = numberString.PadLeft(settings.NumberDigits, '0');
                        Card newCard = new Card();
                        newCard.CardSystem = cs;
                        newCard.Number = numberString;
                        newCard.FullCardNumber = settings.FullPrefix + numberString;
                        newCard.Pin = settings.Pin;
                        newCard.PinLength = (byte)settings.Pin.Length;
                        newCard.State = (byte)settings.CardState;

                        newCard.DateStateLastChange = DateTime.Now;
                        newCard.UtcDateStateLastChange = DateTime.UtcNow;

                        if (DB.Cards.Singleton.InsertOnlyInDatabase(
                            ref newCard,
                            settings.IdSubSite))
                        {
                            insertedCount++;
                        }
                    }
                    catch
                    {
                    }
                }
                ForeachCallbackHandler(RunCardGenerionFinishedEvent, DelegateSequenceBlockingMode.Asynchronous, false, cs.IdCardSystem, insertedCount);
            }
            catch { }
            finally
            {
                _generating = false;
            }
        }

        private void RunCardGenerionFinishedEvent(ARemotingCallbackHandler remoteHandler, object[] values)
        {
            if (values.Length != 2 || !(values[0] is Guid) || !(values[1] is int))
                return;
            try
            {
                if (remoteHandler is CardGenerationFinisheEventHandler)
                    (remoteHandler as CardGenerationFinisheEventHandler).RunEvent((Guid)values[0], (int)values[1]);
            }
            catch
            {
                throw;
            }
        }

        public IList<string> GetServerDirectory(string path)
        {
            IList<string> result = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo di in allDrives)
                    {
                        result.Add(di.Name);
                    }
                }
                else
                {

                    string[] directories = Directory.GetDirectories(path);
                    foreach (string dir in directories)
                    {
                        result.Add(dir);
                    }
                }
            }
            catch { }
            return result;
        }

        public bool ExistFileOnServer(string fileName)
        {
            return File.Exists(fileName);
        }

        public bool DirectoryExitsOnServer(string directoryName)
        {
            return Directory.Exists(directoryName);
        }

        public string[] GetAllLanguages()
        {
            return CgpServer.Singleton.LocalizationHelper.AllLanguages;
        }

        public string GetCurrentLanguage()
        {
            return CgpServer.Singleton.LocalizationHelper.ActualLanguage;
        }

        public void SetServerLanguage(string language)
        {
            CgpServer.Singleton.SetServerAndPluginsLanguage(language);
            GeneralOptions.Singleton.CurrentLanguage = language;
            GeneralOptions.Singleton.SaveSettingsToRegistry();
        }

        /// <summary>
        /// Return server informations in dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetServerInformations()
        {
            return ServerInformations.GetServerInformations();
        }

        public AOrmObject GetTableObject(ObjectType objectType, string strObjectId)
        {
            if (objectType == ObjectType.Eventlog)
            {
                long eventlogId;

                return
                    Int64.TryParse(strObjectId, out eventlogId)
                        ? DB.Eventlogs.Singleton.GetObjectById(eventlogId)
                        : null;
            }

            ITableORM tableOrm = GetTableOrmForObjectType(objectType);

            if (tableOrm == null)
                return null;

            var objectId = tableOrm.ParseId(strObjectId);

            return
                objectId != null
                    ? tableOrm.GetObjectById(objectId)
                    : null;
        }

        public ICollection<IModifyObject> GetModifyObjects(
            ICollection<IdAndObjectType> idAndObjectTypes)
        {
            if (idAndObjectTypes == null)
                return null;

            var modifyObjects = new LinkedList<IModifyObject>();

            foreach (var idAndObjectType in idAndObjectTypes)
            {
                ITableORM tableOrm = GetTableOrmForObjectType(idAndObjectType.ObjectType);

                var ormObject = tableOrm != null
                    ? tableOrm.GetObjectById(idAndObjectType.Id)
                    : null;

                var modifyObject = ormObject != null
                    ? ormObject.CreateModifyObject()
                    : null;

                if (modifyObject == null)
                    continue;

                modifyObjects.AddLast(modifyObject);
            }

            return modifyObjects;
        }

        public ITableORM GetTableOrmForObjectType(ObjectType objectType)
        {
            return
                ObjectTypeTablesORM.GetTableOrm(objectType) ??
                    CgpServer.Singleton.PluginManager
                        .GetLoadedPlugins()
                        .Select(plugin => plugin.GetTableOrmForObjectType(objectType))
                        .FirstOrDefault(tableOrm => tableOrm != null);
        }

        #region ICgpServerRemotingProvider Testing Members


        public void StopDBTest()
        {
            CgpServer.Singleton.StopTestingThred();
        }

        public void RunDBTest()
        {
            CgpServer.Singleton.StartTestingThread();
        }

        #endregion

        protected override IEnumerable<string> GetSessionsForUser(string userIdentifier)
        {
            try
            {
                Guid userId = new Guid(userIdentifier);
                return RemotingAuthentication
                    .GetSessionsForUser(userId).Select(item => item.ToString());
            }
            catch (FormatException)
            {
                return new string[0];
            }
            catch (OverflowException)
            {
                return new string[0];
            }

        }

        public ICollection<IModifyObject> GetIModifyObjects()
        {
            var modifyObject = Enumerable.Empty<IModifyObject>();

            foreach (ObjectType objectType in Enum.GetValues(typeof(ObjectType)))
            {
                var tableOrm = GetTableOrmForObjectType(objectType);

                if (tableOrm != null)
                {
                    var mo = new LinkedList<IModifyObject>(tableOrm.GetIModifyObjects());
                    modifyObject = modifyObject.Concat(tableOrm.GetIModifyObjects());
                }
            }

            return new LinkedList<IModifyObject>(modifyObject);
        }

        public ICollection<ServerAlarmCore> GetAlarms(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
        {
            return AlarmsManager.Singleton.GetAlarms(
                alarmType,
                alarmObject);
        }
    }
}
