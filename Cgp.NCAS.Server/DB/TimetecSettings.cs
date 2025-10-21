using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Timetec;
using Contal.Cgp.ORM;
using Contal.Cgp.Server;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Sys;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class TimetecSettings
        : ANcasBaseOrmTable<TimetecSettings, TimetecSetting>,
            ITimetecSettings
    {
        private class UpdateDbCommand : DbCommandFactory
        {
            public const string PARAM_LAST_EVENT_ID =
                "@LastEventId";

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "UPDATE TimetecData SET LastEventId = {0} WHERE id = 0",
                            PARAM_LAST_EVENT_ID);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                      new SqlParameter(
                          PARAM_LAST_EVENT_ID,
                          SqlDbType.BigInt);
                }
            }
        }

        private class SelectDbCommand : DbCommandFactory
        {
            protected override string Text
            {
                get
                {
                    return
                        "SELECT LastEventId FROM TimetecData WHERE id = 0";
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield break;
                }
            }
        }

        private const int ID_TIMETEC_SETTING = 0;

        private TimetecSetting _timetecSetting;
        private readonly DbConnectionManager _dbConnectionManager;
        private readonly SelectDbCommand _selectDbCommand;
        private readonly UpdateDbCommand _updateDbCommand;

        private TimetecSettings(TimetecSettings singleton) 
            : base(singleton)
        {
            var connectionString =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING_EXTERN_DATABASE);

            if (connectionString == null || !connectionString.IsValid())
                connectionString =
                    ConnectionString.LoadFromRegistry(
                        CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            if (connectionString != null &&
                connectionString.IsValid())
            {
                _dbConnectionManager =
                    new DbConnectionManager(connectionString);

                _selectDbCommand = new SelectDbCommand();
                _updateDbCommand = new UpdateDbCommand();
            }
        }

        public void CreateIfNotExists()
        {
            var dbConnectionHolder = _dbConnectionManager.Get();

            try
            {
                var sqlCommand = new SqlCommand(
                    "SELECT COUNT(*) FROM TimetecData",
                    dbConnectionHolder.SqlConnection);

                if ((int) sqlCommand.ExecuteScalar() > 0)
                    return;

                sqlCommand.CommandText = "INSERT INTO TimetecData VALUES(0, -1);";
                sqlCommand.ExecuteNonQuery();
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _dbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public TimetecSetting GetTimetecSetting()
        {
            var timetecSetting = GetById(ID_TIMETEC_SETTING);
            CgpServer.Singleton.TimetecCommunicationIsEnabled = timetecSetting == null ? false : timetecSetting.IsEnabled;

            return timetecSetting;
        }

        public override bool HasAccessView(Cgp.Server.Beans.Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessInsert(Cgp.Server.Beans.Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessUpdate(Cgp.Server.Beans.Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override bool HasAccessDelete(Cgp.Server.Beans.Login login)
        {
            return Logins.Singleton.IsLoginSuperAdmin(login);
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.TimetecSetting; }
        }

        #region ITimetecSettings Members

        public TimetecSetting LoadTimetecSetting(out bool allowEdit)
        {
            Exception error;

            _timetecSetting = GetObjectForEdit(ID_TIMETEC_SETTING, out error);

            if (_timetecSetting != null)
            {
                allowEdit = true;
                return _timetecSetting;
            }

            allowEdit = false;
            return GetById(ID_TIMETEC_SETTING);
        }

        public bool SaveTimetecSetting(TimetecSetting timetecSetting, out Exception error)
        {
            return Update(timetecSetting, out error);
        }

        public void EditEnd()
        {
            if (_timetecSetting != null)
                EditEnd(_timetecSetting);
        }

        public void ReloadObjectForEdit(out Exception error)
        {
            RenewObjectForEdit(ID_TIMETEC_SETTING, out error);
        }

        #endregion

        protected override void LoadObjectsInRelationship(TimetecSetting obj)
        {
            if (obj.CardReaders != null)
            {
                var crIds = new LinkedList<Guid>(obj.CardReaders.Select(cr => cr.IdCardReader));
                obj.CardReaders.Clear();

                foreach (var crId in crIds)
                {
                    obj.CardReaders.Add(CardReaders.Singleton.GetById(crId));
                }
            }
        }

        public override void AfterUpdate(TimetecSetting newObject, TimetecSetting oldObjectBeforUpdate)
        {
            if (newObject.IsEnabled != oldObjectBeforUpdate.IsEnabled
                || newObject.IpAddress != oldObjectBeforUpdate.IpAddress
                || newObject.Port != oldObjectBeforUpdate.Port
                || newObject.LoginName != oldObjectBeforUpdate.LoginName
                || newObject.LoginPassword != oldObjectBeforUpdate.LoginPassword
                || newObject.DoNotImportDepartments != oldObjectBeforUpdate.DoNotImportDepartments)
            {
                TimetecCore.Singleton.AfterTimetecCommunicationSettingsUpdate(newObject);
                CgpServer.Singleton.TimetecCommunicationIsEnabled = newObject.IsEnabled;
            }

            TimetecCore.Singleton.AfterTimetecCardReaderSettingsUpdate(newObject.CardReaders);
        }

        public Int64 LastEventId
        {
            get
            {
                var dbConnectionHolder = _dbConnectionManager.Get();

                try
                {
                    var sqlCommand = dbConnectionHolder.GetCommand(_selectDbCommand);

                    return (Int64) sqlCommand.ExecuteScalar();
                }
                finally
                {
                    _dbConnectionManager.Return(dbConnectionHolder);
                }
            }
            set
            {
                var dbConnectionHolder = _dbConnectionManager.Get();

                try
                {
                    var sqlCommand = dbConnectionHolder.GetCommand(_updateDbCommand);
                    sqlCommand.Parameters[UpdateDbCommand.PARAM_LAST_EVENT_ID].Value = value;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
                finally
                {
                    _dbConnectionManager.Return(dbConnectionHolder);
                }
            }
        }

        public bool? IsConnected()
        {
            return TimetecCore.Singleton.IsConnected();
        }

        public void InsertTimetecErrorEvent(Int64 idErrorEventlog)
        {
            var dbConnectionHolder = _dbConnectionManager.Get();

            try
            {
                var sqlCommand = new SqlCommand(
                    String.Format(
                        "INSERT INTO TimetecErrorEvents (ErrorEventId) VALUES({0})",
                        idErrorEventlog),
                    dbConnectionHolder.SqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _dbConnectionManager.Return(dbConnectionHolder);
            }
        }

        public void RemoveTimetecErrorEvent(ICollection<int> ids)
        {
            var idsString = new StringBuilder();

            foreach (int id in ids)
            {
                idsString.AppendFormat(string.IsNullOrEmpty(idsString.ToString())
                    ? "({0}"
                    : ",{0}", id);
            }

            idsString.Append(")");

            var dbConnectionHolder = _dbConnectionManager.Get();

            try
            {
                var sqlCommand = new SqlCommand(
                    String.Format(
                        "DELETE FROM TimetecErrorEvents WHERE id IN {0}",
                        idsString),
                    dbConnectionHolder.SqlConnection);

                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _dbConnectionManager.Return(dbConnectionHolder);
            }
        } 

        public void RemoveTimetecErrorEvent(int id)
        {
            RemoveTimetecErrorEvent(new [] {id});
        }

        public ICollection<TimetecErrorEvent> GeTimetecErrorEvents()
        {
            var result = new LinkedList<TimetecErrorEvent>();         
            var dbConnectionHolder = _dbConnectionManager.Get();
            SqlDataReader sqlReader = null;

            try
            {
                var sqlCommand = new SqlCommand(
                    "SELECT * FROM TimetecErrorEvents",
                    dbConnectionHolder.SqlConnection);

                sqlReader = sqlCommand.ExecuteReader();

                while (sqlReader.Read())
                {
                    var idErrorEventlog = (Int64) sqlReader[1];
                    var errorEventlog = Eventlogs.Singleton.GetObjectById(idErrorEventlog);

                    if (errorEventlog == null)
                        continue;

                    var sourceEventlogIdParameter =  errorEventlog.EventlogParameters.FirstOrDefault(param => param.Type == "Eventlog id");

                    if (sourceEventlogIdParameter == null)
                        continue;

                    var sourceEventlog = Eventlogs.Singleton.GetObjectById(Int64.Parse(sourceEventlogIdParameter.Value));

                    if (sourceEventlog == null)
                        continue;

                    result.AddLast(new TimetecErrorEvent(
                        (int) sqlReader[0],
                        errorEventlog,
                        sourceEventlog));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (sqlReader != null)
                    try
                    {
                        sqlReader.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                    
                _dbConnectionManager.Return(dbConnectionHolder);
            }

            return result;
        }

        public Dictionary<int, TransitionAddResult> TryResendTimetecErrorEvents(
            ICollection<TimetecErrorEvent> timetecErrorEvents)
        {
            return TimetecCore.Singleton.TryResendEvents(timetecErrorEvents);
        }
    }
}
