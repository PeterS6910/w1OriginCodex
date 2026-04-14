using System.Data.SqlClient;

using Contal.Cgp.ORM;
using Contal.IwQuick;

namespace Contal.Cgp.Server.DB
{
    public class DbConnectionManager : AObjectPool<DbConnectionHolder>
    {
        private readonly string _connectionString;
        private readonly SqlConnection _probingConnection;

        public DbConnectionManager(ConnectionString connectionString)
        {
            _connectionString = connectionString.ToString();

            _probingConnection =
                new SqlConnection(
                    connectionString.ToString(10, true));
        }


        public new DbConnectionHolder Get()
        {
            while (CheckConnection())
            {
                bool newlyAdded;

                var result = base.Get(out newlyAdded);

                if (result.EnsureConnectionOpen())
                    return result;

                result.Dispose();

                if (newlyAdded)
                    return null;
            }

            Clear();
            return null;
        }

        private bool CheckConnection()
        {
            lock (_probingConnection)
            {
                try
                {
                    _probingConnection.Open();

                    var checkConnectionCommand =
                        new SqlCommand(
                            "select 1",
                            _probingConnection)
                        {
                            CommandTimeout = 10
                        };

                    checkConnectionCommand.ExecuteNonQuery();

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    try
                    {
                        _probingConnection.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        protected override DbConnectionHolder CreateObject()
        {
            var dbConnectionHolder =
                new DbConnectionHolder(_connectionString);

            return dbConnectionHolder;
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            base.InternalDispose(isExplicitDispose);

            if (isExplicitDispose)
                _probingConnection.Dispose();
        }
    }
}
