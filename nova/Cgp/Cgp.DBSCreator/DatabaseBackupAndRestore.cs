using System;
using System.Data.SqlClient;

namespace Contal.Cgp.DBSCreator
{
    public static class DatabaseBackupAndRestore
    {
        /// <summary>
        /// Return path for the database backup.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="databaseName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private static string GetDatabaseBackupPath(
            string path, 
            string databaseName, 
            string connectionString)
        {
            string dbsBackupPath = 
                !string.IsNullOrEmpty(path) &&
                SqlServerRunOnSameComputer(connectionString)
                    ? path + @"\"
                    : string.Empty;

            return dbsBackupPath + databaseName + @".Bak";
        }

        /// <summary>
        /// Return true if sql server is running on the same computer as Nova server
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool SqlServerRunOnSameComputer(string connectionString)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = connectionString
                    };

                sqlConnection.Open();

                var myCommand = 
                    new SqlCommand(
                        "SELECT SERVERPROPERTY('MachineName') as MachineName", 
                        sqlConnection);

                SqlDataReader reader = myCommand.ExecuteReader();

                string sqlServerMachineName = string.Empty;

                if (reader.HasRows)
                    if (reader.Read())
                        sqlServerMachineName =
                            reader.GetString(reader.GetOrdinal("MachineName"));

                return sqlServerMachineName == Environment.MachineName;
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    if (sqlConnection != null)
                        sqlConnection.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Backup the database. Return true if backup the database succeeded otherwise false.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="databaseName"></param>
        /// <param name="connectionString"></param>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        public static bool DatabaseBackup(
            string path, 
            string databaseName, 
            string connectionString, 
            out string errorStr)
        {
            errorStr = string.Empty;

            string where = 
                GetDatabaseBackupPath(
                    path, 
                    databaseName, 
                    connectionString);

            string name = databaseName + " database backup";

            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = connectionString
                    };

                sqlConnection.Open();

                var myCommand = 
                    new SqlCommand(
                        @"BACKUP DATABASE [" + databaseName + @"] TO  DISK = N'" + @where + @"' 
                            WITH NOFORMAT, INIT,  NAME = N'" + name + @"', SKIP, NOREWIND, NOUNLOAD", 
                        sqlConnection)
                    {
                        CommandTimeout = 700
                    };

                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                errorStr = ex.Message;
                return false;
            }
            finally
            {
                try
                {
                    if (sqlConnection != null)
                        sqlConnection.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Check the backup file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="databaseName"></param>
        /// <param name="connectionString"></param>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        public static bool CheckSuccessDatabaseBackup(
            string path, 
            string databaseName, 
            string connectionString, 
            out string errorStr)
        {
            errorStr = string.Empty;

            SqlConnection sqlConnection = null;
            try
            {
                string where = 
                    GetDatabaseBackupPath(
                        path, 
                        databaseName, 
                        connectionString);

                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = connectionString
                    };

                sqlConnection.Open();

                //string sqlQuery = @"RESTORE VERIFYONLY FROM DISK = N'" + where + @"' WITH FILE = 1, NOUNLOAD, NOREWIND";
                string sqlQuery = @"declare @backupSetId as int
                    select @backupSetId = position from msdb..backupset where database_name=N'" + databaseName + @"'
                    and backup_set_id=(select max(backup_set_id) from msdb..backupset where database_name=N'" + databaseName + @"' )
                    if @backupSetId is null begin raiserror(N'error verify database ''" + databaseName + @"''.', 16, 1) end
                    RESTORE VERIFYONLY FROM DISK = N'" + where + @"' WITH FILE = @backupSetId, NOUNLOAD, NOREWIND";

                var myCommand = 
                    new SqlCommand(sqlQuery, sqlConnection)
                    {
                        CommandTimeout = 700
                    };

                myCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                errorStr = ex.Message;
                return false;
            }
            finally
            {
                try
                {
                    if (sqlConnection != null)
                        sqlConnection.Close();
                }
                catch
                {
                }
            }
        }


        /// <summary>
        /// Restore the database. Return true if restore the database succeeded otherwise false.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="databaseName"></param>
        /// <param name="connectionString"></param>
        /// <param name="errorStr"></param>
        /// <returns></returns>
        public static bool DatabaseRestore(
            string path, 
            string databaseName, 
            string connectionString, 
            out string errorStr)
        {
            errorStr = string.Empty;

            SqlConnection sqlConnection = null;
            try
            {
                string where = GetDatabaseBackupPath(path, databaseName, connectionString);

                sqlConnection = 
                    new SqlConnection
                    {
                        ConnectionString = connectionString
                    };

                sqlConnection.Open();

                string sqlQuery = @"RESTORE DATABASE [" + databaseName + @"] FROM DISK = N'" + where + @"' WITH NOREWIND, NOUNLOAD";

                var myCommand = 
                    new SqlCommand(sqlQuery, sqlConnection)
                    {
                        CommandTimeout = 700
                    };

                myCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                errorStr = ex.Message;
                return false;
            }
            finally
            {
                try
                {
                    if (sqlConnection != null)
                        sqlConnection.Close();
                }
                catch
                {
                }
            }
        }
    }
}
