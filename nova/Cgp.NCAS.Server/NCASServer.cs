using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

using Contal.CatCom;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Globals.PlatformPC;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Alarms;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.Timetec;
using Contal.Cgp.ORM;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.Drivers.CardReader;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Net;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.LwRemoting2;
using Contal.LwSerialization;

using Calendar = Contal.Cgp.Server.Beans.Calendar;
using RequiredLicenceProperties = Contal.Cgp.NCAS.Globals.RequiredLicenceProperties;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;
using Contal.IwQuick.Data;
using Contal.Cgp.NCAS.Server.Notifications;
using Contal.Cgp.NCAS.Server.Reports;

namespace Contal.Cgp.NCAS.Server
{
    public class NCASServer : ACgpServerPlugin
    {
        //*************************TEMPORARY CODE
        private static volatile NCASServer _singleton;
        private static readonly object syncRoot = new object();

        private Dictionary<string, int> _avaibleCeUpgrades;

        // this temporary solution is due 
        // automated instantiation via ACgpPlugin reflection mechanism
        public NCASServer()
        {
            lock (syncRoot)
            {
                if (null == _singleton)
                    _singleton = this;
            }
        }

        public static new NCASServer Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (syncRoot)
                    {
                        if (_singleton == null)
                            // ReSharper disable once ObjectCreationAsStatement
                            new NCASServer();
                    }
                return _singleton;
            }
        }

        //*************************TEMPORARY CODE

        private readonly Dictionary<string, object> _requiredLicenceProperties = new Dictionary<string, object>();
        public const string CCU_UPGRADES_DIRECTORY_NAME = "CCU Upgrades";
        public const string DCU_UPGRADES_DIRECTORY_NAME = "DCU Upgrades";
        public const string CE_UPGRADES_DIRECTORY_NAME = "CE Upgrades";
        public const string CR_UPGRADES_DIRECTORY_NAME = "Card Reader Upgrades";
        public const string REPORTS_DIRECTORY_NAME      = "Reports";
        /// <summary>
        /// minimal version of CE on CCU until not considered upgrade only
        /// </summary>
        public static readonly int MINIMAL_FIRMWARE_VERSION_FOR_CE = 3082;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_CE_CHECKING = ExtendedVersion.CreateSimple(2, 0, 5553);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_TCP_DATA_CHANNEL = ExtendedVersion.CreateSimple(1, 9, 4888);

        /// <summary>
        /// minimal version of CCU until not considered upgrade only
        /// </summary>
        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_CCU = ExtendedVersion.CreateSimple(2, 2, 5012);



        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_DCU = ExtendedVersion.CreateSimple(1, 9, 1707);
        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_RS485_DCU = ExtendedVersion.CreateSimple(1, 7, 1610);

        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_INTER_CCU_COMMUNICATION = ExtendedVersion.CreateSimple(1, 8, 4639);

        public static readonly Version MINIMAL_FIRMWARE_VERSION_FOR_WAITING_WHILE_INCOMING_STREAM_IS_IN_PROCESSING = ExtendedVersion.CreateSimple(1, 9, 5219);

        //***********************************************

        //private string _maximalFirmwareVersionForCCU = string.Empty;
        public Version MaximalFirmwareVersionForCCU
        {
            get
            {
                return Version;
            }
        }


        private readonly ExtendedVersion _version = new ExtendedVersion(typeof(NCASServer), true,
#if DEBUG
 DevelopmentStage.Testing
#else
#if RELEASE_SPECIAL
        DevelopmentStage.Testing
#else
        DevelopmentStage.Alpha
#endif
#endif
);

        //private SecurityTimeAxis _securityTimeAxis;

        public override string FriendlyName
        {
            get { return "NCAS plugin"; }
        }

        public override string Description
        {
            get { return String.Empty; }
        }

        public override string[] FriendPlugins
        {
            get { return null; }
        }

        public override ExtendedVersion Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Helper for localization
        /// </summary>
        private LocalizationHelper _localizationHelper = null;
        public LocalizationHelper LocalizationHelper { get { return _localizationHelper; } }

        public override void OnDispose()
        {

        }

        private DbWatcher _dbWatcher = null;

        private AlarmsManager _alarmsManager = null;
        public AlarmsManager GetAlarmsQueue()
        {
            if (_alarmsManager == null)
            {
                var alarmsQueue =
                    _serverDomain.CreateInstanceAndUnwrap(
                    typeof(AlarmsManager).Assembly.ToString(),
                    typeof(AlarmsManager).FullName) as AlarmsManager;

                if (alarmsQueue == null)
                    throw new SystemException("Unable to obtain AlarmsQueue instance");

                _alarmsManager = alarmsQueue.GetSingleton();
                _alarmsManager.ChangedAlarm += AlarmsManagerChangedAlarm;
            }

            return _alarmsManager;
        }

        void AlarmsManagerChangedAlarm(ServerAlarm serverAlarm, bool wasCreatedOrChangedAlarmState)
        {
            if (serverAlarm.ServerAlarmCore.IsBlocked)
                return;

            //find presentation group by type alarm object 
            var pg = GetPresentationGroupForAlarm(serverAlarm);

            //if presentation group exists perform it
            if (pg != null && (wasCreatedOrChangedAlarmState || pg.ResponseDateTimeUpdate))
            {
                PerformPresentationGroup(serverAlarm, pg);
            }

            serverAlarm.ServerAlarmCore.PresentationGruopId = pg != null ? pg.IdGroup : Guid.Empty;
        }

        private PresentationGroup GetPresentationGroupForAlarm(ServerAlarm serverAlarm)
        {
            var iGetPresentationGroup = serverAlarm as IGetPresentationGroup;

            var pg = iGetPresentationGroup != null
                ? iGetPresentationGroup.GetPresentationGroup()
                : null;

            if (pg == null)
                return null;

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            var alarmState = alarm.AlarmState;
            var isAcknowledge = alarm.IsAcknowledged;

            if (pg.ResponseAlarm && alarmState == AlarmState.Alarm && isAcknowledge)
            {
                return pg;
            }
            if (pg.ResponseAlarmNotAck && alarmState == AlarmState.Alarm && !isAcknowledge)
            {
                return pg;
            }
            if (pg.ResponseNormal && alarmState == AlarmState.Normal && isAcknowledge)
            {
                return pg;
            }
            if (pg.ResponseNormalNotAck && alarmState == AlarmState.Normal && !isAcknowledge)
            {
                return pg;
            }

            return null;
        }

        private void PerformPresentationGroup(ServerAlarm serverAlarm, PresentationGroup pg)
        {
             SafeThread<PresentationGroup, string>.StartThread(
                PGProcessSendMessage,
                pg,serverAlarm);
        }

        public void PresentationGroupChanged(AlarmType alarmType)
        {
            PresentationGroupChanged(_alarmsManager.FindAlarmsByAlarmType(alarmType));
        }


        public void PresentationGroupChanged(AlarmType alarmType, ObjectType alarmObjectType, Guid alarmObjectId)
        {
            PresentationGroupChanged(_alarmsManager.FindAlarmsByAlarmType(alarmType, alarmObjectType, alarmObjectId));
        }

        private void PresentationGroupChanged(ICollection<ServerAlarm> serverAlarms)
        {
            if (serverAlarms != null && serverAlarms.Count > 0)
            {
                foreach (var alarm in serverAlarms)
                {
                    var pg = GetPresentationGroupForAlarm(alarm);
                    if (pg != null && alarm.ServerAlarmCore.PresentationGruopId != pg.IdGroup)
                    {
                        PerformPresentationGroup(alarm, pg);
                    }

                    alarm.ServerAlarmCore.PresentationGruopId = pg != null ? pg.IdGroup : Guid.Empty;
                }
            }
        }

        private readonly object _lockPGProcessSendMessage = new object();
        /// <summary>
        /// Send message to presentation group
        /// </summary>
        /// <param name="pg"></param>
        /// <param name="msg"></param>
        public void PGProcessSendMessage(PresentationGroup pg, string msg)
        {
            if (pg != null)
            {
                lock (_lockPGProcessSendMessage)
                {
                    var ppg = new PerformPresentationGroup(pg);
                    PerformPresentationProcessor.Singleton.ProcessSendMessage(
                        ppg,
                        msg);
                }
            }
        }

        public void PGProcessSendMessage(PresentationGroup pg, ServerAlarm serverAlarm)
        {
            if (pg != null)
            {
                lock (_lockPGProcessSendMessage)
                {
                    string acknowledgeState = !serverAlarm.ServerAlarmCore.Alarm.IsAcknowledged ? "Not acknowledged" : "Acknowledged";

                    var ppg = new PerformPresentationGroup(pg);
                    string msg=
                    string.Format("{0} - {1}/{2}",
                     serverAlarm.GetPresentationDescription(),
                     serverAlarm.ServerAlarmCore.Alarm.AlarmState,
                     acknowledgeState);

                    PerformPresentationProcessor.Singleton.ProcessSendMessage(
                        ppg,
                        msg);
                }
            }
        }

        private DateTime? _initializeDateTime = null;

        // SB
        private EmailNotificationService emailNotificationService = new EmailNotificationService();


        public override void Initialize()
        {
            _initializeDateTime = DateTime.Now;
            UserFoldersStructureObjects.Singleton.EventPersonDepartmentChanged += PersonDepartmentChanged;

            TypeCache.LoadAssemblies(
                GetType().Assembly,
                typeof(AlarmArea).Assembly,
                typeof(BlockFileInfo).Assembly,
                typeof(LwRemotingMessage).Assembly);

            // SB
            emailNotificationService.Initialize(GeneralOptions.Singleton);
        }

        private static void PersonDepartmentChanged(Guid personId, PersonDepartmentChange personDepartmentChange)
        {
            ACLGroups.Singleton.PersonDepartmentChanged(personId, personDepartmentChange);
        }

        private DateTime? _loadSuccessfulDateTime = null;
        public override void LoadSuccessful()
        {
            _loadSuccessfulDateTime = DateTime.Now;
            _localizationHelper = new LocalizationHelper(GetType().Assembly);
            _localizationHelper.SetLanguage("English");
        }

        public override Type GetRemotingProviderInterfaceType()
        {
            return typeof(ICgpNCASRemotingProvider);
        }

        public override void EnsureUpgradeDirectories()
        {
            QuickPath.EnsureDirectory(_ccuUpgradesPath);
            QuickPath.EnsureDirectory(_ceUpgradesPath);
            QuickPath.EnsureDirectory(_dcuUpgradesPath);
            QuickPath.EnsureDirectory(_crUpgradesPath);
        }

        private void InsertEventlog(DateTime dateTime, string description)
        {
            Eventlogs.Singleton.InsertEvent(
                Eventlog.TYPEINITIALIZEPLUGIN,
                dateTime,
                GetType().Assembly.GetName().Name,
                null,
                description,
                new EventlogParameter(EventlogParameter.TYPEPLUGINNAME, "NCASServer"));
        }

        public override void InitDatabaseDefaults()
        {
            CatAlarmsManager.Singleton.Init();

            DevicesAlarmSettings.Singleton.ConfigureGeneralAlarmArcs();
            CCUs.Singleton.ConfigureSpecificAlarmArcs();
            DCUs.Singleton.ConfigureSpecificAlarmArcs();
            CardReaders.Singleton.ConfigureSpecificAlarmArcs();

            TimetecSettings.Singleton.CreateIfNotExists();
            TimetecCore.Singleton.Init();

            if (_initializeDateTime != null)
            {
                SafeThread<DateTime, string>.StartThread(InsertEventlog, _initializeDateTime.Value, "Inicialize plugin NCASServer");
            }

            if (_loadSuccessfulDateTime != null)
            {
                SafeThread<DateTime, string>.StartThread(InsertEventlog, _loadSuccessfulDateTime.Value, "Load plugin NCASServer succesful");
            }

            var ccus = CCUs.Singleton.List();

            if (ccus != null)
            {
                foreach (var ccu in ccus)
                {
                    var ccuAlarmsOwner = new CcuAlarmsOwner(ccu.IdCCU);

                    GetAlarmsQueue().RegisterAlarmsOwner(
                        ccu.IdCCU,
                        ccuAlarmsOwner);

                    ccuAlarmsOwner.LoadServerAlarmsFromDatabase();
                }
            }

            DCUs.RemoveDCUsWithNullReferenceForCCU();
            DevicesAlarmSettings.Singleton.CreateDefaultSettings();
            CCUAlarms.Singleton.Init();
            CCUConfigurationHandler.Singleton.Init();
            CCUConfigurationHandler.Singleton.NcasServer = this;
            CCUConfigurationHandler.Singleton.ConnectCCUs();
            SecurityTimeAxis.Singleton.StartOrReset();

            var watcher = _serverDomain.CreateInstanceAndUnwrap(typeof(DbWatcher).Assembly.ToString(), typeof(DbWatcher).FullName) as DbWatcher;

            if (watcher == null)
                throw new SystemException("Unable to obtain DbWatcher instance");

            _dbWatcher = watcher.GetSingleton();
            _dbWatcher.CgpDBObjectChanged += _dbWatcher_CgpDBObjectChanged;
            _dbWatcher.CgpDBPersonBeforeUD += _dbWatcher_CgpDBPersonBeforeUD;
            _dbWatcher.CgpDBPersonAfterUpdate += _dbWatcher_CgpDBPersonAfterUpdate;
            _dbWatcher.CgpDBPersonChanged += _dbWatcher_CgpDBPersonChanged;
            _dbWatcher.CgpDBCardChanged += _dbWatcher_CgpDBCardChanged;
            _dbWatcher.CgpDBTimeZoneChanged += _dbWatcher_CgpDBTimeZoneChanged;
            _dbWatcher.CgpGeneralNtpSettignsChanged += _dbWatcher_CgpGeneralNtpSettignsChanged;
            AlterDCU();
            AlterCentralNameRegisterRepairPersons();
            SetDefaultNTPTimeDiffTolerance();
            SafeThread.StartThread(DeleteRecordsWithNullReferences, ThreadPriority.BelowNormal);
            ACLGroups.Singleton.AddEventOnObjectMoved();

            AlarmTransmitters.Singleton.ValidateAlarmTransmitters();

            //TimerManager.Static.StartTimeout(3000, RunSpecificStuff1);
            //TimerManager.Static.StartTimeout(10000, RunSpecificStuff2);

            Cards.Singleton.InvalidPinRetriesLimitReachedTimeout =
                TimeSpan.FromMinutes(
                    DevicesAlarmSettings.Singleton.InvalidPinRetriesLimitReachedTimeout);

            Cards.Singleton.StartTemporaryBlocks();
            PersonAttributesReportService.Singleton.Start();
            DoorEnviromentAlarmsReportService.Singleton.Start();
        }

        private void DeleteRecordsWithNullReferences()
        {
            try
            {
                var objCount = RunSqlCommandScalar("SELECT COUNT(IdAAInput) FROM AAInput where Input is NULL");
                if (objCount is int)
                {
                    var count = (int)objCount;
                    if (count > 0)
                    {
                        RunSqlCommand("DELETE FROM AAInput where Input is NULL");
                    }
                }

                objCount = RunSqlCommandScalar("SELECT COUNT(IdAACardReader) FROM AACardReader where CardReader is NULL");
                if (objCount is int)
                {
                    var count = (int)objCount;
                    if (count > 0)
                    {
                        RunSqlCommand("DELETE FROM AACardReader where CardReader is NULL");
                    }
                }

                objCount = RunSqlCommandScalar("SELECT COUNT(IdACLPerson) FROM ACLPerson where AccessControlList is NULL or Person is NULL");
                if (objCount is int)
                {
                    var count = (int)objCount;
                    if (count > 0)
                    {
                        RunSqlCommand("DELETE FROM ACLPerson where AccessControlList is NULL or Person is NULL");
                    }
                }

                objCount = RunSqlCommandScalar("SELECT COUNT(IdACLSettingAA) FROM ACLSettingAA where AccessControlList is NULL or AlarmArea is NULL");
                if (objCount is int)
                {
                    var count = (int)objCount;
                    if (count > 0)
                    {
                        RunSqlCommand("DELETE FROM ACLSettingAA where AccessControlList is NULL or AlarmArea is NULL");
                    }
                }
            }
            catch { }
        }

        private void SetDefaultNTPTimeDiffTolerance()
        {
            var connection = new SqlConnection();
            try
            {
                connection.ConnectionString = NhHelper.Singleton.ConnectionString.ToString();
                connection.Open();

                RunSqlCommand("UPDATE ServerGeneralOptionsDB set NTPTimeDiffTolerance=120000 where NTPTimeDiffTolerance=0");
            }
            catch { }
            finally
            {
                connection.Close();
            }
        }

        private void AlterCentralNameRegisterRepairPersons()
        {
            var connection = new SqlConnection();
            try
            {
                connection.ConnectionString = NhHelper.Singleton.ConnectionString.ToString();
                connection.Open();

                if (SelectCount(connection, "SELECT COUNT(Id) from CentralNameRegister where ObjectType='0' and Id in (select IdPerson from Person)") > 0)
                {
                    RunSqlCommand("UPDATE CentralNameRegister set ObjectType=27 where Id in (select IdPerson from Person) and ObjectType=0");
                    var dataSet = GetDataSet(connection, "SELECT Id from CentralNameRegister where ObjectType='27' and CkUnique='00000000-0000-0000-0000-000000000000'");
                    UpdateCentralNameRegisterBadCkUnique(connection, dataSet);
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }
        }

        private void AlterDCU()
        {
            try
            {
                var command = "UPDATE DCU set InputsCount = (Select count(IdInput) from Input where Input.DCU = IdDCU) where InputsCount is null or InputsCount=0";
                RunSqlCommand(command);
                command = "UPDATE DCU set OutputsCount = (Select count(IdOutput) from Output where Output.DCU = IdDCU) where OutputsCount is null or OutputsCount=0";
                RunSqlCommand(command);
            }
            catch { }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool RunSqlCommand(string command)
        {
            var sqlConnection = new SqlConnection();
            try
            {
                sqlConnection.ConnectionString = NhHelper.Singleton.ConnectionString.ToString();
                sqlConnection.Open();
                var sqlQuery = command;
                var myCommand = new SqlCommand(sqlQuery, sqlConnection);
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private object RunSqlCommandScalar(string command)
        {
            var sqlConnection = new SqlConnection();
            try
            {
                sqlConnection.ConnectionString = NhHelper.Singleton.ConnectionString.ToString();
                sqlConnection.Open();
                var sqlQuery = command;
                var myCommand = new SqlCommand(sqlQuery, sqlConnection);
                return myCommand.ExecuteScalar();
            }
            catch
            {
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool RunSqlCommand(SqlConnection sqlConnection, string command)
        {
            try
            {
                var sqlQuery = command;
                var myCommand = new SqlCommand(sqlQuery, sqlConnection);
                myCommand.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void UpdateCentralNameRegisterBadCkUnique(SqlConnection connection, DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var guid = Guid.NewGuid();

                    //Check for guid duplicity
                    var maxCheckCount = 0;
                    while (SelectCount(connection, "SELECT COUNT(CkUnique) from CentralNameRegister where CkUnique='" + guid + "'") > 0)
                    {
                        if (maxCheckCount > 10) return;

                        guid = new Guid();
                        maxCheckCount++;
                    }
                    //
                    RunSqlCommand(connection, "UPDATE CentralNameRegister set CkUnique='" + guid + "' where Id='" + row.ItemArray[0] + "'");
                }
            }
        }

        /*
                private SqlConnection CreateConnection(string connectionString)
                {
                    SqlConnection connection = null;
                    try
                    {
                        connection = new SqlConnection(NhHelper.Singleton.ConnectionString.ToString());
                    }
                    catch { }

                    return connection;
                }
        */

        private DataSet GetDataSet(SqlConnection connection, string select)
        {
            try
            {
                var command = new SqlCommand(@select, connection);
                var adapter = new SqlDataAdapter();
                var ds = new DataSet();

                adapter.SelectCommand = command;
                adapter.Fill(ds);

                adapter.Dispose();
                command.Dispose();
                return ds;
            }
            catch { }

            return null;
        }

        private int SelectCount(SqlConnection connection, string select)
        {
            var count = 0;
            var command = new SqlCommand(select, connection);

            var reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    count = (int)reader[0];
                }
                reader.Close();
            }
            catch (Exception)
            {
                reader.Close();
            }
            return count;
        }

        private readonly Random _rand = new Random();

        protected IList<FilterSettings> _filterSettings = new List<FilterSettings>();

        public override void RunDBTest()
        {
            _filterSettings.Clear();
            Exception ex;
            var a = _rand.Next(10).ToString(CultureInfo.InvariantCulture);
            var b = _rand.Next(10).ToString(CultureInfo.InvariantCulture);
            //string c = rand.Next(10).ToString();
            var res = string.Empty;
            res += a;
            res += b;

            var filterSetting = new FilterSettings(Card.COLUMNFULLCARDNUMBER, res, ComparerModes.LIKEBOTH);
            _filterSettings.Add(filterSetting);

            //ICollection<Card> cards = Cards.Singleton.SelectByCriteria(_filterSettings);
            Console.WriteLine("Test1");
            Cards.Singleton.ListModifyObjects(out ex);
            Console.WriteLine("Test2");

            //switch (a)
            //{
            //    case 1:
            //        {
            //            CCUs.Singleton.ListModifyObjects(out ex);
            //        }
            //        break;
            //    case 2:
            //        {
            //            DCUs.Singleton.ListModifyObjects(out ex);
            //        }
            //        break;
            //    case 3:
            //        {
            //            Inputs.Singleton.ListModifyObjects(out ex);
            //        }
            //        break;
            //    case 4:
            //        {
            //            Outputs.Singleton.ListModifyObjects(out ex);
            //        }
            //        break;
            //    case 5:
            //        {
            //            Cards.Singleton.SelectByCriteria(new FilterSettings(Card.COLUMNFULLCARDNUMBER,"",ComparerModes.LIKEBOTH);
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        /*
                object _dbWatcher_CgpBeforeUD(object objectId, ObjectType objectType)
                {
                    return null;
                }
        */

        void _dbWatcher_CgpDBObjectChanged(object objectId, ObjectType objectType, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                var id = Guid.Empty;
                if (objectId is Guid)
                    id = (Guid)objectId;

                if (id != Guid.Empty)
                {
                    DataReplicationManager.Singleton.DeleteObjectFroCcus(
                        new IdAndObjectType(
                            id,
                            objectType));
                }
            }
            else
            {
                if (objectType == ObjectType.Calendar)
                {
                    _dbWatcher_CgpDBCalendarChanged(objectId);
                }
                else
                {
                    var ormObject = DataReplicationManager.LoadObjectFromDatabase(objectId, objectType);
                    if (ormObject != null)
                    {
                        var id = Guid.Empty;
                        if (objectId is Guid)
                            id = (Guid)objectId;

                        if (id != Guid.Empty)
                        {
                            DataReplicationManager.Singleton.SendModifiedObjectToCcus((AOrmObjectWithVersion)ormObject);
                        }
                    }
                }
            }
        }

        void _dbWatcher_CgpDBCalendarChanged(object objectId)
        {
            var idCalendar = Guid.Empty;
            if (objectId is Guid)
                idCalendar = (Guid)objectId;

            if (idCalendar != Guid.Empty)
            {
                SecurityTimeAxis.Singleton.StartOrReset();

                var calendar = DataReplicationManager.LoadObjectFromDatabase(idCalendar, ObjectType.Calendar) as Calendar;
                if (calendar != null)
                {
                    DataReplicationManager.Singleton.SendModifiedObjectToCcus(calendar);
                }
            }
        }

        void _dbWatcher_CgpDBPersonBeforeUD(Guid idPerson, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                var aclPersons = ACLPersons.Singleton.GetAssignedAclPersons(idPerson);
                if (aclPersons != null && aclPersons.Count > 0)
                {
                    foreach (var aclPerson in aclPersons)
                    {
                        ACLPersons.Singleton.Delete(aclPerson);
                    }
                }

                var accessZones = AccessZones.Singleton.GetAssignedAccessZones(idPerson);
                if (accessZones != null && accessZones.Count > 0)
                {
                    foreach (var accessZone in accessZones)
                    {
                        AccessZones.Singleton.Delete(accessZone);
                    }
                }

                PersonAttributes.Singleton.DeletePersonAttributes(idPerson);
            }
        }

        void _dbWatcher_CgpDBPersonAfterUpdate(
            Person newPerson,
            Person oldPersonBeforeUpdate)
        {
            if (!newPerson.EmploymentEndDate.HasValue
                && oldPersonBeforeUpdate.EmploymentEndDate.HasValue
                || newPerson.EmploymentEndDate.HasValue
                && !oldPersonBeforeUpdate.EmploymentEndDate.HasValue
                || !newPerson.EmploymentEndDate.Equals(
                    oldPersonBeforeUpdate.EmploymentEndDate))
            {
                ACLGroups.Singleton.PersonEmploymentEndDateChanged(newPerson.IdPerson);
                ACLPersons.Singleton.PersonEmploymentEndDateChanged(newPerson.IdPerson);
            }
        }

        void _dbWatcher_CgpDBPersonChanged(Person person, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        person.GetId(),
                        person.GetObjectType()));

                return;
            }


            DataReplicationManager.Singleton.SendModifiedObjectToCcus(person);
        }

        void _dbWatcher_CgpDBCardChanged(Person person, Card card)
        {
            DataReplicationManager.Singleton.SendModifiedObjectToCcus(person);
        }

        private void AddCardsFromObjectReferences(List<Guid> listCards, AOrmObjectWithReferences aOrmObjectWithReferences)
        {
            if (aOrmObjectWithReferences != null)
            {
                if (aOrmObjectWithReferences.ObjectType == ObjectType.Card)
                    listCards.Add(aOrmObjectWithReferences.ObjectGuid);

                if (aOrmObjectWithReferences.References != null && aOrmObjectWithReferences.References.Count > 0)
                {
                    foreach (var actAOrmObjectWithReferences in aOrmObjectWithReferences.References)
                    {
                        AddCardsFromObjectReferences(listCards, actAOrmObjectWithReferences);
                    }
                }
            }
        }

        void _dbWatcher_CgpDBTimeZoneChanged(object objectId, ObjectType objectType, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        (Guid)objectId,
                        objectType));
            }
            else 
            {
                var timeZone = DataReplicationManager.LoadObjectFromDatabase(objectId, objectType) as TimeZone;

                if (timeZone != null)
                    DataReplicationManager.Singleton.SendModifiedObjectToCcus(timeZone);
            }
        }

        void _dbWatcher_CgpGeneralNtpSettignsChanged()
        {
            CCUConfigurationHandler.Singleton.TimeDifferenceTolerance = ServerGeneralOptionsDBs.Singleton.GetGeneralNtpTimeDiffTolerance();
            CCUs.Singleton.GeneralNtpAddressAkn();
        }

        //port to communicate with devices running TFTP server (like CCU)
        private int _tftpClientPort = 980;

        public int TftpClientPort
        {
            get { return _tftpClientPort; }
            set { _tftpClientPort = value; }
        }

        private string _ccuDefaultSaveDir = @"\Temp\";
        public string CcuDefaultSaveDir
        {
            get { return _ccuDefaultSaveDir; }
            set { _ccuDefaultSaveDir = value; }
        }

        private string _ccuCEImagesDir = @"\Storage card\";

        public string CcuCEImagesDir
        {
            get { return _ccuCEImagesDir; }
            set { _ccuCEImagesDir = value; }
        }

        private string _ccuUpgradeDirForDCUFiles = @"\NandFlash\ccu\DcuUpgrades\";

        public string CcuUpgradeDirForDCUFiles
        {
            get { return _ccuUpgradeDirForDCUFiles; }
            set { _ccuUpgradeDirForDCUFiles = value; }
        }

        private string _ccuUpgradeDirForCRFiles = @"\NandFlash\ccu\CRUpgrades\";

        public string CcuUpgradeDirForCRFiles
        {
            get { return _ccuUpgradeDirForCRFiles; }
            set { _ccuUpgradeDirForCRFiles = value; }
        }

        private readonly string _ccuUpgradesPath = QuickPath.AssemblyStartupPath + @"\" + CCU_UPGRADES_DIRECTORY_NAME;
        public string CcuUpgradesPath
        {
            get { return _ccuUpgradesPath; }
        }

        private readonly string _dcuUpgradesPath = QuickPath.AssemblyStartupPath + @"\" + DCU_UPGRADES_DIRECTORY_NAME;
        public string DcuUpgradesPath
        {
            get { return _dcuUpgradesPath; }
        }

        private readonly string _ceUpgradesPath = QuickPath.AssemblyStartupPath + @"\" + CE_UPGRADES_DIRECTORY_NAME;
        public string CEUpgradesPath
        {
            get { return _ceUpgradesPath; }
        }

        private readonly string _crUpgradesPath = QuickPath.AssemblyStartupPath + @"\" + CR_UPGRADES_DIRECTORY_NAME;
        public string CRUpgradesPath
        {
            get { return _crUpgradesPath; }
        }

        private int _ceUpgradeUDPBlockSize = 16384;

        public int CeUpgradeUDPBlockSize
        {
            get { return _ceUpgradeUDPBlockSize; }
            set { _ceUpgradeUDPBlockSize = value; }
        }

        public List<string> GetAvailableUpgrades()
        {
            var result = new List<string>();
            QuickPath.EnsureDirectory(_ccuUpgradesPath);
            foreach (var file in Directory.GetFiles(_ccuUpgradesPath))
            {
                Exception ex;
                var headerProperties = FilePacker.TryGetHeaderParameters(file, out ex);
                if (headerProperties == null || headerProperties.Length < 2 || headerProperties[0].ToLower() != DeviceType.CCU.ToString().ToLower())
                    continue;

                result.Add(headerProperties[1]);
            }
            result.Sort();
            result.Reverse();
            return result;
        }

        private readonly SyncDictionary<string, CCUFileTransferPurpose> _ccuTransferPurposes = new SyncDictionary<string, CCUFileTransferPurpose>();

        private readonly Dictionary<Guid, TransferClient> _clients = new Dictionary<Guid, TransferClient>();
        private readonly Dictionary<Guid, Guid> _ccuUpgraders = new Dictionary<Guid, Guid>();
        private readonly Dictionary<Guid, List<byte>> _dcuUpgraders = new Dictionary<Guid, List<byte>>();
        public bool StartUpgradeCCU(string fileVersion, string ipAddress, Guid upgradeID, out Exception ex)
        {
            ex = null;
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip))
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "IP address " + ipAddress + " has incorrect format");
                ex = new FormatException("IP address " + ipAddress + " has incorrect format");
                return false;
            }
            var fileName = string.Empty;
            foreach (var file in Directory.GetFiles(_ccuUpgradesPath))
            {
                Exception exc;
                var headerParameters = FilePacker.TryGetHeaderParameters(file, out exc);
                if (headerParameters == null || headerParameters.Length < 2 || headerParameters[0].ToLower() != DeviceType.CCU.ToString().ToLower() || headerParameters[1] != fileVersion)
                    continue;

                fileName = file;
                break;
            }
            if (fileName == string.Empty)
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Upgrade file with version " + fileVersion + " was not found on server");
                ex = new FileNotFoundException("Upgrade file with version " + fileVersion + " was not found on server");
                return false;
            }
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress);
            if (ccu == null)
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "There is no CCU with IP address " + ipAddress);
                ex = new NullReferenceException("There is no CCU with IP address " + ipAddress);
                return false;
            }

            try
            {
                _ccuUpgraders[ccu.IdCCU] = upgradeID;
                _ccuTransferPurposes[ccu.IPAddress] = CCUFileTransferPurpose.CCUUpgrade;

                if (!CCUConfigurationHandler.Singleton.IsSupportedTCPDataChannel(ccu.IdCCU))
                {
                    if (!CCUConfigurationHandler.Singleton.StartUpgrade(ccu.IdCCU))
                    {
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU }, "Failed to start upgrade mode",
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));
                        throw new Exception("Failed to start upgrade mode");
                    }

                    try
                    {
                        _clients[ccu.IdCCU] = new TransferClient(new TFTPClient(_tftpClientPort))
                        {
                            TftpClient = { ProgressStep = 5 }
                        };
                        _clients[ccu.IdCCU].TftpClient.Start();
                        _clients[ccu.IdCCU].TftpClient.OnPercentSent += TFTPOnPercentSent;
                    }
                    catch (Exception e)
                    {
                        CCUConfigurationHandler.Singleton.StopTFTPServer(ccu.IdCCU);
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU }, "Failed to start TFTP client",
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));
                        ex = e;
                        throw ex;
                    }
                    _clients[ccu.IdCCU].TftpClient.BeginSendFile(fileName, _ccuDefaultSaveDir, ipAddress, OnSendFileFinish);
                }
                else
                {
                    if (!CCUConfigurationHandler.Singleton.StartUpgradeMode(ccu.IdCCU))
                    {
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU }, "Failed to start upgrade mode",
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));
                        throw new Exception("Failed to start upgrade mode");
                    }

                    FileStream stream = null;
                    try
                    {
                        stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU },
                            string.Format("Failed to open file stream {0}. Exception: {1}", fileName, ex.Message),
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));

                        if (stream != null)
                            try { stream.Close(); } catch { }
                    }

                    if (ex != null)
                        throw ex;

                    try
                    {
                        var ti =
                            CCUConfigurationHandler.Singleton.DataChannel.Transfer(
                                stream,
                                new IPEndPoint(ip, NCASConstants.TCP_DATA_CHANNEL_PORT),
                                true,
                                // ReSharper disable once AssignNullToNotNullAttribute
                                Path.Combine(_ccuDefaultSaveDir, Path.GetFileName(fileName)),
                                true);
                        _clients[ccu.IdCCU] = new TransferClient(ti);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPECCUUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU },
                            string.Format("Failed to transfer stream {0}. Exception: {1}", fileName, ex.Message),
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));

                        try
                        {
                            stream.Close();
                        }
                        catch
                        {

                        }
                    }
                    finally
                    {
                        // THE STREAM SHOULD NOT BE CLOSED HERE BECAUSE
                        // IF NO EXCEPTION IN PREVIOUS CATCH, THE STREAM IS BEING
                        // USED BY THE DataChannel.Transfer CALL IN SEPARATE THREAD
                        DebugHelper.NOP();
                    }

                    if (ex != null)
                        throw ex;
                }
                return true;
            }
            catch (Exception)
            {
                CCUConfigurationHandler.Singleton.StopUpgradeMode(ccu.IdCCU, false);
                ClearCCUUpgradeResources(ccu.IdCCU);
                return false;
            }
        }

        private void OnSendFileFinish(string filename, string savePath, string address, bool success, Exception ex)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(address);
            if (ccu != null)
            {
                lock (_clients)
                {
                    if (_clients.ContainsKey(ccu.IdCCU))
                    {
                        _clients[ccu.IdCCU].Stop();
                        _clients.Remove(ccu.IdCCU);
                    }
                }

                if (_ccuTransferPurposes.ContainsKey(ccu.IPAddress))
                {
                    switch (_ccuTransferPurposes[ccu.IPAddress])
                    {
                        case CCUFileTransferPurpose.DCUUpgrade:
                        case CCUFileTransferPurpose.CCUUpgrade:
                        case CCUFileTransferPurpose.CEUpgrade:
                        case CCUFileTransferPurpose.CRUpgrade:
                            if (!success)
                            {
                                var transferPurpose = CCUFileTransferPurpose.CCUUpgrade;
                                if (_ccuTransferPurposes.ContainsKey(address))
                                    transferPurpose = _ccuTransferPurposes[address];
                                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunTFTPTransferProgressChanged, DelegateSequenceBlockingMode.Asynchronous, false, new object[] { IPAddress.Parse(address), -1, (byte)transferPurpose });
                                ClearCCUUpgradeResources(ccu.IdCCU);
                                CCUConfigurationHandler.Singleton.StopUpgradeMode(ccu.IdCCU, false);
                            }
                            break;
                            /*default:
                                break;*/
                    }
                }
            }
        }

        /*
        private byte[] CountCheckSum(string fullFileName)
        {
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(fullFileName);
                return BitConverter.GetBytes(Crc32.ComputeChecksum(fs));
            }
            catch
            { }
            finally
            {
                if (fs != null)
                    try
                    {
                        fs.Close();
                    }
                    catch
                    { }
            }
            return null;
        }
         * */

        public void ClearCCUUpgradeResources(Guid ccuGuid)
        {
            SafeThread<Guid>.StartThread(DoClearCCUUpgradeResources, ccuGuid);
        }

        private void DoClearCCUUpgradeResources(Guid ccuGuid)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(ccuGuid))
                    _clients.Remove(ccuGuid);
            }
            lock (_ccuUpgraders)
            {
                if (_ccuUpgraders.ContainsKey(ccuGuid))
                    _ccuUpgraders.Remove(ccuGuid);
            }
            lock (_dcuUpgraders)
            {
                if (_dcuUpgraders.ContainsKey(ccuGuid))
                    _dcuUpgraders.Remove(ccuGuid);
            }
        }

        public void ClearDCUUpgradeResources(Guid ccuGuid, byte dcuLogicalAddress)
        {
            lock (_dcuUpgraders)
            {
                if (_dcuUpgraders.ContainsKey(ccuGuid) && _dcuUpgraders[ccuGuid].Contains(dcuLogicalAddress))
                {
                    _dcuUpgraders[ccuGuid].Remove(dcuLogicalAddress);
                    if (_dcuUpgraders[ccuGuid].Count == 0)
                        _dcuUpgraders.Remove(ccuGuid);
                }
            }
        }

        public void ClearDCUUpgradeResources(Guid ccuGuid, byte[] dcuLogicalAddresses)
        {
            foreach (var logicalAddress in dcuLogicalAddresses)
            {
                ClearDCUUpgradeResources(ccuGuid, logicalAddress);
            }
        }

        public bool CanStop(Guid clientUpgradeID, Guid ccuGuid)
        {
            if (_ccuUpgraders.ContainsKey(ccuGuid))
                return clientUpgradeID == _ccuUpgraders[ccuGuid];
            return false;
        }

        internal bool StopUnpackUpgradeProcess(Guid upgradeID, Guid ccuGuid)
        {
            if (_ccuUpgraders[ccuGuid] != upgradeID)
                return false;
            return CCUs.Singleton.StopUnpack(ccuGuid);
        }

        internal bool StopTransferUpgradeProcess(Guid upgradeID, Guid ccuGuid)
        {
            if (!_ccuUpgraders.ContainsKey(ccuGuid))
                return false;
            if (_ccuUpgraders[ccuGuid] != upgradeID)
                return false;

            _ccuUpgraders.Remove(ccuGuid);
            if (_clients.ContainsKey(ccuGuid))
            {
                _clients[ccuGuid].Stop();
                if (_clients[ccuGuid].TftpClient != null)
                    CCUConfigurationHandler.Singleton.StopTFTPServer(ccuGuid);
                _clients.Remove(ccuGuid);
            }

            return true;
        }

        public void UpgradeDCU(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte dcuLogicalAddress, out Exception ex, out List<byte> registeredDCUs)
        {
            UpgradeDCUs(selectedVersion, upgradeID, ccuIpAddress, new[] { dcuLogicalAddress }, out ex, out registeredDCUs);
        }


        public void UpgradeDCUs(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte[] dcusLogicalAddresses, out Exception ex, out List<byte> registeredDCUs)
        {
            ex = null;
            registeredDCUs = new List<byte>();

            IPAddress ip;
            if (!IPAddress.TryParse(ccuIpAddress, out ip))
            {
                ex = new FormatException("IP address " + ccuIpAddress + " has incorrect format");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Failed to parse CCU IP address: " + ccuIpAddress);
                return;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIpAddress);
            if (ccu == null)
            {
                ex = new NullReferenceException("There is no CCU with IP address " + ccuIpAddress);
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Failed to get CCU with IP address: " + ccuIpAddress);
                return;
            }

            var mbv = CCUs.Singleton.GetCCUMainBoardType(ccu.IdCCU);
            var dcuhwVersion = DCUHWVersion.Unknown;

            switch (mbv)
            {
                case MainBoardVariant.CCU0_ECHELON:
                    dcuhwVersion = DCUHWVersion.Echelon;
                    break;

                case MainBoardVariant.CCU0_RS485:
                    dcuhwVersion = DCUHWVersion.RS485;
                    break;
            }

            var fileName = string.Empty;
            var dcuTag = DeviceType.DCU.ToString().ToLower();
            var gzFiles = Directory.GetFiles(_dcuUpgradesPath, "*.gz");
            foreach (var file in gzFiles)
            {
                try
                {
                    Exception exc;
                    var headerProperties = FilePacker.TryGetHeaderParameters(file, out exc);
                    if (headerProperties == null ||
                        headerProperties.Length < 4 ||
                        (DCUHWVersion)(Int32.Parse(headerProperties[1])) != dcuhwVersion ||
                        headerProperties[0].ToLower() != dcuTag ||
                        headerProperties[2] != selectedVersion)
                    {
                        continue;
                    }

                }
                catch
                {
                    continue;
                }

                fileName = file;
                break;
            }
            if (fileName == string.Empty)
            {
                ex = new FileNotFoundException("Upgrade file with version " + selectedVersion + " was not found on server");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Could not find upgrade file with specific version on server. Version: " + selectedVersion);
                return;
            }

            var regDCUs = CCUConfigurationHandler.Singleton.RegisterDCUsForUpgradeVersion(ccuIpAddress, dcusLogicalAddresses, selectedVersion);
            if (regDCUs == null || regDCUs.Length == 0)
            {
                ex = new RegisterDcuUpgradeVersionException("All selected DCUs for upgrade are offline");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now,
                    GetType().Assembly.GetName().Name,
                    new[] { ccu.IdCCU }, "All selected DCUs for upgrade with version " + selectedVersion + " are offline",
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));
                return;
            }
            registeredDCUs = new List<byte>(regDCUs);


            string upgradePackageFromCCU;
            var fullVersionInfo = string.Format("(0{0}){1}", ((byte)dcuhwVersion), selectedVersion);
            var versionExists = CCUConfigurationHandler.Singleton.DCUUpgradeFileVersionExists(ccu.IdCCU, _ccuUpgradeDirForDCUFiles, fullVersionInfo, out upgradePackageFromCCU);

            _dcuUpgraders[ccu.IdCCU] = new List<byte>(dcusLogicalAddresses);

            if (!versionExists)
            {
                if (CCUConfigurationHandler.Singleton.EnsureDirectory(ccu.IdCCU, _ccuUpgradeDirForDCUFiles))
                {
                    _ccuTransferPurposes[ccu.IPAddress] = CCUFileTransferPurpose.DCUUpgrade;

                    FileStream stream = null;
                    try
                    {
                        stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU },
                            string.Format("Failed to open file stream {0}. Exception: {1}", fileName, ex.Message),
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));

                        if (stream != null)
                            try
                            {
                                stream.Close();
                            }
                            catch
                            {

                            }
                        return;
                    }

                    try
                    {
                        CCUConfigurationHandler.Singleton.DataChannel.Transfer(
                            stream,
                            new IPEndPoint(ip, NCASConstants.TCP_DATA_CHANNEL_PORT),
                            true,
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Path.Combine(_ccuUpgradeDirForDCUFiles, Path.GetFileName(fileName)),
                            true);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                        Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now,
                            GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU },
                            string.Format("Failed to transfer stream {0}. Exception: {1}", fileName, ex.Message),
                            new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                                CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                            new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                                CgpServer.Singleton.Version));
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (stream != null)
                            try
                            {
                                stream.Close();
                            }
                            catch
                            {

                            }
                        //return;

                    }
                }
                else
                {
                    ClearDCUUpgradeResources(ccu.IdCCU, dcusLogicalAddresses);
                    ex = new DirectoryNotFoundException(_ccuUpgradeDirForDCUFiles);
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU }, "Failed to create directory for DCU upgrades: " + _ccuUpgradeDirForDCUFiles,
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));
                    //return;
                }
            }
            else
            {
                CCUConfigurationHandler.Singleton.UpgradeDcus(ccu.IdCCU, upgradePackageFromCCU);
            }
        }

        public IList<string> GetAvailableDCUUpgrades(Guid ccuGuid, out string delimeter)
        {
            delimeter = ":";
            if (_dcuUpgradesPath == string.Empty)
                return null;
            if (!Directory.Exists(_dcuUpgradesPath))
            {
                QuickPath.EnsureDirectory(_dcuUpgradesPath);
                return null;
            }
            var result = new List<string>();
            foreach (var fileName in Directory.GetFiles(_dcuUpgradesPath))
            {
                Exception ex;
                var headerParameters = FilePacker.TryGetHeaderParameters(fileName, out ex);
                if (headerParameters == null || headerParameters.Length < 4 || headerParameters[0].ToLower() != DeviceType.DCU.ToString().ToLower())
                    continue;

                result.Add(headerParameters[2] + delimeter + headerParameters[1]);
            }
            if (ccuGuid != Guid.Empty)
            {
                var resultFromCCU = CCUConfigurationHandler.Singleton.GetAvailableDCUUpgrades(ccuGuid, _ccuUpgradeDirForDCUFiles);
                if (resultFromCCU != null && resultFromCCU.Count > 0)
                {
                    foreach (var version in resultFromCCU)
                    {
                        if (!result.Contains(version))
                            result.Add(version);
                    }
                }
            }
            result.Sort();
            result.Reverse();
            return result;
        }

        public override ICollection<AOrmObject> GetReferencedByObject(AOrmObject objRef)
        {
            return NcasDbs.Singleton.GetReferencedByObjects(objRef);
        }

        public override IEnumerable<AOrmObject> GetDirectReferences(AOrmObject objRef)
        {
            return NcasDbs.GetDirectReferences(objRef);
        }

        public bool IsCCUUpgrading(Guid ccuGuid)
        {
            var result = (_ccuUpgraders.ContainsKey(ccuGuid) || _dcuUpgraders.ContainsKey(ccuGuid) || IsAnyDCUUpgrading(ccuGuid));
            return result;
        }

        private bool IsAnyDCUUpgrading(Guid ccuGuid)
        {
            if (ccuGuid == Guid.Empty)
                return false;
            var ccu = CCUs.Singleton.GetById(ccuGuid);
            if (ccu == null)
                return false;
            foreach (var dcu in ccu.DCUs)
            {
                var state = CCUConfigurationHandler.Singleton.GetDCUOnlineState(dcu);
                if (state == OnlineState.AutoUpgrading || state == OnlineState.Upgrading)
                    return true;
            }
            return false;
        }

        public bool CanUpgradeDCU(Guid ccuGuid, byte dcuLogicalAddress)
        {
            if (_ccuUpgraders.ContainsKey(ccuGuid))
                return false;
            var ccu = CCUs.Singleton.GetById(ccuGuid);
            if (ccu == null)
                return false;
            return !IsDCUUpgrading(ccuGuid, dcuLogicalAddress);
        }

        private bool IsDCUUpgrading(Guid ccuGuid, byte dcuLogicalAddress)
        {
            if (ccuGuid == Guid.Empty)
                return false;
            var ccu = CCUs.Singleton.GetById(ccuGuid);
            if (ccu == null)
                return false;
            foreach (var dcu in ccu.DCUs)
            {
                var state = CCUConfigurationHandler.Singleton.GetDCUOnlineState(dcu);
                if (dcu.LogicalAddress == dcuLogicalAddress && (state == OnlineState.Upgrading || state == OnlineState.AutoUpgrading))
                    return true;
            }
            return false;
        }

        public bool SetDefaultDCUUpgradeVersion(Guid ccuGuid, string upgradeVersion)
        {
            if (ccuGuid == Guid.Empty || string.IsNullOrEmpty(upgradeVersion))
                return false;

            return CCUConfigurationHandler.Singleton.SetDefaultDCUUpgradeVersion(ccuGuid, upgradeVersion);
        }

        private string GetWindowsCeVersion(string filePath, out int crcVersion)
        {
            crcVersion = 0;
            Exception error;

            if (!string.IsNullOrEmpty(FilePacker.TryGetHeaderText(filePath, out error)))
            {
                string outputDirectory = string.Format(
                    "{0}\\{1}\\",
                    Path.GetDirectoryName(filePath),
                    "CePackage");

                if (Directory.Exists(outputDirectory))
                {
                    try
                    {
                        Directory.Delete(outputDirectory, true);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                new FileInfo(outputDirectory).Directory.Create();

                var filePacker = new FilePacker();

                try
                {
                    filePacker.Unpack(filePath, outputDirectory);
                }
                catch
                {
                    return null;
                }

                var ceBinFileName = Directory.GetFiles(outputDirectory)
                    .FirstOrDefault(file =>
                    {
                        string fileName = Path.GetFileName(file).ToLower();
                        return fileName.StartsWith("nk") && fileName.EndsWith(".bin");
                    });

                if (ceBinFileName == null)
                    return null;

                filePath = Path.Combine(outputDirectory, ceBinFileName);
            }

            return WinCeUpgradeFileHelper.GetVersion(filePath, out crcVersion);
        }

        public IList<string[]> GetAvailableCEUpgrades()
        {
            var result = new List<string[]>();

            if (_avaibleCeUpgrades != null)
            {
                result.AddRange(
                    _avaibleCeUpgrades.Keys.ToList().OrderBy(
                        obj => obj)
                        .Select(
                            key => new[]
                            {
                                key,
                                _avaibleCeUpgrades[key].ToString()
                            }));

                result = result.OrderBy(obj => obj[1]).ToList();
                result.Reverse();

                return result;
            }

            _avaibleCeUpgrades = new Dictionary<string, int>();

            if (!QuickPath.EnsureDirectory(_ceUpgradesPath))
                return null;

            foreach (var fullFileName in Directory.GetFiles(_ceUpgradesPath))
            {
                string fileName = Path.GetFileName(fullFileName);
                int crcVersion;
                string version = GetWindowsCeVersion(fullFileName, out crcVersion);

                if (!string.IsNullOrEmpty(version))
                {
                    _avaibleCeUpgrades.Add(fileName, Int32.Parse(version));

                    result.Add(new[]
                    {
                        fileName,
                        _avaibleCeUpgrades[fileName].ToString()
                    });
                }
                else
                {
                    try
                    {
                        File.Delete(fullFileName);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }

            result = result.OrderBy(obj => obj[1]).ToList();
            result.Reverse();

            return result;
        }

        public bool AddCeUpgradeFile(string filePath)
        {
            int crcVersion;
            string version = GetWindowsCeVersion(filePath, out crcVersion);
            string fileName = Path.GetFileName(filePath);

            if (!string.IsNullOrEmpty(version))
            {
                if (!_avaibleCeUpgrades.ContainsKey(fileName))
                    _avaibleCeUpgrades.Add(fileName, Int32.Parse(version));

                return true;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        public void RemoveCeUpgradeFile(string fileName)
        {
            if (_avaibleCeUpgrades.ContainsKey(fileName))
                _avaibleCeUpgrades.Remove(fileName);
        }

        //private bool PassCeUpgradeFileCondition(string fileName)
        //{
        //    if (string.IsNullOrEmpty(fileName))
        //        return false;
        //    if (Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}.bin") 
        //        || Regex.IsMatch(fileName, "NK_[0-9]{4,5}.bin")
        //        || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[0-9]{1}.bin")
        //        || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[C]{1}[R]{1}[C]{1}[v]{1}[0-9]{1}.bin")
        //        || Regex.IsMatch(fileName, "NK_[0-9]{6,8}_[0-9]{4,5}_[0-9]{1}_[C]{1}[R]{1}[C]{1}[v]{1}[0-9]{1}.bin"))
        //        return true;
        //    return false;
        //}

        //public string RenameCEFileIfNeeded(string fullFileName)
        //{
        //    if (!PassCeUpgradeFileCondition(Path.GetFileName(fullFileName)))
        //        return fullFileName;

        //    var newName = Path.GetFileName(fullFileName);
        //    if (newName != null)
        //    {
        //        if (Regex.IsMatch(newName, "NK_[0-9]{6,8}_[0-9]{4,5}.bin"))
        //        {
        //            try
        //            {
        //                newName = newName.Remove(newName.IndexOf('_') + 1,
        //                    newName.IndexOf('_', newName.IndexOf('_') + 1) - newName.IndexOf('_'));
        //                // ReSharper disable once AssignNullToNotNullAttribute
        //                newName = Path.Combine(Path.GetDirectoryName(fullFileName), newName);
        //                File.Move(fullFileName, newName);
        //                return newName;
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    }

        //    return fullFileName;
        //}

        public bool UpgradeCE(string selectedFileName, Guid upgradeID, string ipAddress, out Exception ex)
        {
            ex = null;
            if (string.IsNullOrEmpty(selectedFileName))
            {
                ex = new FormatException("selectedFileName parameter must be specified");
                return false;
            }

            //if (WinCeUpgradeFileHelper.GetVersion(_ceUpgradesPath + "\\" +selectedFileName) == null)
            //{
            //    ex = new FormatException("selectedFileName parameter does not match CE upgrade conditions");
            //    return false;
            //}

            if (upgradeID == Guid.Empty)
            {
                ex = new FormatException("upgradeID parameter must be defined");
                return false;
            }
            IPAddress ip;
            if (!IPAddress.TryParse(ipAddress, out ip))
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                            new Guid[] { }, string.Format("Failed to parse CCU IP address from: {0}", ipAddress));
                ex = new FormatException("ip parameter is invalid");
                return false;
            }
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress);
            if (ccu == null)
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                            new Guid[] { }, "Failed to get CCU with ip address: " + ipAddress);
                ex = new NullReferenceException("There is no CCU with ip address: " + ipAddress);
                return false;
            }
            var ceUpgradeFileName = string.Empty;
            foreach (var fullFileName in Directory.GetFiles(_ceUpgradesPath))
            {
                if (Path.GetFileName(fullFileName) == selectedFileName)
                {
                    ceUpgradeFileName = fullFileName;
                    break;
                }
            }
            if (ceUpgradeFileName == string.Empty)
            {
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                            new[] { ccu.IdCCU }, "File " + selectedFileName + " was not found on server or is not valid");
                ex = new FileNotFoundException("File " + selectedFileName + " was not found on server or is not valid");
                return false;
            }

            try
            {
                if (!CCUConfigurationHandler.Singleton.StartUpgradeMode(ccu.IdCCU))
                {
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU }, "Failed to start upgrade mode",
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));
                    ex = new Exception("Failed to start upgrade mode");
                    throw ex;
                }

                _ccuUpgraders[ccu.IdCCU] = upgradeID;
                _ccuTransferPurposes[ccu.IPAddress] = CCUFileTransferPurpose.CEUpgrade;

                FileStream stream = null;
                try
                {
                    stream = new FileStream(ceUpgradeFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception e)
                {
                    ex = e;
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU },
                        string.Format("Failed to open file stream {0}. Exception: {1}", ceUpgradeFileName, ex.Message),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                    if (stream != null)
                        try
                        {
                            stream.Close();
                        }
                        catch
                        {
                        }
                }

                if (ex != null)
                    throw ex;

                try
                {
                    var ti =
                        CCUConfigurationHandler.Singleton.DataChannel.Transfer(
                            stream,
                            new IPEndPoint(ip, NCASConstants.TCP_DATA_CHANNEL_PORT),
                            true,
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Path.Combine(_ccuCEImagesDir, Path.GetFileName(ceUpgradeFileName)),
                            true);
                    _clients[ccu.IdCCU] = new TransferClient(ti);
                }
                catch (Exception e)
                {
                    ex = e;
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPECEUPGRADEFAILED, DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU },
                        string.Format("Failed to transfer stream {0}. Exception: {1}", ceUpgradeFileName, ex.Message),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (stream != null)
                        try
                        {
                            stream.Close();
                        }
                        catch
                        {

                        }
                    throw ex;
                }
            }
            catch (Exception e)
            {
                ClearCCUUpgradeResources(ccu.IdCCU);
                CCUConfigurationHandler.Singleton.StopUpgradeMode(ccu.IdCCU, false);
                ex = e;
                return false;
            }

            return true;
        }

        private void TFTPOnPercentSent(IPAddress destinationIPAddress, int percents)
        {
            var transferPurpose = CCUFileTransferPurpose.CCUUpgrade;
            if (_ccuTransferPurposes.ContainsKey(destinationIPAddress.ToString()))
                transferPurpose = _ccuTransferPurposes[destinationIPAddress.ToString()];
            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(RunTFTPTransferProgressChanged, DelegateSequenceBlockingMode.Asynchronous, false, new object[] { destinationIPAddress, percents, (byte)transferPurpose });
        }

        public void DataChannelTransferProgress(DataChannelPeer.TransferInfo transferInfo, int progress)
        {
            if (transferInfo == null)
                return;

            if (_ccuTransferPurposes.ContainsKey(transferInfo.RemoteEndPoint.Address.ToString()))
            {
                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    RunTFTPTransferProgressChanged,
                    DelegateSequenceBlockingMode.Asynchronous,
                    false,
                    new object[]
                    {
                        transferInfo.RemoteEndPoint.Address,
                        progress,
                        (byte) _ccuTransferPurposes[transferInfo.RemoteEndPoint.Address.ToString()]
                    });
            }
        }

        private void RunTFTPTransferProgressChanged(ARemotingCallbackHandler remoteHandler, object[] info)
        {
            try
            {
                if (info != null && info.Length == 3 && info[0] is IPAddress && info[1] is int && info[2] is byte)
                {
                    var ipAddress = (IPAddress)info[0];
                    var percents = (int)info[1];
                    var transferPurpose = (CCUFileTransferPurpose)info[2];
                    if (remoteHandler is TFTPFileTransferProgressChangedHandler)
                        (remoteHandler as TFTPFileTransferProgressChangedHandler).RunEvent(ipAddress, percents, (byte)transferPurpose);
                }
            }
            catch (Exception e)
            {
                DebugHelper.NOP(e);
                throw;
            }
        }

        public override Dictionary<string, Type> GetRequiredLicenceProperties()
        {
            var result = new Dictionary<string, Type>();
            foreach (RequiredLicenceProperties property in Enum.GetValues(typeof(RequiredLicenceProperties)))
            {
                switch (property)
                {
                    case RequiredLicenceProperties.DoorEnvironmentCount:
                        result.Add(property.ToString(), typeof(int));
                        break;
                    case RequiredLicenceProperties.Graphics:
                        result.Add(property.ToString(), typeof(bool));
                        break;
                    case RequiredLicenceProperties.TimetecMaster:
                        result.Add(property.ToString(), typeof(bool));
                        break;
                    case RequiredLicenceProperties.CCU40MaxDsm:
                        result.Add(property.ToString(), typeof(int));
                        break;
                    case RequiredLicenceProperties.CAT12CEMaxDsm:
                        result.Add(property.ToString(), typeof(int));
                        break;
                    case RequiredLicenceProperties.CCU05MaxDsm:
                        result.Add(property.ToString(), typeof(int));
                        break;
                    default:
                        result.Add(property.ToString(), typeof(string));
                        break;
                }
            }
            return result;
        }

        public override bool SetRequiredLicenceProperties(Dictionary<string, object> properties)
        {
            try
            {
                bool needRestart = false;
                var requiredProperties = GetRequiredLicenceProperties();

                if (requiredProperties == null)
                    return false;

                if (_requiredLicenceProperties.Count > 0)
                {
                    if (_requiredLicenceProperties.Any(obj => !properties.ContainsKey(obj.Key)) ||
                        _requiredLicenceProperties.Any(
                            obj => obj.Value.GetType() != typeof(string)
                                   && ((IComparable)obj.Value).CompareTo(properties[obj.Key]) > 0))
                    {
                        needRestart = true;
                    }
                }

                _requiredLicenceProperties.Clear();

                foreach (var property in properties)
                {
                    if (property.Value == null)
                        continue;

                    if (requiredProperties.ContainsKey(property.Key) &&
                        requiredProperties[property.Key] == (property.Value.GetType()))
                    {
                        _requiredLicenceProperties[property.Key] = property.Value;
                    }
                }

                return needRestart;
            }
            catch
            {
                return false;
            }
        }

        public override bool GetLicencePropertyInfo(string propertyName, string language, out string localisedName, out object value)
        {
            if (string.IsNullOrEmpty(language))
                language = GeneralOptions.Singleton.CurrentLanguage;

            localisedName = propertyName;
            value = null;

            try
            {
                if (_requiredLicenceProperties.ContainsKey(propertyName))
                {
                    value = _requiredLicenceProperties[propertyName];
                    if (value != null)
                    {

                        // this part is not that important,
                        // so even if translation fails, the value cannot be degraded by any exception
                        try
                        {
                            localisedName = LocalizationHelper.GetString(propertyName, language);
                        }
                        catch
                        {
                            try
                            {
                                localisedName = LocalizationHelper.GetString(propertyName, CgpServerGlobals.DEFAULT_LANGUAGE);
                            }
                            catch
                            {
                                localisedName = propertyName;
                            }
                        }
                    }
                    return true;
                }
            }
            catch { }
            return false;
        }

        public override bool GetLocalisedLicencePropertyName(string propertyName, string language, out string localisedName)
        {
            localisedName = string.Empty;

            var result = LocalizationHelper.GetString(propertyName, language);
            if (!string.IsNullOrEmpty(result) && !result.StartsWith(LocalizationHelper.NO_TRANSLATION))
            {
                localisedName = result;
                return true;
            }
            return false;
        }

        public override void SetLanguage(string language)
        {
            TrySetLanguage(language);
        }

        private void TrySetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return;
            try
            {
                LocalizationHelper.SetLanguage(language);
            }
            catch { }
        }

        public void OnCCUFileReceived(IPAddress ipAddress, string fullFileName)
        {
            if (ipAddress == null || fullFileName == null || fullFileName == string.Empty)
                return;
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ipAddress.ToString());
            if (ccu == null)
                return;

            RemoveTransferPurpose(ccu.IPAddress);
        }

        private void RemoveTransferPurpose(string ccuIpAddress)
        {
            _ccuTransferPurposes.Remove(ccuIpAddress);
        }

        public IList<string> GetAvailableCRUpgrades(out string delimeter)
        {
            delimeter = ":";
            if (_crUpgradesPath == string.Empty)
                return null;
            if (!Directory.Exists(_crUpgradesPath))
            {
                QuickPath.EnsureDirectory(_crUpgradesPath);
                return null;
            }

            var result = new List<string>();
            foreach (var fullFileName in Directory.GetFiles(_crUpgradesPath))
            {
                Exception ex;
                var headerParameters = FilePacker.TryGetHeaderParameters(fullFileName, out ex);
                if (headerParameters == null || headerParameters.Length < 5 || headerParameters[0].ToLower() != DeviceType.CR.ToString().ToLower())
                    continue;

                result.Add(headerParameters[1] + delimeter + headerParameters[2]);
            }
            result.Sort();
            result.Reverse();
            return result;
        }

        public bool UpgradeCRs(
            string selectedVersion,
            byte crHwVersion,
            Guid upgradeID,
            string ccuIpAddress,
            List<Guid> ccusCardReadersToUpgrade,
            Dictionary<byte, List<byte>> dcusCardReadersToUpgrade,
            out Exception ex)
        {
            ex = null;
            var fileName = string.Empty;
            foreach (var file in Directory.GetFiles(_crUpgradesPath))
            {
                Exception exc;
                var headerParameters = FilePacker.TryGetHeaderParameters(file, out exc);
                if (headerParameters == null || headerParameters.Length < 5)
                    continue;

                byte hwVersion;
                try
                {
                    hwVersion = (byte.Parse(headerParameters[2], NumberStyles.HexNumber));
                }
                catch
                {
                    continue;
                }

                if (headerParameters[0].ToLower() != DeviceType.CR.ToString().ToLower()
                    || headerParameters[1] != selectedVersion
                    || hwVersion != crHwVersion)
                    continue;

                fileName = file;
                break;
            }
            if (fileName == string.Empty)
            {
                ex = new FileNotFoundException("Upgrade file with version " + selectedVersion + " was not found on server");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Could not find upgrade file with specific version on server. Version: " + selectedVersion);
                return false;
            }

            IPAddress ip;
            if (!IPAddress.TryParse(ccuIpAddress, out ip))
            {
                ex = new FormatException("IP address " + ccuIpAddress + " has incorrect format");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Failed to parse CCU IP address: " + ccuIpAddress);
                return false;
            }

            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuIpAddress);
            if (ccu == null)
            {
                ex = new NullReferenceException("There is no CCU with IP address " + ccuIpAddress);
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                new Guid[] { }, "Failed to get CCU with IP address: " + ccuIpAddress);
                return false;
            }

            var registerCRsForUpgradeVersionSuccessed = true;
            if (dcusCardReadersToUpgrade != null && dcusCardReadersToUpgrade.Count > 0)
            {
                var allFound = true;
                var dcusAndCardReaders = new Dictionary<byte, byte[]>();
                var missingDCUs = new List<byte>();

                var dcus = new List<byte>();
                if (ccu.DCUs != null && ccu.DCUs.Count > 0)
                {
                    foreach (var dcu in ccu.DCUs)
                    {
                        dcus.Add(dcu.LogicalAddress);
                    }
                }

                foreach (var kvp in dcusCardReadersToUpgrade)
                {
                    if (!dcus.Contains(kvp.Key))
                    {
                        allFound = false;
                        missingDCUs.Add(kvp.Key);
                    }
                    else
                    {
                        if (kvp.Value != null)
                        {
                            dcusAndCardReaders.Add(kvp.Key, kvp.Value.ToArray());
                        }
                    }
                }

                if (!allFound)
                {
                    var missingDCUsMessage = new StringBuilder();
                    foreach (var address in missingDCUs)
                    {
                        if (missingDCUsMessage.Length != 0)
                            missingDCUsMessage.Append(",");
                        missingDCUsMessage.Append(address);
                    }

                    ex = new NullReferenceException("Specified CCU has no DCUs with addresses: " + missingDCUsMessage);
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                                    new Guid[] { }, "Specified CCU has no DCUs with addresses: " + missingDCUsMessage + ". CCU IP address: " + ccuIpAddress);
                    return false;
                }

                registerCRsForUpgradeVersionSuccessed = CCUConfigurationHandler.Singleton.RegisterCRsForUpgradeVersion(ccuIpAddress, dcusAndCardReaders, selectedVersion);
            }

            if (registerCRsForUpgradeVersionSuccessed && ccusCardReadersToUpgrade != null && ccusCardReadersToUpgrade.Count > 0)
            {
                registerCRsForUpgradeVersionSuccessed = CCUConfigurationHandler.Singleton.RegisterCCUsCRsForUpgradeVersion(ccuIpAddress, ccusCardReadersToUpgrade.ToArray(), selectedVersion);
            }

            if (!registerCRsForUpgradeVersionSuccessed)
            {
                ex = new RegisterDcuUpgradeVersionException("Failed to register card readers for upgrade version");
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now,
                    GetType().Assembly.GetName().Name,
                    new[] { ccu.IdCCU }, "Failed to register card readers for upgrade version: " + selectedVersion,
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));
                return false;
            }

            if (CCUConfigurationHandler.Singleton.EnsureDirectory(ccu.IdCCU, _ccuUpgradeDirForCRFiles))
            {
                _ccuTransferPurposes[ccu.IPAddress] = CCUFileTransferPurpose.CRUpgrade;

                FileStream stream = null;
                try
                {
                    stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch (Exception e)
                {
                    ex = e;
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPEDCUSUPGRADEFAILED, DateTime.Now, GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU }, string.Format("Failed to open file stream {0}. Exception: {1}", fileName, ex.Message),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                    if (stream != null)
                        try
                        {
                            stream.Close();
                        }
                        catch
                        {
                        }
                    return false;
                }

                try
                {
                    CCUConfigurationHandler.Singleton.DataChannel.Transfer(
                        stream,
                        new IPEndPoint(ip, NCASConstants.TCP_DATA_CHANNEL_PORT),
                        true,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Path.Combine(_ccuUpgradeDirForCRFiles, Path.GetFileName(fileName)),
                        true);
                }
                catch (Exception e)
                {
                    ex = e;
                    Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now,
                        GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU },
                        string.Format("Failed to transfer stream {0}. Exception: {1}", fileName, ex.Message),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (stream != null)
                        try
                        {
                            stream.Close();
                        }
                        catch
                        {

                        }
                    return false;
                }
            }
            else
            {
                ex = new DirectoryNotFoundException(_ccuUpgradeDirForCRFiles);
                Eventlogs.Singleton.InsertEvent(Eventlog.TYPECARDREADERSUPGRADEFAILED, DateTime.Now,
                    GetType().Assembly.GetName().Name,
                    new[] { ccu.IdCCU },
                    "Failed to create directory for card reader upgrades: " + _ccuUpgradeDirForCRFiles,
                    new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                        CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                    new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                        CgpServer.Singleton.Version));
                return false;
            }
            return true;
        }

        public override void GeneralOptionsChanged()
        {
            CCUConfigurationHandler.Singleton.GeneralOptionsEventlogsChanged();
            CCUConfigurationHandler.Singleton.SetEnableLoggingSDPSTZChanges();
            CCUConfigurationHandler.Singleton.SetTimeSyncingFromServer();
            CCUConfigurationHandler.Singleton.SetAllowPINCachingInCardReaderMenu();
            CCUConfigurationHandler.Singleton.SetAllowCodeLength();
            CCUConfigurationHandler.Singleton.SetPinConfirmationObligatory();
            CCUConfigurationHandler.Singleton.SetAlarmAreaRestrictivePolicyForTimeBuying();
            DataReplicationManager.Singleton.GeneralOptionsChanged();
        }

        public void DataChannelTransferResult(DataChannelPeer.TransferInfo transferDescriptor, bool success)
        {
            if (transferDescriptor == null)
                return;

            var ip = transferDescriptor.RemoteEndPoint.Address.ToString();

            CCUFileTransferPurpose transferPurpose;
            if (_ccuTransferPurposes.TryGetValue(ip, out transferPurpose))
            {
                if (success)
                {
                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        RunTFTPTransferProgressChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            transferDescriptor.RemoteEndPoint.Address,
                            100,
                            (byte) transferPurpose
                        });
                }
                else
                {
                    var ccu = CCUs.Singleton.GetCCUFormIpAddress(ip);
                    if (ccu == null)
                        return;


                    var eventType = default(string);
                    switch (transferPurpose)
                    {
                        case CCUFileTransferPurpose.DCUUpgrade:
                            eventType = Eventlog.TYPEDCUSUPGRADEFAILED;
                            break;
                        case CCUFileTransferPurpose.CCUUpgrade:
                            CCUConfigurationHandler.Singleton.StopUpgradeMode(ccu.IdCCU, false);
                            eventType = Eventlog.TYPECCUUPGRADEFAILED;
                            break;
                        case CCUFileTransferPurpose.CEUpgrade:
                            eventType = Eventlog.TYPECEUPGRADEFAILED;
                            CCUConfigurationHandler.Singleton.StopUpgradeMode(ccu.IdCCU, false);
                            break;
                        case CCUFileTransferPurpose.CRUpgrade:
                            eventType = Eventlog.TYPECARDREADERSUPGRADEFAILED;
                            break;
                            /*default:
                            break;*/
                    }

                    ClearCCUUpgradeResources(ccu.IdCCU);

                    CCUConfigurationHandler.Singleton.TCPTransferFailed(ccu, ccu.IdCCU,
                        Path.GetFileName(transferDescriptor.DestinationName), "Data channel error",
                        new EventlogParameter(EventlogParameter.TYPECCU, ip, ccu.IdCCU, ObjectType.CCU),
                        new EventlogParameter(EventlogParameter.TYPE_STREAM_NAME,
                            Path.GetFileName(transferDescriptor.DestinationName)),
                        new EventlogParameter("Error code", transferDescriptor.LastErrorCode.ToString()),
                        new EventlogParameter("Error message", transferDescriptor.LastErrorMessage));

                    Eventlogs.Singleton.InsertEvent(eventType, DateTime.Now, GetType().Assembly.GetName().Name,
                        new[] { ccu.IdCCU },
                        string.Format("Failed to transfer upgrade stream: {0}", transferDescriptor.DestinationName),
                        new EventlogParameter(EventlogParameter.TYPE_CCU_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuFirmwareVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_CE_VERSION,
                            CCUConfigurationHandler.Singleton.GetCcuCeVersion(ccu.IdCCU)),
                        new EventlogParameter(EventlogParameter.TYPE_SERVER_VERSION,
                            CgpServer.Singleton.Version));

                    NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                        RunTFTPTransferProgressChanged,
                        DelegateSequenceBlockingMode.Asynchronous,
                        false,
                        new object[]
                        {
                            transferDescriptor.RemoteEndPoint.Address,
                            -1,
                            (byte) transferPurpose
                        });
                }
            }

            try
            {
                transferDescriptor.SourceStream.Close();
            }
            catch (Exception)
            {
            }

            RemoveTransferPurpose(ip);
        }

        public override AOrmObject GetPluginTableObject(
            ObjectType objectType,
            Guid objectId)
        {
            if (objectType == ObjectType.CardReader)
                return CardReaders.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.CCU)
                return CCUs.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.DCU)
                return DCUs.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.SecurityDayInterval)
                return SecurityDayIntervals.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.SecurityDailyPlan)
                return SecurityDailyPlans.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.Output)
                return Outputs.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.Input)
                return Inputs.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.ACLSetting)
                return ACLSettings.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.ACLSettingAA)
                return ACLSettingAAs.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.ACLPerson)
                return ACLPersons.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.AccessControlList)
                return AccessControlLists.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.AlarmArea)
                return AlarmAreas.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.AACardReader)
                return AACardReaders.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.AccessZone)
                return AccessZones.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.DoorEnvironment)
                return DoorEnvironments.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.DevicesAlarmSetting)
                return DevicesAlarmSettings.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.SecurityTimeZone)
                return SecurityTimeZones.Singleton.GetObjectById(objectId);

            if (objectType == ObjectType.SecurityTimeZoneDateSetting)
                return SecurityTimeZoneDateSettings.Singleton.GetObjectById(objectId);

            return null;
        }

        public override ITableORM GetTableOrmForObjectType(ObjectType objectType)
        {
            return NcasDbs.GetTableOrm(objectType);
        }

        public override bool ProcessPresentationGroup(PerformPresentationGroup performPresentationGroup, string msg)
        {
            if (performPresentationGroup == null)
                return false;

            try
            {
                if (performPresentationGroup.AlarmTransmitterId == null)
                    return true;

                var alarmTransmitter =
                    AlarmTransmitters.Singleton.GetObjectById(performPresentationGroup.AlarmTransmitterId.Value);

                if (alarmTransmitter == null)
                    return false;

                if (performPresentationGroup.PhoneNumbers == null
                    || performPresentationGroup.PhoneNumbers.Length == 0)
                    return true;

                var catSms = new CatSms
                {
                    MessageBody = performPresentationGroup.SmsPrefix + performPresentationGroup.ReturnFormattedMessage(msg),
                    Recipients = performPresentationGroup.PhoneNumbers
                };

                CatComClient.Singleton.SendSms(IPAddress.Parse(alarmTransmitter.IpAddress), catSms);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool CodeAlreadyUsed(string codeHashValue)
        {
            return CardReaders.Singleton.CodeAlreadyUsed(codeHashValue)
                || DevicesAlarmSettings.Singleton.CodeAlreadyUsed(codeHashValue);
        }
    }
}

