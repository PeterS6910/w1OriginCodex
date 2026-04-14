using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.ORM;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Contal.Cgp.Server.DB
{
    public sealed class ConsecutiveEvents : AMbrSingleton<ConsecutiveEvents>, IConsecutiveEvent
    {
        public const string TABLE_NAME = "ConsecutiveEvents";
        public const string PARAM_EVENT_ID =
             "@" + ConsecutiveEvent.COLUMN_LASTEVENT_ID;

        public const string PARAM_REASON_ID =
                     "@" + ConsecutiveEvent.COLUMN_REASON_ID;

        public const string PARAM_SOURCE_ID =
                     "@" + ConsecutiveEvent.COLUMN_SOURCE_ID;

        public const string PARAM_LASTEVENT_DT =
             "@" + ConsecutiveEvent.COLUMN_LASTEVENTDATETIME;
        public DbConnectionManager DbConnectionManager { get; private set; }


        private readonly InsertDbCommandForTriggeredEvent _insertCommandTriggeredEvent;
        private readonly UpdateDbCommandForTriggeredEvent _updateCommandTriggeredEvent;
        private readonly SelectDbCommandForTriggeredEvent _selectCommandTriggeredEvent;
        private readonly InsertDbCommandForExcelExport _insertCommandExcel;
        private readonly UpdateDbCommandForExcelExport _updateCommandExcel;
        private readonly SelectDbCommandForExcelExport _selectCommandExcel;
        private readonly DeleteOldDbCommand _deleteCommand;

        //private void AddLogMessage(string message)
        //{
        //    var logFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\logAD.log";
        //    if (!File.Exists(logFile))
        //    {
        //        // Create a file to write to.
        //        using (StreamWriter sw = File.CreateText(logFile))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = File.AppendText(logFile))
        //        {
        //            sw.WriteLine(message);
        //        }
        //    }

        //}
        private ConsecutiveEvents() : base(null)
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
                _insertCommandTriggeredEvent = new InsertDbCommandForTriggeredEvent();
                _updateCommandTriggeredEvent = new UpdateDbCommandForTriggeredEvent();
                _selectCommandTriggeredEvent = new SelectDbCommandForTriggeredEvent();

                _insertCommandExcel = new InsertDbCommandForExcelExport();
                _updateCommandExcel = new UpdateDbCommandForExcelExport();
                _selectCommandExcel = new SelectDbCommandForExcelExport();
                _deleteCommand= new DeleteOldDbCommand();

                DbConnectionManager =
                    new DbConnectionManager(connectionString);
            }
        }
        public ConsecutiveEvent GetConsecutiveTriggeredEvent(Eventlog eventlog)
        {
            var c_event = GetConsecutiveEventCR_Card(eventlog);
            ConsecutiveEvent result=null;
            if (c_event != null && c_event.AccessDeniedType)
                result = GetConsecutiveTriggeredEventInternal(c_event.ReasonId, c_event.SourceId);

            if(result != null)
                return result;
            return c_event;
        }
        public ConsecutiveEvent GetConsecutiveEventCR_Card(Eventlog eventlog)
        {
            if (eventlog != null && eventlog.EventSources == null)
            {
                eventlog = Eventlogs.Singleton.GetObjectById(eventlog.IdEventlog);
              //  AddLogMessage("Load Event sources");
            }

            Guid crGuid = Guid.Empty;
            Guid cardGuid= Guid.Empty;

            if (eventlog.EventSources != null)
            {
                foreach (var eventsource in  eventlog.EventSources)
                {
                    if (crGuid == Guid.Empty && CentralNameRegisters.Singleton.GetObjectTypeFromGuid(eventsource.EventSourceObjectGuid) == ObjectType.CardReader)
                        crGuid = eventsource.EventSourceObjectGuid;

                    if (cardGuid == Guid.Empty && CentralNameRegisters.Singleton.GetObjectTypeFromGuid(eventsource.EventSourceObjectGuid) == ObjectType.Card)
                        cardGuid = eventsource.EventSourceObjectGuid;

                    if (crGuid != Guid.Empty && cardGuid != Guid.Empty)
                    {
                        return new ConsecutiveEvent(eventlog.IdEventlog, eventlog.Type, cardGuid, crGuid, eventlog.EventlogDateTime);
                    }
                }

             //   if (crGuid == Guid.Empty || cardGuid == Guid.Empty)
                   // AddLogMessage($"Card reader or  card is not found for Eventlog {eventlog.IdEventlog}");
            }
            return null;
        }

        public bool TriggerEventlog(Eventlog curlog, int failcount)
        {
          //  AddLogMessage("Start TriggerEventlog");
           // AddLogMessage($"Event log Id: {(curlog!=null? curlog.IdEventlog: -1)}, fails count : {failcount}") ;
            bool res = false;
            var emailEvent = GetConsecutiveTriggeredEvent(curlog);
            if (emailEvent != null && emailEvent.AccessDeniedType)
            {
                var consevents = GetConsecutiveEvents(curlog, emailEvent, 2);
                if (consevents != null)
                {
                    consevents = consevents.OrderByDescending(c => c.EventlogDateTime);
                    if (consevents.Count() >= failcount)
                    {
                        int _count = 0;
                        DateTime dtFirst=DateTime.MinValue;
                        foreach (var eventlog in consevents)
                        {
                            if(_count==0)
                                dtFirst=eventlog.EventlogDateTime;

                            if (!InMinutes(dtFirst, eventlog.EventlogDateTime,60*12))
                            {
                                break;
                            }
                          if(ConsecutiveEvent.IsAccessDeniedTypeMultiple(eventlog.Type))
                            _count++;

                            if (emailEvent.Id != 0 && _count <= failcount && eventlog.IdEventlog == emailEvent.LastEventLogId) // the  consecutive events do include the last sent event registered in DB, do not send for now this event  
                            {
                              //  AddLogMessage($"Not anough count after  last sent event (id: {emailEvent.LastEventLogId}), current event count : {_count-1}");
                                break;
                            }

                            if (_count >= failcount)
                            {
                                res = true;
                                if (emailEvent.Id != 0)
                                    UpdateConsecutiveEventInternal(curlog.IdEventlog, emailEvent);
                                else
                                    InsertConsecutiveEventInternal(curlog.IdEventlog, emailEvent);

                                break;
                            }
                        }
                    }
                }
            }
           // AddLogMessage($"Trigger event result: " +(res? "sent mail": "not sent mail"));
            return res;
        }


        private IEnumerable<Eventlog> GetConsecutiveEvents(Eventlog lastEvenLog, ConsecutiveEvent emailEvent, long dayBack)
        {
            var dateTimeFrom = lastEvenLog.EventlogDateTime.AddDays(-1 * dayBack);
            var filterSettings = new List<FilterSettings>
            {

                 new FilterSettings(
                    Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                    dateTimeFrom,
                    ComparerModes.EQUALLMORE),

                 new FilterSettings(
                    Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                    lastEvenLog.EventlogDateTime,
                    ComparerModes.EQUALLLESS),

              new FilterSettings(
                Eventlog.COLUMN_EVENTSOURCES,
                                new List<Guid>{emailEvent.SourceId,emailEvent.ReasonId},
                                ComparerModes.IN)
            };

            return FilterConsecutiveEvents(filterSettings, emailEvent.ReasonId, emailEvent.SourceId);
        }


        private IEnumerable<Eventlog>  FilterConsecutiveEvents(IList<FilterSettings> filterSettings,  Guid reasonId, Guid sourceId)
        {
            var items = Eventlogs.Singleton.SelectByCriteria(filterSettings);
            foreach (var item in items)
            {
                var ce = GetConsecutiveEventCR_Card(item);

                if (ce != null && ce.ReasonId == reasonId && ce.SourceId == sourceId)
                {
                    yield return item;
                }
            }
        }


        private ConsecutiveEvent GetConsecutiveTriggeredEventInternal(Guid reasonId /*card reader*/, Guid sourceId /*card id*/)
        {
            ConsecutiveEvent res = null;
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_selectCommandTriggeredEvent);
                sqlCommand.Parameters[PARAM_REASON_ID].Value = reasonId;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = sourceId;
                var sqlDataReader =
                       sqlCommand.ExecuteReader(CommandBehavior.SingleRow);

                if (!sqlDataReader.Read())
                    return res;

                    res=new ConsecutiveEvent(
                        sqlDataReader.GetInt32(0),
                        sqlDataReader.GetInt64(1),
                        sqlDataReader.GetGuid(2),
                        sqlDataReader.GetGuid(3));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
            return res;
        }

        public Dictionary<Guid, DateTime> GetLastExportedEventForCardSourceId(Guid sourceId /*card id*/)
        {
            Dictionary<Guid, DateTime> _dic= new Dictionary<Guid, DateTime>();
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_selectCommandExcel);
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = sourceId;

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        var _event = new ConsecutiveEvent(
                            sqlDataReader.GetInt32(0),
                            sqlDataReader.GetGuid(2),
                            sqlDataReader.GetGuid(3),
                            sqlDataReader.GetDateTime(4));
                        if (!_dic.ContainsKey(_event.ReasonId))
                        {
                            _dic[_event.ReasonId] = _event.EventDateTime.HasValue ? _event.EventDateTime.Value : DateTime.Today.AddDays(-30);
                        }
                    }
                }


            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return _dic;
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
            return _dic;
        }

        void UpdateConsecutiveEventInternal(long eventlogId, ConsecutiveEvent consecutiveEvent)
        {
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_updateCommandTriggeredEvent);
                sqlCommand.Parameters[PARAM_EVENT_ID].Value = eventlogId;
                sqlCommand.Parameters[PARAM_REASON_ID].Value = consecutiveEvent.ReasonId;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = consecutiveEvent.SourceId;
 
                var res= sqlCommand.ExecuteNonQuery();
            }
            catch(Exception error)
            {
               HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        private void InsertConsecutiveEventInternal(long idEventlog, ConsecutiveEvent consecutiveEvent)
        {
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_insertCommandTriggeredEvent);
                sqlCommand.Parameters[PARAM_EVENT_ID].Value = idEventlog;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = consecutiveEvent.SourceId;
                sqlCommand.Parameters[PARAM_REASON_ID].Value = consecutiveEvent.ReasonId;
                var res = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public IEnumerable<ConsecutiveEvent> FilterEventsByCard(DateTime dtEventlogFrom, Guid sourceId)
        {
            var filterSettings = new List<FilterSettings>
            {
                new FilterSettings(
                    Eventlog.COLUMN_EVENTLOG_DATE_TIME,
                    dtEventlogFrom,
                    ComparerModes.EQUALLMORE),


              new FilterSettings(
                Eventlog.COLUMN_EVENTSOURCES,
                                new List<Guid>{sourceId}, //card guid 
                                ComparerModes.IN)
            };

            var items = Eventlogs.Singleton.SelectByCriteria(filterSettings);
            foreach (var item in items)
            {
                var ce = GetConsecutiveEventCR_Card(item);

                if (ce != null && ce.SourceId == sourceId)
                {
                    yield return ce;
                }
            }
        }

        public bool InMinutes(DateTime dt1, DateTime dt2, int minutes)
        {
            var result = Math.Abs(dt2.Subtract(dt1).TotalMinutes);
            return result <= minutes;
        }

        public void  InsertOrUpdateEventExport(ConsecutiveEvent e)
        {
            var dic=GetLastExportedEventForCardSourceId(e.SourceId);
            if(!dic.ContainsKey(e.ReasonId))
            {
                InsertEventExport(e.SourceId, e.ReasonId, e.EventDateTime.Value);
            }
            else
            {
                UpdateEventExport(e.SourceId, e.ReasonId, e.EventDateTime.Value);
            }
        }
        private void UpdateEventExport(Guid sourceId, Guid reasonId, DateTime lasteportEventDT)
        {
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_updateCommandExcel);
                sqlCommand.Parameters[PARAM_LASTEVENT_DT].Value = lasteportEventDT;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = sourceId;
                sqlCommand.Parameters[PARAM_REASON_ID].Value = reasonId;

                var res = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        private  void InsertEventExport(Guid sourceId, Guid reasonId, DateTime lasteportEventDT )
        {
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                var sqlCommand = dbConnectionHolder.GetCommand(_insertCommandExcel);
                sqlCommand.Parameters[PARAM_LASTEVENT_DT].Value = lasteportEventDT;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = sourceId;
                sqlCommand.Parameters[PARAM_REASON_ID].Value = reasonId;
                var res = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public void CleanConsecutiveEvents(Guid idResonSourceIds)
        {
            var dbConnectionHolder = DbConnectionManager.Get();
            try
            {
                 var sqlCommand = dbConnectionHolder.GetCommand(_deleteCommand);
                sqlCommand.Parameters[PARAM_REASON_ID].Value = idResonSourceIds;
                sqlCommand.Parameters[PARAM_SOURCE_ID].Value = idResonSourceIds;
                var res = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                DbConnectionManager.Return(dbConnectionHolder);
            }
        }
        private class UpdateDbCommandForTriggeredEvent : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return $"UPDATE {TABLE_NAME}  SET {ConsecutiveEvent.COLUMN_LASTEVENT_ID} = {PARAM_EVENT_ID}" +
                           $" WHERE {ConsecutiveEvent.COLUMN_REASON_ID} = {PARAM_REASON_ID} AND {ConsecutiveEvent.COLUMN_SOURCE_ID} = {PARAM_SOURCE_ID}"+
                           $" AND {ConsecutiveEvent.COLUMN_LASTEVENTDATETIME} IS NULL";

                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                        new SqlParameter(
                            PARAM_EVENT_ID,
                            SqlDbType.BigInt);
                    yield return
                     new SqlParameter(
                          PARAM_REASON_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                }
            }
        }

        private class UpdateDbCommandForExcelExport : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return $"UPDATE {TABLE_NAME}  SET {ConsecutiveEvent.COLUMN_LASTEVENTDATETIME} = {PARAM_LASTEVENT_DT}" +
                           $" WHERE {ConsecutiveEvent.COLUMN_REASON_ID} = {PARAM_REASON_ID} AND {ConsecutiveEvent.COLUMN_SOURCE_ID} = {PARAM_SOURCE_ID}" +
                           $" AND {ConsecutiveEvent.COLUMN_LASTEVENTDATETIME} IS NOT NULL";

                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                     new SqlParameter(
                          PARAM_REASON_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_LASTEVENT_DT,
                          SqlDbType.DateTime2);
                }
            }
        }

        private class SelectDbCommandForTriggeredEvent : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return
                        $"SELECT * FROM  {TABLE_NAME} "+
                         $" WHERE {ConsecutiveEvent.COLUMN_REASON_ID} = {PARAM_REASON_ID} AND {ConsecutiveEvent.COLUMN_SOURCE_ID} = {PARAM_SOURCE_ID}"+
                         $" AND {ConsecutiveEvent.COLUMN_LASTEVENTDATETIME} IS NULL";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                    new SqlParameter(
                         PARAM_REASON_ID,
                         SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                }
            }
        }

        private class SelectDbCommandForExcelExport : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return
                        $"SELECT * FROM  {TABLE_NAME} " +
                         $" WHERE {ConsecutiveEvent.COLUMN_SOURCE_ID} = {PARAM_SOURCE_ID}" +
                         $" AND {ConsecutiveEvent.COLUMN_LASTEVENTDATETIME} IS NOT NULL";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                }
            }
        }

        private class InsertDbCommandForTriggeredEvent : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return $"INSERT INTO {TABLE_NAME}  VALUES ({PARAM_EVENT_ID}, {PARAM_SOURCE_ID},{PARAM_REASON_ID}, NULL);";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                        new SqlParameter(
                            PARAM_EVENT_ID,
                            SqlDbType.BigInt);
                    yield return
                     new SqlParameter(
                          PARAM_REASON_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                }
            }
        }

        private class InsertDbCommandForExcelExport : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return $"INSERT INTO {TABLE_NAME}  VALUES (NULL, {PARAM_SOURCE_ID},{PARAM_REASON_ID}, {PARAM_LASTEVENT_DT});";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                     new SqlParameter(
                          PARAM_REASON_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_LASTEVENT_DT,
                          SqlDbType.DateTime2);
                }
            }
        }

        private class DeleteOldDbCommand : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return
                        $"DELETE  FROM  {TABLE_NAME} WHERE " +
                         $"{ConsecutiveEvent.COLUMN_REASON_ID}  = {PARAM_REASON_ID}" +
                         $" OR {ConsecutiveEvent.COLUMN_SOURCE_ID}  = {PARAM_SOURCE_ID}";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                     new SqlParameter(
                          PARAM_REASON_ID,
                          SqlDbType.UniqueIdentifier);
                    yield return
                     new SqlParameter(
                          PARAM_SOURCE_ID,
                          SqlDbType.UniqueIdentifier);
                }
            }
        }
    }
}
