using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Server.DB
{
    public abstract class ADbBatchWorker<T>
        where T : ADbBatchWorker<T>.DbItem
    {
        public class DbItem
        {
            public virtual void OnComplete()
            {
            }
        }

        protected abstract class ADbItemWorker
        {
            private readonly SqlConnection _sqlConnection;
            private readonly IEnumerable<SqlCommand> _sqlCommands;

            protected ADbItemWorker(
                SqlConnection sqlConnection,
                IEnumerable<SqlCommand> sqlCommands)
            {
                _sqlConnection = sqlConnection;
                _sqlCommands = sqlCommands;
            }

            public bool Process(T item)
            {
                SqlTransaction transaction;

                try
                {
                    transaction = _sqlConnection.BeginTransaction();
                }
                catch
                {
                    return false;
                }

                try
                {
                    foreach (var sqlCommand in _sqlCommands)
                        sqlCommand.Transaction = transaction;

                    ProcessInternal(item);

                    transaction.Commit();
                }
                catch (Exception errorInsert)
                {
                    string strExceptionInsert = errorInsert.ToString();
                    if (errorInsert is SqlException)
                    {
                        strExceptionInsert += Environment.NewLine +
                                              string.Format("SqlException number: {0}",
                                                  (errorInsert as SqlException).Number);
                    }

                    CgpServer.Singleton.LogCgpServer.Error(string.Format("Exception while inserting to the Eventlog(Insert, commit): {0}", strExceptionInsert));

                    try
                    {
                        if (transaction.Connection.State != ConnectionState.Open)
                        {
                            CgpServer.Singleton.LogCgpServer.Error("Exception while inserting to the Eventlog: connection is not opened");
                            return false;
                        }

                        transaction.Rollback();
                    }
                    catch(Exception errorRollback)
                    {
                        string strExceptionRollback = errorRollback.ToString();
                        if (errorRollback is SqlException)
                        {
                            strExceptionRollback += Environment.NewLine +
                                                    string.Format("SqlException number: {0}",
                                                        (errorRollback as SqlException).Number);
                        }

                        CgpServer.Singleton.LogCgpServer.Error(string.Format("Exception while inserting to the Eventlog(Rollback): {0}", strExceptionRollback));
                        return false;
                    }
                }
                finally
                {
                    // There's no need to call transaction.Dispose, 
                    // as all it does is that it calls transaction.Rollback

                    foreach (var sqlCommand in _sqlCommands)
                        sqlCommand.Transaction = null;
                }

                item.OnComplete();

                return true;
            }

            protected abstract void ProcessInternal(T item);
        }

        private readonly DbConnectionManager _dbConnectionManager;

        protected ADbBatchWorker(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        protected abstract ADbItemWorker CreateDbItemWorker(DbConnectionHolder dbConnectionHolder);

        public void Process(LinkedList<T> batch)
        {
            int checkConnectionTimeout = InitialConnectionRetryInterval;

            while (batch.Count > 0)
            {
                DbConnectionHolder dbConnectionHolder =
                    _dbConnectionManager.Get();

                if (dbConnectionHolder == null)
                {
                    Thread.Sleep(checkConnectionTimeout);

                    if (checkConnectionTimeout < MaxConnectionRetryInterval)
                        checkConnectionTimeout += ConnectionRetryIntervalIncrement;

                    continue;
                }

                ProcessUsingConnection(
                    dbConnectionHolder,
                    batch);

                if (batch.Count == 0)
                    _dbConnectionManager.Return(dbConnectionHolder);
                else
                    dbConnectionHolder.Dispose();
            }
        }

        public abstract int InitialConnectionRetryInterval { get; }
        public abstract int MaxConnectionRetryInterval { get; }
        public abstract int ConnectionRetryIntervalIncrement { get; }
        public event Action<T> OnProcessItemSucceded;

        private void ProcessUsingConnection(
            DbConnectionHolder dbConnectionHolder, 
            LinkedList<T> currentBatch)
        {
            var dbItemWorker = CreateDbItemWorker(dbConnectionHolder);

            while (currentBatch.Count > 0)
            {
                T currentItem = currentBatch.First.Value;

                if (!dbItemWorker.Process(currentItem))
                    break;

                if (OnProcessItemSucceded != null)
                    try
                    {
                        OnProcessItemSucceded(currentItem);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                currentBatch.RemoveFirst();
            }
        }
    }
}
