using System.Data.SqlServerCe;
using System.Threading;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public abstract class ASqlCeDbEngine :
        ISqlCeDbCommandFactory,
        ISqlCeConnectionProvider
    {
        private SqlCeConnection _sqlCeConnection;
        private readonly object _lockCreateConnection = new object();
        private readonly AutoResetEvent _databaseOperationEvent = new AutoResetEvent(true);

        SqlCeDbCommand ISqlCeDbCommandFactory.CreateSqlCeDbCommand(string command)
        {
            return new SqlCeDbCommand(
                this,
                command);
        }

        SqlCeDbResultSet ISqlCeDbCommandFactory.CreateSqlCeDbResultSet(string tableName)
        {
            return new SqlCeDbResultSet(
                this,
                tableName);
        }

        SqlCeConnection ISqlCeConnectionProvider.Connection
        {
            get
            {
                if (_sqlCeConnection == null)
                    lock (_lockCreateConnection)
                        if (_sqlCeConnection == null)
                        {
                            _sqlCeConnection = CreateConnection();
                            _sqlCeConnection.Open();
                        }

                return _sqlCeConnection;
            }
        }

        protected abstract SqlCeConnection CreateConnection();

        void ISqlCeConnectionProvider.StartDatabaseOperaion()
        {
            _databaseOperationEvent.WaitOne();
        }

        void ISqlCeConnectionProvider.FinishDatabaseOperation()
        {
            _databaseOperationEvent.Set();
        }
    }
}
