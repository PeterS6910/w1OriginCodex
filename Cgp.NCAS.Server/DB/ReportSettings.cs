using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Sys;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class ReportSettings
        : ANcasBaseOrmTable<ReportSettings, ReportSetting>,
            IReportSettings
    {
        private class UpdateDbCommand : DbCommandFactory
        {
            public const string PARAM_LAST_ID =
                "@LastId";

            protected override string Text
            {
                get
                {
                    return
                        string.Format(
                            "UPDATE ReportData SET LastId = {0} WHERE id = 0",
                            PARAM_LAST_ID);
                }
            }

            protected override IEnumerable<SqlParameter> Parameters
            {
                get
                {
                    yield return
                      new SqlParameter(
                          PARAM_LAST_ID,
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
                        "SELECT LastId FROM ReportData WHERE id = 0";
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

        private const int ID_REPORT_SETTING = 0;

        private ReportSetting _ReportSetting;
        private readonly DbConnectionManager _dbConnectionManager;
        private readonly SelectDbCommand _selectDbCommand;
        private readonly UpdateDbCommand _updateDbCommand;

        private ReportSettings(ReportSettings singleton) 
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
                    "SELECT COUNT(*) FROM ReportData",
                    dbConnectionHolder.SqlConnection);

                if ((int) sqlCommand.ExecuteScalar() > 0)
                    return;

                sqlCommand.CommandText = "INSERT INTO ReportData VALUES(0, -1);";
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

        public ReportSetting GetReportSetting()
        {
            var ReportSetting = GetById(ID_REPORT_SETTING);
            //CgpServer.Singleton.ReportCommunicationIsEnabled = ReportSetting.IsEnabled;

            return ReportSetting;
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
            get { return ObjectType.ReportSetting; }
        }

        #region IReportSettings Members

        public ReportSetting LoadReportSetting(out bool allowEdit)
        {
            Exception error;

            _ReportSetting = GetObjectForEdit(ID_REPORT_SETTING, out error);

            if (_ReportSetting != null)
            {
                allowEdit = true;
                return _ReportSetting;
            }

            allowEdit = false;
            return GetById(ID_REPORT_SETTING);
        }

        public bool SaveReportSetting(ReportSetting ReportSetting, out Exception error)
        {
            return Update(ReportSetting, out error);
        }

        public void EditEnd()
        {
            if (_ReportSetting != null)
                EditEnd(_ReportSetting);
        }

        public void ReloadObjectForEdit(out Exception error)
        {
            RenewObjectForEdit(ID_REPORT_SETTING, out error);
        }

        #endregion

        protected override void LoadObjectsInRelationship(ReportSetting obj)
        {
           /* if (obj.CardReaders != null)
            {
                var crIds = new LinkedList<Guid>(obj.CardReaders.Select(cr => cr.IdCardReader));
                obj.CardReaders.Clear();

                foreach (var crId in crIds)
                {
                    obj.CardReaders.Add(CardReaders.Singleton.GetById(crId));
                }
            }*/
        }

        public override void AfterUpdate(ReportSetting newObject, ReportSetting oldObjectBeforUpdate)
        {
          /*  if (newObject.IsEnabled != oldObjectBeforUpdate.IsEnabled
                || newObject.IpAddress != oldObjectBeforUpdate.IpAddress
                || newObject.Port != oldObjectBeforUpdate.Port
                || newObject.LoginName != oldObjectBeforUpdate.LoginName
                || newObject.LoginPassword != oldObjectBeforUpdate.LoginPassword)
            {
                ReportCore.Singleton.AfterReportCommunicationSettingsUpdate(newObject);
                CgpServer.Singleton.ReportCommunicationIsEnabled = newObject.IsEnabled;
            }

            ReportCore.Singleton.AfterReportCardReaderSettingsUpdate(newObject.CardReaders);*/
        }

        public Int64 LastId
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
                    sqlCommand.Parameters[UpdateDbCommand.PARAM_LAST_ID].Value = value;
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
            return true; // ReportCore.Singleton.IsConnected();
        }

        public void InsertReportErrorEvent(Int64 idErrorEventlog)
        {
            var dbConnectionHolder = _dbConnectionManager.Get();

            try
            {
                var sqlCommand = new SqlCommand(
                    String.Format(
                        "INSERT INTO ReportErrorEvents (ErrorEventId) VALUES({0})",
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

        public void RemoveReportErrorEvent(ICollection<int> ids)
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
                        "DELETE FROM ReportErrorEvents WHERE id IN {0}",
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

        public void RemoveReportErrorEvent(int id)
        {
            RemoveReportErrorEvent(new [] {id});
        }

        public ICollection<ReportErrorEvent> GeReportErrorEvents()
        {
            var result = new LinkedList<ReportErrorEvent>();         
            var dbConnectionHolder = _dbConnectionManager.Get();
            SqlDataReader sqlReader = null;

            try
            {
                var sqlCommand = new SqlCommand(
                    "SELECT * FROM ReportErrorEvents",
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

                    result.AddLast(new ReportErrorEvent(
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

        public Dictionary<int, TransitionAddResult> TryResendReportErrorEvents(
            ICollection<ReportErrorEvent> ReportErrorEvents)
        {
            return null; // ReportCore.Singleton.TryResendEvents(ReportErrorEvents);
        }
    }
}
