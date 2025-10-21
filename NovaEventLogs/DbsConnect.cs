using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace NovaEventLogs
{
    public class DbsConnect
    {
        public virtual string DataSource { get; set; }
        public virtual string Server { get; set; }
        public virtual string User { get; set; }
        public virtual string Password { get; set; }
        public virtual string EventLogDbs { get; set; }

        SqlConnection _sqlConnection;
        SqlConnectionStringBuilder _sqlCS;

        public SqlConnection SqlConnection { get { return _sqlConnection; } }

        private void CreateConnectionString()
        {
            _sqlCS = new SqlConnectionStringBuilder();
            _sqlCS.PersistSecurityInfo = true;
            _sqlCS.DataSource = Server;
            _sqlCS.InitialCatalog = EventLogDbs;
            _sqlCS.UserID = User;
            _sqlCS.Password = Password;
            _sqlCS.IntegratedSecurity = false;
        }

        public bool SameDatabases()
        {
            return (DataSource == EventLogDbs);
        }

        public void CreateSqlConnection()
        {
            CreateConnectionString();
            _sqlConnection = new SqlConnection(_sqlCS.ConnectionString);
        }

        public bool ValidCS()
        {
            if (string.IsNullOrEmpty(DataSource) || string.IsNullOrEmpty(Server) ||
                string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(EventLogDbs))
                return false;
            else
                return true;
        }

    }
}
