using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.BaseLib;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Globals;
using Contal.Cgp.ORM;
using Contal.IwQuick;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.DB
{
    public sealed partial class Eventlogs : AMbrSingleton<Eventlogs>, IEventlogs
    {
        private const string EVENTLOG_TABLE_NAME = "Eventlog";
        private const string EVENTLOG_ALIAS = "e";
        private const string CENTRAL_NAME_REGISTER_TABLE_NAME = "CentralNameRegister";
        private const string EVENT_SOURCE_ALIAS = "es";
        private const string TMP_TABLE_CENTRAL_NAME_REGIST_ID_NAME = "TmpTableCentralNameRegisterId";
        private const string SERVER_SESION_ID = "Server";

        private readonly List<string> _eventTypes = new List<string>();

        private readonly Dictionary<string, SqlCommand> _runningQueries =
            new Dictionary<string, SqlCommand>();

        public DbConnectionManager DbConnectionManager { get; private set; }

        public string LastReportFile;

        private Eventlogs() : base(null)
        {
            ConnectionString connectionString =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);

            if (connectionString == null || !connectionString.IsValid())
                connectionString =
                    ConnectionString.LoadFromRegistry(
                        CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            if (connectionString != null &&
                connectionString.IsValid())
            {
                _eventlogDatabaseName = connectionString.DatabaseName;

                InsertCommandFactory =
                    new InsertCommandFactoryType();

                _getByIdCommandFactory =
                    new GetByIdCommandFactory();

                DbConnectionManager =
                    new DbConnectionManager(connectionString);
            }

            FillEventTypes();
            DropAllTmpTablesCentralNameRegisterId();
        }

        private void FillEventTypes()
        {
            _eventTypes.Clear();

            _eventTypes.Add(Eventlog.TYPECONNECTDATABASE);
            _eventTypes.Add(Eventlog.TYPEDISCONNECTDATABASE);
            _eventTypes.Add(Eventlog.TYPECLIENTCONNECT);
            _eventTypes.Add(Eventlog.TYPECLIENTDISCONNECT);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGIN);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGOUT);
            _eventTypes.Add(Eventlog.TYPECLIENTLOGINWRONGPASSWORD);
            _eventTypes.Add(Eventlog.TYPEPRESENTATIONPROCESSOR);
            _eventTypes.Add(Eventlog.TYPEINITIALIZEPLUGIN);
            _eventTypes.Add(Eventlog.TYPELOADPLUGINSUCCESFUL);
            _eventTypes.Add(Eventlog.TYPEALARMOCCURED);
            _eventTypes.Add(Eventlog.TYPEALARMAREAACTIVATIONSTATECHANGED);
            _eventTypes.Add(Eventlog.TYPEACTALARMACKNOWLEDGED);
            _eventTypes.Add(Eventlog.TYPEINACTALARMACKNOWLEDGED);
            _eventTypes.Add(Eventlog.TYPEBINDINGEVENTFAILED);
            _eventTypes.Add(Eventlog.TYPERUNMETHODFAILED);
            _eventTypes.Add(Eventlog.TYPE_CCU_EXCEPTION_OCCURRED);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIED);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDCODE);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE);
            _eventTypes.Add(Eventlog.TYPEUNKNOWNCARD);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDCODE);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDCODE);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS);
            _eventTypes.Add(Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS);
            _eventTypes.Add(Eventlog.TYPESETALARMAREAFROMCARDREADER);
            _eventTypes.Add(Eventlog.TYPEUNSETALARMAREAFROMCARDREADER);
            _eventTypes.Add(Eventlog.TYPESETALARMAREABYOBJECTFORAA);
            _eventTypes.Add(Eventlog.TYPEUNSETALARMAREABYOBJECTFORAA);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSPERMITTED);
            _eventTypes.Add(Eventlog.TYPEDSMAPASRESTORED);
            _eventTypes.Add(Eventlog.TYPEDSMNORMALACCESS);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSINTERRUPTED);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSRESTRICTED);
            _eventTypes.Add(Eventlog.TYPEDSMACCESSVIOLATED);
            _eventTypes.Add(Eventlog.TYPEDCUONLINE);
            _eventTypes.Add(Eventlog.TYPEDCUOFFLINE);
            _eventTypes.Add(Eventlog.TYPEDCUUPGRADING);
            _eventTypes.Add(Eventlog.TYPEDCUSUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPECEUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPECCUUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPECARDREADERSUPGRADEFAILED);
            _eventTypes.Add(Eventlog.TYPEDCUWAITINGFORUPGRADE);
            _eventTypes.Add(Eventlog.TYPEDCUAUTOUPGRADING);
            _eventTypes.Add(Eventlog.TYPECREATEDNEWDCU);
            _eventTypes.Add(Eventlog.TYPEDCUCOMMANDTIMEOUT);
            _eventTypes.Add(Eventlog.TYPEDCUCOMMANDNACK);
            _eventTypes.Add(Eventlog.TYPEDCUNODASSIGNED);
            _eventTypes.Add(Eventlog.TYPEDCUNODRENEWED);
            _eventTypes.Add(Eventlog.TYPEDCUNODRELEASED);
            _eventTypes.Add(Eventlog.TYPEFAILEDTOUPDATESTRUCTUREONCCU);
            _eventTypes.Add(Eventlog.TYPE_CARDSYSTEM_ADDED);
            _eventTypes.Add(Eventlog.TYPE_CARDSYSTEM_REMOVED);
            _eventTypes.Add(Eventlog.TYPE_CARD_ENCODED);
            _eventTypes.Add(Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_DAILY_PLAN);
            _eventTypes.Add(Eventlog.TYPE_ACTUAL_STATE_OF_SECURITY_TIME_ZONE);
            _eventTypes.Add(Eventlog.TYPE_TIME_FOR_NEXT_EVALUATING_STATES_OF_SDP);
            _eventTypes.Add(Eventlog.TYPE_COPROCESSOR_FAILURE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CCU_TIME_ADJUSTED);
            _eventTypes.Add(Eventlog.TYPE_CCU_OBJECT_DESERIALIZE_FAILED);
            _eventTypes.Add(Eventlog.TYPE_CCU_TIMING_PROBLEM);
            _eventTypes.Add(Eventlog.TYPE_DSM_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_PRIMARY_POWER_MISSING);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_BATTERY_IS_LOW);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_FUSE_ON_EXTENSION_BOARD);
            _eventTypes.Add(Eventlog.TYPE_CCU_TAMPER_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_DCU_TAMPER_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CARD_READER_TAMPER_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CCU_GETFROMDATABASE_RETURN_NULL);
            _eventTypes.Add(Eventlog.TYPE_CCU_DATA_CHANNEL_TRANSFER_FAILED);
            _eventTypes.Add(Eventlog.TYPE_CCU_INCOMING_TRANSFER_INFO);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_SETUNSET_NOT_RESPOND);
            _eventTypes.Add(Eventlog.TYPE_ICCU_SENDING_OF_OBJECT_STATE_FAILED);
            _eventTypes.Add(Eventlog.TYPE_ICCU_PORT_ALREADY_USED);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_SET_FROM_CR_FAILED);
            _eventTypes.Add(Eventlog.TYPE_CCU_INCOMING_AUTONOMOUS_EVENTS);
            _eventTypes.Add(Eventlog.TYPE_CCU_AUTONOMOUS_EVENTS_PROCESSED);
            _eventTypes.Add(Eventlog.TYPE_CCU_AUTONOMOUS_EVENTS_PROCESSING_CANCELED);
            _eventTypes.Add(Eventlog.TYPE_INPUT_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_OUTPUT_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CARD_READER_ONLINE_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_CCU_ONLINE);
            _eventTypes.Add(Eventlog.TYPE_CCU_OFFLINE);
            _eventTypes.Add(Eventlog.TYPEDATABASEBACKUP);
            _eventTypes.Add(Eventlog.TYPE_CCU_MEMORY_LOAD);
            _eventTypes.Add(Eventlog.TYPE_CCU_FILESYSTEM_PROBLEM);
            _eventTypes.Add(Eventlog.TYPE_CCU_PROCESSING_STREAM_TAKES_TOO_LONG_TIME);
            _eventTypes.Add(Eventlog.TYPE_CCU_SD_CARD_NOT_FOUND);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_BOUGHT_TIME_EXPIRED);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_TIME_BUYING_FAILED);
            _eventTypes.Add(Eventlog.TYPE_FUNCTIONKEY_PRESSED);
            _eventTypes.Add(Eventlog.TYPE_ALARM_AREA_ALARM_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_SET_ALARM_AREA_FROM_CLIENT);
            _eventTypes.Add(Eventlog.TYPE_UNSET_ALARM_AREA_FROM_CLIENT);
            _eventTypes.Add(Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_ENTERED);
            _eventTypes.Add(Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_EXITED);
            _eventTypes.Add(Eventlog.TYPE_ANTI_PASS_BACK_ZONE_CARD_TIMED_OUT);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_UPS_OUTPUT_FUSE);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_UPS_BATTERY_FAULT);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_UPS_BATTERY_FUSE);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_UPS_OVERTEMPERATURE);
            _eventTypes.Add(Eventlog.TYPE_CCU_ALARM_UPS_TAMPER);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_PIN);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_CODE);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_PIN);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_CODE);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_PIN);
            _eventTypes.Add(Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_CODE);
            _eventTypes.Add(Eventlog.TYPE_TIMETEC_COMMUNICATION_ONLINE_STATE_CHANGED);
            _eventTypes.Add(Eventlog.TYPE_TIMETEC_EVENT_TRANSFER_FAILED);
            _eventTypes.Add(Eventlog.TYPE_TIMETEC_EVENT_SAVE_OBJECT);
            _eventTypes.Add(Eventlog.TYPE_TIMETEC_EVENT_DELETE_OBJECT);
            _eventTypes.Add(Eventlog.TYPE_CCU_CE_VERSIONS_WERE_LOADED);
        }

        public Eventlog GetObjectById(long id)
        {
            Eventlog result;

            DbConnectionHolder dbConnectionHolder = null;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                SqlCommand sqlCommand =
                    dbConnectionHolder.GetCommand(_getByIdCommandFactory);

                sqlCommand.Parameters[GetByIdCommandFactory.PARAM_ID_EVENTLOG].Value = id;

                SqlDataReader sqlDataReader = null;

                try
                {
                    sqlDataReader =
                        sqlCommand.ExecuteReader(CommandBehavior.SingleRow);

                    if (!sqlDataReader.Read())
                        return null;

                    result =
                        new Eventlog(
                            sqlDataReader.GetString(0),
                            sqlDataReader.GetDateTime(1),
                            sqlDataReader.GetString(2),
                            sqlDataReader.GetString(3))
                        {
                            IdEventlog = id
                        };
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (sqlDataReader != null && !sqlDataReader.IsClosed)
                        try
                        {
                            sqlDataReader.Close();
                        }
                        catch
                        {
                        }
                }
                EventSources.Singleton.GetSourcesForEventlog(
                    result,
                    dbConnectionHolder);

                EventlogParameters.Singleton.GetParametersForEventlog(
                    result,
                    dbConnectionHolder);
            }
            finally
            {
                if (dbConnectionHolder != null)
                    DbConnectionManager.Return(dbConnectionHolder);
            }

            return result;
        }

        public List<string> GetEventLogTypes()
        {
            return _eventTypes;
        }

        #region ITableORM<Logins> Members

        //        private readonly ConnectionString _connectionString;

        /// <summary>
        /// Overrided method for select count using ADO.NET
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="subSitesFilter"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int SelectCount(
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            out Exception error)
        {
            error = null;

            DbConnectionHolder dbConnectionHolder = null;

            string sessionId = ClientIdentificationServerSink.CallingSessionId;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                SqlCommand selectCountSqlCommand;

                lock (_runningQueries)
                {
                    // Drop old temporary table from central name register if exist
                    DropTmpTableCentralNameRegisterId(
                        dbConnectionHolder.SqlConnection,
                        sessionId);

                    var sqlCommandCount = GetSqlCommandCount(
                        filterSettings,
                        subSitesFilter,
                        EVENTLOG_TABLE_NAME,
                        sessionId);

                    if (sqlCommandCount == null)
                        return 0;

                    selectCountSqlCommand =
                        new SqlCommand(
                            sqlCommandCount,
                            dbConnectionHolder.SqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);

                    _runningQueries.Add(sessionId, selectCountSqlCommand);
                }

                return (int)selectCountSqlCommand.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (dbConnectionHolder != null)
                    try
                    {
                        // Drop temporary table from central name register if was created
                        DropTmpTableCentralNameRegisterId(
                            dbConnectionHolder.SqlConnection,
                            sessionId);

                        DbConnectionManager.Return(dbConnectionHolder);
                    }
                    catch
                    {
                    }

                lock (_runningQueries)
                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);
            }
        }

        /// <summary>
        /// Return sql command for get count of selected eventlogs
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="subSitesFilter"></param>
        /// <param name="tableNameForSelect"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private string GetSqlCommandCount(
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            string tableNameForSelect,
            string sessionId)
        {
            var filterSettingsCommand = GetFilterAndSubsitesSettingsCommand(null, filterSettings, sessionId, subSitesFilter, false);

            if (filterSettingsCommand == null)
                return null;

            return
                string.Format(
                    "Select count({0}) from {1} as {2}{3}",
                    Eventlog.COLUMN_ID_EVENTLOG,
                    tableNameForSelect,
                    EVENTLOG_ALIAS,
                    filterSettingsCommand);
        }

        /// <summary>
        /// Abort query for reading eventlogs
        /// </summary>
        public void AbortCurrentQuery()
        {
            string sessionId = ClientIdentificationServerSink.CallingSessionId;

            lock (_runningQueries)
            {
                SqlCommand sqlCommand;

                if (!_runningQueries.TryGetValue(
                        sessionId,
                        out sqlCommand))
                    return;

                if (sqlCommand != null)
                    try
                    {
                        sqlCommand.Cancel();
                    }
                    catch
                    {
                    }

                _runningQueries.Remove(sessionId);
            }
        }

        /// <summary>
        /// Overrided method for select range by criteria using ADO.NET
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="subSitesFilter"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <returns></returns>
        public ICollection<Eventlog> SelectRangeByCriteria(
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            int firstResult,
            int maxResults)
        {
            SqlDataReader dataReader = null;

            DbConnectionHolder dbConnectionHolder = null;
            string sessionId = ClientIdentificationServerSink.CallingSessionId;

            try
            {
                SqlCommand selectEventlogsSqlCommand;

                lock (_runningQueries)
                {
                    dbConnectionHolder = DbConnectionManager.Get();

                    // Drop old temporary table from central name register if exist
                    DropTmpTableCentralNameRegisterId(
                        dbConnectionHolder.SqlConnection,
                        sessionId);

                    var commandText =
                        GetSqlCommandEventlogs(
                            null,
                            filterSettings,
                            subSitesFilter,
                            EVENTLOG_TABLE_NAME,
                            sessionId,
                            string.Format(
                                "{0} DESC",
                                Eventlog.COLUMN_EVENTLOG_DATE_TIME),
                            firstResult + 1,
                            firstResult + maxResults + 1,
                            false);

                    if (commandText == null)
                        return null;

                    selectEventlogsSqlCommand =
                        new SqlCommand(
                            commandText,
                            dbConnectionHolder.SqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);

                    _runningQueries.Add(
                        sessionId,
                        selectEventlogsSqlCommand);
                }

                dataReader = selectEventlogsSqlCommand.ExecuteReader();

                var eventlogs = new List<Eventlog>();

                while (dataReader.Read())
                    eventlogs.Add(
                        new Eventlog
                        {
                            IdEventlog = dataReader.GetInt64(0),
                            Type = dataReader.GetString(1),
                            EventlogDateTime = dataReader.GetDateTime(2),
                            CGPSource = dataReader.GetString(3),
                            Description = dataReader.GetString(4)
                        });

                return eventlogs;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dataReader != null)
                    try
                    {
                        dataReader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    try
                    {
                        // Drop temporary table from central name register if was created
                        DropTmpTableCentralNameRegisterId(
                            dbConnectionHolder.SqlConnection,
                            sessionId);

                        DbConnectionManager.Return(dbConnectionHolder);
                    }
                    catch
                    {
                    }

                lock (_runningQueries)
                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);
            }
        }

        public ICollection<Eventlog> SelectByCriteria(IList<FilterSettings> filterSettings)
        {
            SqlDataReader dataReader = null;

            DbConnectionHolder dbConnectionHolder = null;

            try
            {
                SqlCommand selectEventlogsSqlCommand;

                lock (_runningQueries)
                {
                    dbConnectionHolder = DbConnectionManager.Get();

                    // Drop old temporary table from central name register if exist
                    DropTmpTableCentralNameRegisterId(
                        dbConnectionHolder.SqlConnection,
                        SERVER_SESION_ID);

                    var commandText =
                        GenerateSqlCommandEventlogs(
                            null,
                            filterSettings,
                            null,
                            EVENTLOG_TABLE_NAME,
                            SERVER_SESION_ID,
                            string.Format(
                                "{0} ASC",
                                Eventlog.COLUMN_ID_EVENTLOG),
                            false);

                    if (commandText == null)
                        return null;

                    selectEventlogsSqlCommand =
                        new SqlCommand(
                            commandText,
                            dbConnectionHolder.SqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (_runningQueries.ContainsKey(SERVER_SESION_ID))
                        _runningQueries.Remove(SERVER_SESION_ID);

                    _runningQueries.Add(
                        SERVER_SESION_ID,
                        selectEventlogsSqlCommand);
                }

                dataReader = selectEventlogsSqlCommand.ExecuteReader();

                var eventlogs = new List<Eventlog>();

                while (dataReader.Read())
                    eventlogs.Add(
                        new Eventlog
                        {
                            IdEventlog = dataReader.GetInt64(1),
                            Type = dataReader.GetString(2),
                            EventlogDateTime = dataReader.GetDateTime(3),
                            CGPSource = dataReader.GetString(4),
                            Description = dataReader.GetString(5)
                        });

                return eventlogs;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dataReader != null)
                    try
                    {
                        dataReader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    try
                    {
                        // Drop temporary table from central name register if was created
                        DropTmpTableCentralNameRegisterId(
                            dbConnectionHolder.SqlConnection,
                            SERVER_SESION_ID);

                        DbConnectionManager.Return(dbConnectionHolder);
                    }
                    catch
                    {
                    }

                lock (_runningQueries)
                    if (_runningQueries.ContainsKey(SERVER_SESION_ID))
                        _runningQueries.Remove(SERVER_SESION_ID);
            }
        }

        /// <summary>
        /// Return actual insert counter
        /// </summary>
        /// <returns></returns>
        public int GetInsertCounter()
        {
            return _insertCounter;
        }

        /// <summary>
        /// Create csv export lines from evenlogs
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="columns"></param>
        /// <param name="separator"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        public IList<string> CreateCSVExportLines(
            IList<FilterSettings> filterSettings,
            IList<string> columns,
            string separator,
            string dateFormat)
        {
            if (columns == null || columns.Count <= 0)
                return null;

            SqlDataReader dataReader = null;

            DbConnectionHolder dbConnectionHolder = null;
            string sessionId = ClientIdentificationServerSink.CallingSessionId;

            try
            {
                SqlCommand selectEventlogsSqlCommand;

                lock (_runningQueries)
                {
                    dbConnectionHolder = DbConnectionManager.Get();

                    // Drop old temporary table from central name register if exist
                    DropTmpTableCentralNameRegisterId(dbConnectionHolder.SqlConnection, sessionId);

                    // Get sql command for select records from database
                    string sqlCommadForExportToCSV =
                        GetSqlCommandForExportToCSV(
                            filterSettings,
                            EVENTLOG_TABLE_NAME,
                            sessionId,
                            columns);

                    if (string.IsNullOrEmpty(sqlCommadForExportToCSV))
                        return null;

                    selectEventlogsSqlCommand =
                        new SqlCommand(
                            sqlCommadForExportToCSV,
                            dbConnectionHolder.SqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);

                    _runningQueries.Add(
                        sessionId,
                        selectEventlogsSqlCommand);
                }

                dataReader = selectEventlogsSqlCommand.ExecuteReader();

                int eventSourcesIndex =
                    columns.IndexOf(Eventlog.COLUMN_EVENTSOURCES);

                var exportValues = new List<object[]>();
                var evenlogsId = new List<long>();

                // Read records from the database
                while (dataReader.Read())
                {
                    evenlogsId.Add(dataReader.GetInt64(0));

                    var lineValues = new object[dataReader.FieldCount];

                    for (int actualFieldIndex = 0;
                            actualFieldIndex < dataReader.FieldCount;
                            actualFieldIndex++)
                        lineValues[actualFieldIndex] =
                            dataReader.GetValue(actualFieldIndex);

                    exportValues.Add(lineValues);
                }

                Dictionary<long, ICollection<string>> eventSourcesNames = null;
                // Get event sources names from the database
                if (eventSourcesIndex >= 0 && evenlogsId.Count > 0)
                    eventSourcesNames = GetEventSourceNames(evenlogsId);

                if (exportValues.Count <= 0)
                    return null;

                var exportLines = new List<string>();

                foreach (object[] lineValues in exportValues)
                {
                    if (lineValues == null ||
                            lineValues.Length <= 0 ||
                            !(lineValues[0] is long))
                        continue;

                    string strEventSourcesNames = string.Empty;

                    // Get event sources names for eventlog
                    if (eventSourcesNames != null)
                    {
                        var eventlogId = (long)lineValues[0];

                        ICollection<string> names;
                        if (
                            eventSourcesNames.TryGetValue(
                                eventlogId,
                                out names) &&
                            names != null &&
                            names.Count > 0)
                        {
                            string eventSourcesSeparator = ",";
                            if (separator == eventSourcesSeparator)
                                eventSourcesSeparator = ";";

                            foreach (string name in names)
                            {
                                if (strEventSourcesNames != string.Empty)
                                    strEventSourcesNames +=
                                        eventSourcesSeparator;

                                strEventSourcesNames += name;
                            }
                        }
                    }

                    string currentLine = string.Empty;

                    // If event sources is first column add event sources names to the line
                    if (eventSourcesIndex == 0)
                        currentLine = strEventSourcesNames + separator;

                    for (int actualValuesIndex = 1;
                        actualValuesIndex < lineValues.Length;
                        actualValuesIndex++)
                    {
                        if (actualValuesIndex > 1)
                            currentLine += separator;

                        // Add event sources names to the line if event sources column index is actual index
                        if (eventSourcesIndex != 0 && actualValuesIndex - 1 == eventSourcesIndex)
                            currentLine +=
                                strEventSourcesNames + separator;

                        object value = lineValues[actualValuesIndex];

                        if (value == null)
                            continue;

                        if (value is DateTime)
                        {
                            var dateTime = (DateTime)value;

                            currentLine +=
                                dateTime.ToString(dateFormat);
                        }
                        else
                            currentLine += value.ToString();
                    }

                    // If event sources is last column add event sources names to the line
                    if (eventSourcesIndex != 0 &&
                        eventSourcesIndex + 1 >= lineValues.Length)
                        currentLine += separator + strEventSourcesNames;

                    exportLines.Add(currentLine);
                }

                return exportLines;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dataReader != null)
                    try
                    {
                        dataReader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    try
                    {
                        // Drop temporary table from central name register if was created
                        DropTmpTableCentralNameRegisterId(
                            dbConnectionHolder.SqlConnection,
                            sessionId);

                        DbConnectionManager.Return(dbConnectionHolder);
                    }
                    catch
                    {
                    }

                lock (_runningQueries)
                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);
            }
        }

        /// <summary>
        /// Return sql command for select eventlogs
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="subSitesFilter"></param>
        /// <param name="tableNameForSelect"></param>
        /// <param name="sessionId"></param>
        /// <param name="firstResult"></param>
        /// <param name="lastResult"></param>
        /// <param name="orderByColumn"></param>
        /// <returns></returns>
        private string GetSqlCommandEventlogs(
            Login login,
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            string tableNameForSelect,
            string sessionId,
            string orderByColumn,
            int firstResult,
            int lastResult,
            bool orSubsites)
        {
            string innerSelect = GenerateSqlCommandEventlogs(login, filterSettings, subSitesFilter, tableNameForSelect, sessionId, orderByColumn, orSubsites);

            if (innerSelect == null)
                return null;

            return
                string.Format(
                    "Select {0}, {1}, {2}, {3}, {4} from ({5}) allEventlogsRecords WHERE allEventlogsRecords.actualRowNumber BETWEEN {6} AND {7}",
                    Eventlog.COLUMN_ID_EVENTLOG,
                    Eventlog.COLUMN_TYPE,
                    Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                    Eventlog.COLUMN_CGPSOURCE,
                    Eventlog.COLUMN_DESCRIPTION,
                    innerSelect,
                    firstResult,
                    lastResult);
        }

        private string GenerateSqlCommandEventlogs(
            Login login,
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            string tableNameForSelect,
            string sessionId,
            string orderByColumn,
            bool orSubsites)
        {
            var filterSettingsCommand = GetFilterAndSubsitesSettingsCommand(login, filterSettings, sessionId, subSitesFilter, orSubsites);

            if (filterSettingsCommand == null)
                return null;

            return string.Format(
                "Select ROW_NUMBER() OVER (ORDER BY {0}) AS actualRowNumber, {1}, {2}, {3}, {4}, {5} from {6} as {7}{8}",
                orderByColumn,
                Eventlog.COLUMN_ID_EVENTLOG,
                Eventlog.COLUMN_TYPE,
                Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                Eventlog.COLUMN_CGPSOURCE,
                Eventlog.COLUMN_DESCRIPTION,
                tableNameForSelect,
                EVENTLOG_ALIAS,
                filterSettingsCommand);
        }

        /// <summary>
        ///  Return sql command for create csv export from eventlogs. Select all records from the database that meets all conditions in the filterSettings.
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="tableNameForSelect"></param>
        /// <param name="sessionId"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetSqlCommandForExportToCSV(
            IList<FilterSettings> filterSettings,
            string tableNameForSelect,
            string sessionId,
            IEnumerable<string> columns)
        {
            var strColumns =
                new StringBuilder(Eventlog.COLUMN_ID_EVENTLOG);

            if (columns != null)
                foreach (string strColumn in columns.Where(strColumn => strColumn != Eventlog.COLUMN_EVENTSOURCES))
                {
                    strColumns.Append(", ");
                    strColumns.Append(strColumn);
                }

            var filterSettingsCommand = GetFilterAndSubsitesSettingsCommand(null, filterSettings, sessionId, null, false);

            if (filterSettingsCommand == null)
                return null;

            return
                string.Format(
                    "Select {0} from {1} as {2}{3} ORDER BY {4} DESC",
                    strColumns,
                    tableNameForSelect,
                    EVENTLOG_ALIAS,
                    filterSettingsCommand,
                    Eventlog.COLUMN_EVENTLOG_DATE_TIME);
        }

        /// <summary>
        /// Return condition for sql command from filter settings
        /// </summary>
        /// <param name="filterSettings"></param>
        /// <param name="sessionId"></param>
        /// /// <param name="subSitesFilter"></param>
        /// <returns></returns>
        private string GetFilterAndSubsitesSettingsCommand(
            Login login,
            IList<FilterSettings> filterSettings,
            string sessionId,
            ICollection<int> subSitesFilter,
            bool orSubsites)
        {
            string filterSettingsCommand = string.Empty;
            if (filterSettings != null || filterSettings.Count > 0)
            {
                var filterSettingsCommandGenerator =
                    new FilterSettingsCommandGenerator(
                        this,
                        sessionId);

                filterSettingsCommandGenerator.Parse(filterSettings);

                filterSettingsCommand = filterSettingsCommandGenerator.Generate();
            }

            if (sessionId == SERVER_SESION_ID)
                return filterSettingsCommand;

            string structuredSubSiteConditionForLogin = StructuredSubSites.Singleton.GetEventlogSubSiteConditionForLogin(
                login, EVENTLOG_ALIAS, EVENT_SOURCE_ALIAS, subSitesFilter, orSubsites);

            if (structuredSubSiteConditionForLogin == null)
                return null;

            if (structuredSubSiteConditionForLogin == string.Empty)
            {
                return filterSettingsCommand;
            }

            if (filterSettingsCommand == string.Empty)
                return string.Format(" WHERE {0}", structuredSubSiteConditionForLogin);

            return string.Format("{0} AND ({1})", filterSettingsCommand, structuredSubSiteConditionForLogin);
        }

        /// <summary>
        /// Drop temporary table from central name register
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sessionId"></param>
        private void DropTmpTableCentralNameRegisterId(
            SqlConnection connection,
            string sessionId)
        {
            DropTable(
                connection,
                TMP_TABLE_CENTRAL_NAME_REGIST_ID_NAME + sessionId);
        }

        /// <summary>
        /// Drop table
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        private static void DropTable(SqlConnection connection, string tableName)
        {
            try
            {
                string command =
                    string.Format(
                        "IF EXISTS (SELECT 1 FROM sysobjects WHERE xtype='u' AND name='{0}') Drop table [{0}]",
                        tableName);

                var sqlCommand = new SqlCommand(command, connection);

                sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Drop all temporary tables from central name register
        /// </summary>
        private void DropAllTmpTablesCentralNameRegisterId()
        {
            SqlDataReader dataRader = null;

            DbConnectionHolder dbConnectionHolder = null;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                string command =
                    string.Format(
                        "SELECT name FROM sysobjects WHERE xtype='u' AND name like '{0}%'",
                        TMP_TABLE_CENTRAL_NAME_REGIST_ID_NAME);

                var sqlCommand =
                    new SqlCommand(
                        command,
                        dbConnectionHolder.SqlConnection);

                dataRader = sqlCommand.ExecuteReader();

                var tablesToDrop = new List<string>();

                while (dataRader.Read())
                    tablesToDrop.Add((string)dataRader.GetValue(0));

                dataRader.Close();
                dataRader = null;

                if (tablesToDrop.Count > 0)
                    foreach (string tableName in tablesToDrop)
                        DropTable(
                            dbConnectionHolder.SqlConnection,
                            tableName);
            }
            catch
            {
            }
            finally
            {
                if (dataRader != null)
                    try
                    {
                        dataRader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        /// <summary>
        /// Create temporary table from central name register for objects which name contains input string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sessionId"></param>
        /// <returns>Temporary table name</returns>
        private string CreateTmpTableCentralNameRegisterId(string name, string sessionId)
        {
            string tmpTableCentralNameRegisterId =
                TMP_TABLE_CENTRAL_NAME_REGIST_ID_NAME + sessionId;

            ConnectionString connectionString = NhHelper.Singleton.ConnectionString;

            if (connectionString == null || !connectionString.IsValid())
                return tmpTableCentralNameRegisterId;

            SqlConnection sqlConnection = null;

            try
            {
                string command =
                    string.Format(
                        "select {0} into [{1}].[dbo].[{2}] from {3} where {4} like '%{5}%' or {6} like '%{7}%' or {8} like '%{9}%'",
                        CentralNameRegister.COLUMN_ID,
                        _eventlogDatabaseName ?? "",
                        tmpTableCentralNameRegisterId,
                        CENTRAL_NAME_REGISTER_TABLE_NAME,
                        CentralNameRegister.COLUMN_NAME,
                        name,
                        CentralNameRegister.COLUMN_ALTERNATE_NAME,
                        name,
                        CentralNameRegister.COLUMN_FULL_TEXT_SEARCH_STRING,
                        name);

                sqlConnection =
                    new SqlConnection(
                        connectionString.ToString());

                sqlConnection.Open();

                var sqlCommand =
                    new SqlCommand(
                        command,
                        sqlConnection)
                    {
                        CommandTimeout = 7200
                    };

                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                if (sqlConnection != null)
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch
                    {
                    }
            }

            return tmpTableCentralNameRegisterId;
        }

        /// <summary>
        /// Return formated string for DateTime with miliseconds
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string GetDateFromDateTime(DateTime dt)
        {
            return string.Format(
                "{0}-{1}-{2} {3}:{4}:{5}.{6}",
                dt.Year,
                dt.Month,
                dt.Day,
                dt.Hour,
                dt.Minute,
                dt.Second,
                dt.Millisecond.ToString().PadLeft(3, '0'));
        }

        #endregion

        private int _insertCounter;

        private const int DELETE_EXPIRED_EVENTS_TIMEOUT = 1800000;

        public InsertCommandFactoryType InsertCommandFactory
        {
            get;
            private set;
        }
        private readonly GetByIdCommandFactory _getByIdCommandFactory;
        private readonly string _eventlogDatabaseName;

        public void InsertInternal(Eventlog eventlog, SqlCommand insertCommand)
        {
            insertCommand.Parameters[InsertCommandFactoryType.PARAM_TYPE].Value =
                eventlog.Type;

            insertCommand.Parameters[
                InsertCommandFactoryType.PARAM_EVENTLOG_DATE_TIME].Value =
                eventlog.EventlogDateTime;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_CGPSOURCE].Value =
                eventlog.CGPSource;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_DESCRIPTION].Value =
                eventlog.Description;

            eventlog.IdEventlog = (long)insertCommand.ExecuteScalar();

            _insertCounter++;
        }

        public bool InsertEvent(
            string type,
            DateTime date,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description,
            IEnumerable<EventlogParameter> parameters)
        {
            try
            {
                var eventlog =
                    new Eventlog(
                        type,
                        date,
                        cgpSource,
                        description);

                var eventlogInsertItem =
                    new EventlogInsertItem(
                        eventlog,
                        eventSourceObjectGuids != null
                            ? eventSourceObjectGuids
                                .Select(eventSourceGuid =>
                                    new EventSource(
                                        eventSourceGuid,
                                        eventlog))
                            : null,
                        parameters);

                EventlogInsertQueue.Singleton.Enqueue(eventlogInsertItem);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateEvent(
            string type,
            DateTime date,
            string cgpSource,
            IEnumerable<Guid> eventSourceObjectGuids,
            string description,
            out Eventlog eventlog,
            out List<EventSource> eventSources)
        {
            eventSources = new List<EventSource>();
            eventlog = null;

            try
            {
                eventlog = new Eventlog(type, date, cgpSource, description);

                if (eventSourceObjectGuids == null)
                    return true;

                Eventlog eventlog1 = eventlog;

                eventSources.AddRange(
                    eventSourceObjectGuids
                        .Select(
                            eventSourceGuid =>
                                new EventSource(
                                    eventSourceGuid,
                                    eventlog1)));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Dictionary<long, ICollection<string>> GetEventSourceNames(IList<long> eventlogsId)
        {
            if (eventlogsId == null || eventlogsId.Count == 0)
                return null;

            var result = new Dictionary<long, ICollection<string>>();

            DbConnectionHolder dbConnectionHolder = null;

            SqlDataReader dataReader = null;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                string strEventlogsId = string.Empty;

                foreach (long eventlogId in eventlogsId)
                {
                    if (strEventlogsId != string.Empty)
                        strEventlogsId += ", ";

                    strEventlogsId += string.Format("'{0}'", eventlogId);
                }

                string command =
                    string.Format(
                        "select {0}, {1} from {2} where {3} in ({4})",
                        EventSource.COLUMN_EVENTLOG_ID,
                        EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                        EventSources.EVENTSOURCE_TABLE_NAME,
                        EventSource.COLUMN_EVENTLOG_ID,
                        strEventlogsId);

                var sqlCommand =
                    new SqlCommand(
                        command,
                        dbConnectionHolder.SqlConnection)
                    {
                        CommandTimeout = 7200
                    };

                dataReader = sqlCommand.ExecuteReader();

                var dicIdEventlogEventSources =
                    new Dictionary<long, ICollection<Guid>>();

                while (dataReader.Read())
                {
                    var actEventlogId = dataReader.GetInt64(0);
                    var actEventSourceObjectGuid = dataReader.GetGuid(1);

                    ICollection<Guid> eventSources;

                    if (!dicIdEventlogEventSources.TryGetValue(
                        actEventlogId,
                        out eventSources))
                    {
                        eventSources = new LinkedList<Guid>();

                        dicIdEventlogEventSources.Add(
                            actEventlogId,
                            eventSources);
                    }

                    eventSources.Add(actEventSourceObjectGuid);
                }

                if (dicIdEventlogEventSources.Count == 0)
                    return result;

                var cnrs =
                    CentralNameRegisters.Singleton
                        .List()
                        .ToDictionary(item => item.Id);

                var objectsNameCache = new Dictionary<IdAndObjectType, string>();

                foreach (var kvPair in dicIdEventlogEventSources)
                {
                    var listTp = new LinkedList<ObjTypePriority>();

                    foreach (var eventSourceObjectGuid in kvPair.Value)
                    {
                        CentralNameRegister cnr;

                        if (!cnrs.TryGetValue(eventSourceObjectGuid, out cnr))
                            continue;

                        if (cnr == null)
                            continue;

                        listTp.AddLast(
                            new ObjTypePriority(
                                cnr,
                                objectsNameCache));
                    }

                    if (listTp.Count == 0)
                        continue;

                    var objTypePriorities =
                        GeneralOptions.Singleton.EventSourcesReverseOrder
                            ? listTp.OrderBy(x => x.Priority)
                            : listTp.OrderByDescending(x => x.Priority);

                    result.Add(
                        kvPair.Key,
                        new LinkedList<string>(
                            objTypePriorities
                                .Select(objTypePriority => objTypePriority.Name)
                                .ToList()));
                }

                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dataReader != null)
                    try
                    {
                        dataReader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public bool InsertEvent(
            string type,
            DateTime date,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description,
            params EventlogParameter[] parameters)
        {
            return InsertEvent(
                type,
                date,
                cgpSource,
                eventSourceObjectGuids,
                description,
                parameters as IEnumerable<EventlogParameter>);
        }

        public bool InsertEvent(
            string type,
            DateTime date,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description)
        {
            try
            {
                var eventlog = new Eventlog(type, date, cgpSource, description);

                var eventlogInsertItem =
                    new EventlogInsertItem(
                        eventlog,
                        eventSourceObjectGuids != null
                            ? eventSourceObjectGuids
                                .Select(eventSourceGuid =>
                                    new EventSource(
                                        eventSourceGuid,
                                        eventlog))
                            : null,
                        null);

                EventlogInsertQueue.Singleton.Enqueue(eventlogInsertItem);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertEvent(
            string type,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description,
            IEnumerable<EventlogParameter> parameters)
        {
            return
                InsertEvent(
                    type,
                    DateTime.Now,
                    cgpSource,
                    eventSourceObjectGuids,
                    description,
                    parameters);
        }

        public bool InsertEvent(
            string type,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description)
        {
            return
                InsertEvent(
                    type,
                    DateTime.Now,
                    cgpSource,
                    eventSourceObjectGuids,
                    description);
        }

        public bool InsertEvent(
            string type,
            string cgpSource,
            Guid[] eventSourceObjectGuids,
            string description,
            params EventlogParameter[] parameters)
        {
            try
            {
                var uniqueEventlogParameters = new Dictionary<string, EventlogParameter>();

                foreach (EventlogParameter eventlogParameter in parameters)
                {
                    var parameterType = eventlogParameter.Type;

                    if (string.IsNullOrEmpty(parameterType))
                        continue;

                    if (uniqueEventlogParameters.ContainsKey(parameterType))
                        return false;

                    uniqueEventlogParameters.Add(
                        parameterType,
                        eventlogParameter);
                }

                return
                    InsertEvent(
                        type,
                        DateTime.Now,
                        cgpSource,
                        eventSourceObjectGuids,
                        description,
                        uniqueEventlogParameters.Values);
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<EventlogParameter> ParseEventlogParameters(
            string[] parameters)
        {
            string logName = string.Empty;

            IDictionary<string, EventlogParameter> result =
                new Dictionary<string, EventlogParameter>();

            try
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        logName = parameters[i];
                        continue;
                    }

                    if (string.IsNullOrEmpty(logName))
                        continue;

                    // SB
                    //if (result.ContainsKey(logName))
                    //    return null;

                    EventlogParameter eventlogParameter = null;

                    switch (logName)
                    {
                        case "CardReaderGuid":
                            try
                            {
                                eventlogParameter =
                                    new EventlogParameter(
                                        "Card reader",
                                        "GuidCR",
                                        new Guid(parameters[i]),
                                        ObjectType.CardReader);
                            }
                            catch
                            {
                            }
                            break;

                        case "PushButtonGuid":
                            try
                            {
                                eventlogParameter =
                                    new EventlogParameter(
                                        "Push button",
                                        "GuidInput",
                                        new Guid(parameters[i]),
                                        ObjectType.Input);
                            }
                            catch
                            {
                            }
                            break;

                        default:
                            eventlogParameter =
                                new EventlogParameter(
                                    logName,
                                    parameters[i]);
                            break;
                    }

                    if (eventlogParameter != null)
                        // SB - Return unique results
                        //result.Add(logName, eventlogParameter);
                        result[logName] = eventlogParameter;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return result.Values;
        }

        public bool CreateEvent(
            string type,
            DateTime date,
            string cgpSource,
            IEnumerable<Guid> eventSourceObjectGuids,
            string description,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters,
            params string[] parameters)
        {
            eventlog = null;
            eventSources = new List<EventSource>();
            eventlogParameters = new List<EventlogParameter>();

            if (parameters != null && (parameters.Length % 2) != 0)
                return false;

            if (!CreateEvent(
                    type,
                    date,
                    cgpSource,
                    eventSourceObjectGuids,
                    description,
                    out eventlog,
                    out eventSources))
                return false;

            if (parameters != null)
            {
                var result = ParseEventlogParameters(parameters);

                if (result == null)
                    return false;

                eventlogParameters = result.ToList();
            }

            return true;
        }

        public bool HasAccessView()
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.EVENTLOGS), AccessChecker.GetActualLogin());
        }

        public bool HasAccessViewForObject(Eventlog eventlog)
        {
            return HasAccessView();
        }

        public bool HasAccessExport()
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.EventlogsPerform), AccessChecker.GetActualLogin());
        }

        protected IList<AOrmObject> GetReferencedObjects(object idObj)
        {
            return null;
        }

        private bool RunSlqCommandFromStream(
            Stream commandTextStream,
            int commandTimeout,
            IEnumerable<SqlParameter> parameters)
        {
            string commandText;
            StreamReader streamReader = null;

            try
            {
                streamReader = new StreamReader(commandTextStream);

                commandText = streamReader.ReadToEnd();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
            }

            DbConnectionHolder dbConnectionHolder = null;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                var sqlCommand =
                    new SqlCommand(
                        commandText,
                        dbConnectionHolder.SqlConnection)
                    {
                        CommandTimeout = commandTimeout
                    };

                foreach (var sqlParameter in parameters)
                    sqlCommand.Parameters.Add(sqlParameter);

                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            finally
            {
                if (dbConnectionHolder != null)
                    DbConnectionManager.Return(dbConnectionHolder);
            }

            return true;
        }

        private static IEnumerable<SqlParameter> DeleteEventlogByExpiratedDaysParameters(int days)
        {
            yield return
                new SqlParameter("@DateTime", SqlDbType.DateTime)
                {
                    Value = DateTime.Now.AddDays(-days)
                };
        }

        public bool DeleteEventlogByExpiratedDays(int days)
        {
            Type currentType = GetType();

            return
                RunSlqCommandFromStream(
                    currentType.Assembly.GetManifestResourceStream(
                        currentType.Namespace + ".DeleteEventlogByExpiratedDays.sql"),
                    DELETE_EXPIRED_EVENTS_TIMEOUT,
                    DeleteEventlogByExpiratedDaysParameters(days));
        }

        private static IEnumerable<SqlParameter> DeleteEventlogByExpiratedCountParameters(int maximumRecords)
        {
            yield return
                new SqlParameter("@maxRecords", SqlDbType.Int)
                {
                    Value = maximumRecords
                };
        }

        public bool DeleteEventlogByExpiratedCount(int maximumRecords)
        {
            Type currentType = GetType();

            return
                RunSlqCommandFromStream(
                    currentType.Assembly.GetManifestResourceStream(
                        currentType.Namespace + ".DeleteEventlogByExpiratedCount.sql"),
                    DELETE_EXPIRED_EVENTS_TIMEOUT,
                    DeleteEventlogByExpiratedCountParameters(maximumRecords));
        }

        public void EnsureNewEventSourceStructure()
        {
            string commandText = "declare @oldEventSources as INTEGER ";
            commandText += "set @oldEventSources = (select COUNT(*) from EventSources) ";
            commandText += "if (@oldEventSources>0) ";
            commandText += "BEGIN ";
            commandText += "INSERT into EventSource SELECT newid(), IdEventlog, EventSourceGuid from EventSources ";
            commandText += "END ";
            commandText += "DROP TABLE EventSources ";

            DbConnectionHolder dbConnectionHolder = null;

            try
            {
                dbConnectionHolder = DbConnectionManager.Get();

                var sqlCommand =
                    new SqlCommand(commandText)
                    {
                        Connection = dbConnectionHolder.SqlConnection
                    };

                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                if (dbConnectionHolder != null)
                    DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public void InsertEventClientLogin(string loginUserName)
        {
            var parameterList = Enumerable.Repeat(
                new EventlogParameter(EventlogParameter.TYPEUSERNAME, loginUserName),
                1);
            Guid[] eventSources = GetEventSourcesForUserName(loginUserName);
            InsertEvent(Cgp.Server.Beans.Extern.Eventlog.TYPECLIENTLOGIN, this.GetType().Assembly.GetName().Name, eventSources, "Client with USERNAME " + loginUserName + " login", parameterList);
        }

        public void InsertEventClientLogout(string loginUserName)
        {
            var parameterList = Enumerable.Repeat(
                new EventlogParameter(EventlogParameter.TYPEUSERNAME, loginUserName),
                1);
            Guid[] eventSources = GetEventSourcesForUserName(loginUserName);
            InsertEvent(Eventlog.TYPECLIENTLOGOUT, GetType().Assembly.GetName().Name, eventSources, "Client with USERNAME " + loginUserName + " logout", parameterList);
        }

        private Guid[] GetEventSourcesForUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            Guid[] eventSources = null;

            var login = Logins.Singleton.GetLoginByUserName(userName);
            if (login != null)
            {
                if (login.Person != null)
                {
                    eventSources = new Guid[] { login.IdLogin, login.Person.IdPerson };
                }
                else
                {
                    eventSources = new Guid[] { login.IdLogin };
                }
            }

            return eventSources;
        }

        public long GenerateDataForExcelByCriteria(IList<FilterSettings> filterSettings, ICollection<int> subSitesFilter, string dateFormat,
            string sessionId, string emailToSend)
        {
            SqlDataReader dataReader = null;
            long lastId = 0;

            DbConnectionHolder dbConnectionHolder = null;
            if (String.IsNullOrEmpty(sessionId))
                sessionId = ClientIdentificationServerSink.CallingSessionId;

            try
            {
                SqlCommand selectEventlogsSqlCommand;

                lock (_runningQueries)
                {
                    dbConnectionHolder = DbConnectionManager.Get();

                    // Drop old temporary table from central name register if exist
                    DropTmpTableCentralNameRegisterId(
                        dbConnectionHolder.SqlConnection,
                        sessionId);

                    Login login = Logins.Singleton.GetLoginByUserName("admin");

                    var commandText =
                        GetSqlCommandEventlogs(
                            login,
                            filterSettings,
                            subSitesFilter,
                            EVENTLOG_TABLE_NAME,
                            sessionId,
                            string.Format("{0} DESC", Eventlog.COLUMN_EVENTLOG_DATE_TIME),
                            1,
                            10000,
                            true);

                    if (commandText == null)
                        return 0;

                    selectEventlogsSqlCommand =
                        new SqlCommand(
                            commandText,
                            dbConnectionHolder.SqlConnection)
                        {
                            CommandTimeout = 7200
                        };

                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);

                    _runningQueries.Add(
                        sessionId,
                        selectEventlogsSqlCommand);
                }

                dataReader = selectEventlogsSqlCommand.ExecuteReader();

                var eventlogs = new List<Eventlog>();

                while (dataReader.Read())
                {
                    eventlogs.Add(
                        new Eventlog
                        {
                            IdEventlog = dataReader.GetInt64(0),
                            Type = dataReader.GetString(1),
                            EventlogDateTime = dataReader.GetDateTime(2),
                            CGPSource = dataReader.GetString(3),
                            Description = dataReader.GetString(4)
                        });

                    lastId = dataReader.GetInt64(0);
                }     

                ExcelHelper excelHelper = new ExcelHelper();
                excelHelper._FileName = string.Format("{0}\\Report_{1}.xlsx", System.IO.Path.GetTempPath(), DateTime.Now.ToString("yyyy-MM-dd"));
                excelHelper._DateFormat = dateFormat;
                if (!String.IsNullOrEmpty(emailToSend))
                {
                    excelHelper._EmailToSend = emailToSend;
                    excelHelper.OnGenerateFinished += excelHelper.SendEmail;
                }

                // Use the data to create the Excel file
                SafeThread<ICollection<Eventlog>>.StartThread<ICollection<Eventlog>>(excelHelper.GenerateExcelFile, eventlogs);

                return lastId;
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (dataReader != null)
                    try
                    {
                        dataReader.Close();
                    }
                    catch
                    {
                    }

                if (dbConnectionHolder != null)
                    try
                    {
                        // Drop temporary table from central name register if was created
                        DropTmpTableCentralNameRegisterId(
                            dbConnectionHolder.SqlConnection,
                            sessionId);

                        DbConnectionManager.Return(dbConnectionHolder);
                    }
                    catch
                    {
                    }

                lock (_runningQueries)
                    if (_runningQueries.ContainsKey(sessionId))
                        _runningQueries.Remove(sessionId);
            }
        }

        public class InsertCommandFactoryType : DbCommandFactory
        {
            public const string PARAM_TYPE =
                "@" + Eventlog.COLUMN_TYPE;

            public const string PARAM_EVENTLOG_DATE_TIME =
                "@" + Eventlog.COLUMN_EVENTLOG_DATE_TIME;

            public const string PARAM_CGPSOURCE =
                "@" + Eventlog.COLUMN_CGPSOURCE;

            public const string PARAM_DESCRIPTION =
                "@" + Eventlog.COLUMN_DESCRIPTION;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "insert into {0} ({1}, {2}, {3}, {4}) values ({5}, {6}, {7}, {8}); select cast(scope_identity() as bigint)",
                            EVENTLOG_TABLE_NAME,
                            Eventlog.COLUMN_TYPE,
                            Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                            Eventlog.COLUMN_CGPSOURCE,
                            Eventlog.COLUMN_DESCRIPTION,
                            PARAM_TYPE,
                            PARAM_EVENTLOG_DATE_TIME,
                            PARAM_CGPSOURCE,
                            PARAM_DESCRIPTION);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                        new SqlParameter(
                            PARAM_TYPE,
                            SqlDbType.NVarChar,
                            255);

                    yield return
                        new SqlParameter(
                            PARAM_EVENTLOG_DATE_TIME,
                            SqlDbType.DateTime2,
                            3);

                    yield return
                        new SqlParameter(
                            PARAM_CGPSOURCE,
                            SqlDbType.NVarChar,
                            255);

                    yield return
                        new SqlParameter(
                            PARAM_DESCRIPTION,
                            SqlDbType.NVarChar,
                            4000);
                }
            }
        }

        private class GetByIdCommandFactory : DbCommandFactory
        {
            public const string PARAM_ID_EVENTLOG =
                "@" + Eventlog.COLUMN_ID_EVENTLOG;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "select {2}, {3}, {4}, {5} from {0} where {1} = {6}",
                            EVENTLOG_TABLE_NAME,
                            Eventlog.COLUMN_ID_EVENTLOG,
                            Eventlog.COLUMN_TYPE,
                            Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                            Eventlog.COLUMN_CGPSOURCE,
                            Eventlog.COLUMN_DESCRIPTION,
                            PARAM_ID_EVENTLOG);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                        new SqlParameter(
                            PARAM_ID_EVENTLOG,
                            SqlDbType.BigInt);
                }
            }
        }
    }
}
