using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using Contal.IwQuick;

namespace Contal.Cgp.Server.DB
{
    public class DbConnectionHolder : ADisposable
    {
        private readonly SqlConnection _sqlConnection;

        private readonly IDictionary<DbCommandFactory, SqlCommand> _sqlCommands =
            new Dictionary<DbCommandFactory, SqlCommand>();


        public DbConnectionHolder(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
        }

        public SqlConnection SqlConnection
        {
            get { return _sqlConnection; } 
        }

        public bool EnsureConnectionOpen()
        {
            ConnectionState connectionState =
                ConnectionState.Closed;

            try
            {
                lock (_sqlConnection)
                {
                    connectionState = _sqlConnection.State;

                    switch (_sqlConnection.State)
                    {
                        case ConnectionState.Open:

                            return true;

                        case ConnectionState.Closed:

                            _sqlConnection.Open();
                            break;

                        case ConnectionState.Broken:

                            _sqlConnection.Close();
                            _sqlConnection.Open();
                            break;
                    }

                    ClearPreparedCommands();
                }

                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    CgpServer.Singleton.LogCgpServer.Error(
                        string.Format(
                            "DbConnectionHolder.EnsureConnectionOpen() failed : " + Environment.NewLine +
                            "_sqlConnection.State = {0}" + Environment.NewLine +
                            "Exception = {1}",
                            connectionState,
                            ex));
                }
                catch
                {
                    
                }

                return false;
            }
        }

        private void ClearPreparedCommands()
        {
            lock (_sqlCommands)
            {
                foreach (var sqlCommand in _sqlCommands.Values)
                    try
                    {
                        sqlCommand.Dispose();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            CgpServer.Singleton.LogCgpServer.Error(
                                string.Format(
                                    "slqCommand.Dispose() failed : {0}",
                                    ex));
                        }
                        catch
                        {
                            
                        }
                    }

                _sqlCommands.Clear();
            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (!isExplicitDispose)
                return;

            ClearPreparedCommands();

            try
            {
                _sqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    CgpServer.Singleton.LogCgpServer.Error(
                        string.Format(
                            "DbConnectionHolder.InternalDispose() failed : {0}",
                            ex));
                }
                catch
                {
                    
                }
            }
        }

        public SqlCommand GetCommand(DbCommandFactory dbCommandFactory)
        {
            lock (_sqlCommands)
            {
                SqlCommand result;

                if (_sqlCommands.TryGetValue(dbCommandFactory, out result))
                    return result;

                result = dbCommandFactory.CreateCommand(_sqlConnection);

                //result.Prepare();

                _sqlCommands.Add(
                    dbCommandFactory,
                    result);

                return result;
            }
        }
    }
}
