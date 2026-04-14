using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.Server.DB
{
    public class EventSources
    {
        public const string EVENTSOURCE_TABLE_NAME = "EventSource";

        public class InsertCommandFactoryType : DbCommandFactory
        {
            public const string PARAM_EVENTLOG_ID =
                "@" + EventSource.COLUMN_EVENTLOG_ID;

            public const string PARAM_EVENTSOURCE_OBJECT_GUID =
                "@" + EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "insert into {0} ({1}, {2}) values ({3}, {4})",
                            EVENTSOURCE_TABLE_NAME,
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                            PARAM_EVENTLOG_ID,
                            PARAM_EVENTSOURCE_OBJECT_GUID);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return 
                        new SqlParameter(
                            PARAM_EVENTLOG_ID,
                            SqlDbType.BigInt);

                    yield return 
                        new SqlParameter(
                            PARAM_EVENTSOURCE_OBJECT_GUID,
                            SqlDbType.UniqueIdentifier);
                }
            }
        }

        private class GetByEventlogIdCommandFactory : DbCommandFactory
        {
            public const string PARAM_EVENTLOG_ID = 
                "@" +EventSource.COLUMN_EVENTLOG_ID;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "select {2} from {0} where {1} = {3}",
                            EVENTSOURCE_TABLE_NAME,
                            EventSource.COLUMN_EVENTLOG_ID,
                            EventSource.COLUMN_EVENTSOURCE_OBJECT_GUID,
                            PARAM_EVENTLOG_ID);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return 
                        new SqlParameter(
                            PARAM_EVENTLOG_ID,
                            SqlDbType.BigInt);
                }
            }
        }

        private static readonly object SyncRoot = new object();

        private static volatile EventSources _singleton;

        public InsertCommandFactoryType InsertCommandFactory { get; private set; }
        private readonly GetByEventlogIdCommandFactory _getByEventlogIdCommandFactory;

        public EventSources()
        {
            InsertCommandFactory = new InsertCommandFactoryType();
            _getByEventlogIdCommandFactory = new GetByEventlogIdCommandFactory();
        }

        public static EventSources Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;

                lock (SyncRoot)
                    if (_singleton == null)
                        _singleton = new EventSources();

                return _singleton;
            }
        }

        public void InsertInternal(EventSource eventSource, SqlCommand insertCommand)
        {
            insertCommand.Parameters[InsertCommandFactoryType.PARAM_EVENTLOG_ID].Value =
                eventSource.Eventlog.IdEventlog;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_EVENTSOURCE_OBJECT_GUID].Value =
                eventSource.EventSourceObjectGuid;

            insertCommand.ExecuteNonQuery();
        }

        public void GetSourcesForEventlog(
            Eventlog eventlog,
            DbConnectionHolder dbConnectionHolder)
        {
            if (eventlog.EventSources == null)
                eventlog.EventSources = new List<EventSource>();
            else
                eventlog.EventSources.Clear();

            SqlCommand getByEventlogIdCommand =
                dbConnectionHolder.GetCommand(
                    _getByEventlogIdCommandFactory);

            getByEventlogIdCommand.Parameters[GetByEventlogIdCommandFactory.PARAM_EVENTLOG_ID].Value =
                eventlog.IdEventlog;

            SqlDataReader reader = null;

            try
            {
                reader = getByEventlogIdCommand.ExecuteReader();

                while (reader.Read())
                    eventlog.EventSources.Add(
                        new EventSource(
                            reader.GetGuid(0),
                            eventlog));
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    try
                    {
                        reader.Close();
                    }
                    catch
                    {
                    }
            }
        }
    }
}
