using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.Server.DB
{
    public class EventlogParameters
    {
        public const string EVENTLOG_TABLE_NAME = "EventlogParameter";

        public class InsertCommandFactoryType : DbCommandFactory
        {
            public const string PARAM_TYPE =
                "@" + EventlogParameter.COLUMN_TYPE;

            public const string PARAM_TYPE_GUID =
                "@" + EventlogParameter.COLUMN_TYPE_GUID;

            public const string PARAM_TYPE_OBJECT_TYPE =
                "@" + EventlogParameter.COLUMN_TYPE_OBJECT_TYPE;

            public const string PARAM_VALUE =
                "@" + EventlogParameter.COLUMN_VALUE;

            public const string PARAM_ID_EVENTLOG =
                "@" + EventlogParameter.COLUMN_ID_EVENTLOG;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "insert into {0} ({1}, {2}, {3}, {4}, {5}) values ({6}, {7}, {8}, {9}, {10}); select cast(scope_identity() as bigint)",
                            EVENTLOG_TABLE_NAME,
                            EventlogParameter.COLUMN_TYPE,
                            EventlogParameter.COLUMN_TYPE_GUID,
                            EventlogParameter.COLUMN_TYPE_OBJECT_TYPE,
                            EventlogParameter.COLUMN_VALUE,
                            EventlogParameter.COLUMN_ID_EVENTLOG,
                            PARAM_TYPE,
                            PARAM_TYPE_GUID,
                            PARAM_TYPE_OBJECT_TYPE,
                            PARAM_VALUE,
                            PARAM_ID_EVENTLOG);
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
                            PARAM_TYPE_GUID,
                            SqlDbType.UniqueIdentifier);

                    yield return
                        new SqlParameter(
                            PARAM_TYPE_OBJECT_TYPE,
                            SqlDbType.TinyInt);

                    yield return
                        new SqlParameter(
                            PARAM_VALUE,
                            SqlDbType.NVarChar);

                    yield return
                        new SqlParameter(
                            PARAM_ID_EVENTLOG,
                            SqlDbType.BigInt);
                }
            }
        }

        private class GetByEventlogIdCommandFactory : DbCommandFactory
        {
            public const string PARAM_EVENTLOG_ID = 
                "@" + EventlogParameter.COLUMN_ID_EVENTLOG;

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "select {2}, {3}, {4}, {5}, {6} from {0} where {1} = {7}",
                            EVENTLOG_TABLE_NAME,
                            EventlogParameter.COLUMN_ID_EVENTLOG,
                            EventlogParameter.COLUMN_ID_EVENTLOG_PARAMETER,
                            EventlogParameter.COLUMN_TYPE,
                            EventlogParameter.COLUMN_TYPE_GUID,
                            EventlogParameter.COLUMN_TYPE_OBJECT_TYPE,
                            EventlogParameter.COLUMN_VALUE,
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

        private static volatile EventlogParameters _singleton;
        private static readonly object SyncRoot = new object();

        public InsertCommandFactoryType InsertCommandFactory { get; private set; }
        private readonly GetByEventlogIdCommandFactory _getByEventlogIdCommandFactory;

        public EventlogParameters()
        {
            InsertCommandFactory = 
                new InsertCommandFactoryType();

            _getByEventlogIdCommandFactory = 
                new GetByEventlogIdCommandFactory();
        }

        public static EventlogParameters Singleton
        {
            get
            {
                if (_singleton != null)
                    return _singleton;

                lock (SyncRoot)
                    if (_singleton == null)
                        _singleton = new EventlogParameters();

                return _singleton;
            }
        }

        public void InsertInternal(
            EventlogParameter eventlogParameter, 
            SqlCommand insertCommand)
        {
            insertCommand.Parameters[InsertCommandFactoryType.PARAM_TYPE].Value =
                eventlogParameter.Type;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_TYPE_GUID].Value =
                eventlogParameter.TypeGuid;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_TYPE_OBJECT_TYPE].Value =
                eventlogParameter.TypeObjectType;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_VALUE].Value =
                eventlogParameter.Value;

            insertCommand.Parameters[InsertCommandFactoryType.PARAM_ID_EVENTLOG].Value =
                eventlogParameter.Eventlog.IdEventlog;

            eventlogParameter.IdEventlogParameter = 
                (long)insertCommand.ExecuteScalar();
        }

        public void GetParametersForEventlog(
            Eventlog eventlog, 
            DbConnectionHolder dbConnectionHolder)
        {
            if (eventlog.EventlogParameters == null)
                eventlog.EventlogParameters = new List<EventlogParameter>();
            else
                eventlog.EventlogParameters.Clear();

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
                    eventlog.EventlogParameters.Add(
                        new EventlogParameter(
                            reader.GetString(1),
                            reader.GetString(4),
                            reader.GetGuid(2),
                            (ObjectType)reader.GetByte(3))
                        {
                            Eventlog = eventlog,
                            IdEventlogParameter = reader.GetInt64(0)
                        });
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
