using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Sys;
using Contal.LwSerialization;
using Contal.IwQuick.CrossPlatform;

namespace Contal.Cgp.Server.DB
{
    public sealed class DatabaseServerAlarms : ATableORM<DatabaseServerAlarms>
    {
        private abstract class BatchWorkerAction
        {
            public abstract void Process(MemoryStream memoryStream);
        }

        private class ActionInsertOrUpdateServerAlarm : BatchWorkerAction
        {
            private readonly ServerAlarm _serverAlarm;

            public ActionInsertOrUpdateServerAlarm(ServerAlarm serverAlarm)
            {
                _serverAlarm = serverAlarm;
            }

            public override void Process(MemoryStream memoryStream)
            {
                Singleton.InternalInsertOrUpdateServerAlarm(
                    _serverAlarm,
                    memoryStream);
            }
        }

        private class ActionDeleteServerAlarm : BatchWorkerAction
        {
            private readonly IdServerAlarm _idServerAlarm;

            public ActionDeleteServerAlarm(IdServerAlarm idServerAlarm)
            {
                _idServerAlarm = idServerAlarm;
            }

            public override void Process(MemoryStream memoryStream)
            {
                Singleton.InternalDeleteServerAlarm(_idServerAlarm);
            }
        }

        private class ActionDeleteServerAlarmsForOwner : BatchWorkerAction
        {
            private readonly Guid _idOwner;

            public ActionDeleteServerAlarmsForOwner(Guid idOwner)
            {
                _idOwner = idOwner;
            }

            public override void Process(MemoryStream memoryStream)
            {
                Singleton.InternalDeleteServerAlarmsForOwner(_idOwner);
            }
        }

        private class BatchWorkerOperationsExecutor : IBatchExecutor<BatchWorkerAction>
        {
            public int Execute(ICollection<BatchWorkerAction> requests)
            {
                var memoryStream = new MemoryStream();
                int count = 0;

                try
                {
                    foreach (var batchWorkerOperation in requests)
                    {
                        batchWorkerOperation.Process(memoryStream);
                        count++;
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
                finally
                {
                    memoryStream.Close();
                }

                return count;
            }
        }

        private const int DELAY_BETWEEN_DATABASE_OPERATIONS = 1000;
        private const int MAX_OBJECTS_COUNT_FOR_DATABSE_OPERATION = 2000;

        private readonly BatchWorker<BatchWorkerAction> _operationsBatchWorker;

        private DatabaseServerAlarms()
            : base(null)
        {
            _operationsBatchWorker = new BatchWorker<BatchWorkerAction>(
                new BatchWorkerOperationsExecutor(),
                DELAY_BETWEEN_DATABASE_OPERATIONS,
                MAX_OBJECTS_COUNT_FOR_DATABSE_OPERATION);

        }

        public void InsertOrUpdateServerAlarm(ServerAlarm serverAlarm)
        {
            _operationsBatchWorker.Add(
                new ActionInsertOrUpdateServerAlarm(serverAlarm));
        }

        private void InternalInsertOrUpdateServerAlarm(
            ServerAlarm serverAlarm,
            MemoryStream memoryStream)
        {
            try
            {
                var serializer = new LwBinarySerializer<ServerAlarm>(memoryStream);

                serializer.Serialize(serverAlarm);

                var databaseServerAlarm = GetDatabaseServerAlarm(serverAlarm.ServerAlarmCore.IdServerAlarm);

                if (databaseServerAlarm != null)
                {
                    databaseServerAlarm.RawData = memoryStream.ToArray();

                    Update(databaseServerAlarm);

                    return;
                }

                databaseServerAlarm = new DatabaseServerAlarm
                {
                    IdOwner = serverAlarm.ServerAlarmCore.IdServerAlarm.IdOwner,
                    IdAlarm = serverAlarm.ServerAlarmCore.IdServerAlarm.Id,
                    RawData = memoryStream.ToArray()
                };

                Insert(databaseServerAlarm, null);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                memoryStream.SetLength(0);
            }
        }

        private DatabaseServerAlarm GetDatabaseServerAlarm(IdServerAlarm idServerAlarm)
        {
            var databaseServerAlarms = SelectLinq<DatabaseServerAlarm>(
                databaseServerAlarm =>
                    databaseServerAlarm.IdOwner.Equals(idServerAlarm.IdOwner)
                    && databaseServerAlarm.IdAlarm.Equals(idServerAlarm.Id));

            return databaseServerAlarms != null
                ? databaseServerAlarms.FirstOrDefault()
                : null;
        }

        public void DeleteServerAlarm(IdServerAlarm idServerAlarm)
        {
            _operationsBatchWorker.Add(
                new ActionDeleteServerAlarm(idServerAlarm));
        }

        private void InternalDeleteServerAlarm(IdServerAlarm idServerAlarm)
        {
            var databaseServerAlarm = GetDatabaseServerAlarm(idServerAlarm);

            if (databaseServerAlarm == null)
                return;

            Delete(databaseServerAlarm);
        }

        public void DeleteServerAlarmsForOwner(Guid idOwner)
        {
            _operationsBatchWorker.Add(
                new ActionDeleteServerAlarmsForOwner(idOwner));
        }

        private void InternalDeleteServerAlarmsForOwner(Guid idOwner)
        {
            var databaseServerAlarms = GetDatabaseServerAlarmsForOwner(idOwner);

            if (databaseServerAlarms == null)
                return;

            foreach (var databaseServerAlarm in databaseServerAlarms)
            {
                Delete(databaseServerAlarm);
            }
        }

        private IEnumerable<DatabaseServerAlarm> GetDatabaseServerAlarmsForOwner(
            Guid idOwner)
        {
            return SelectLinq<DatabaseServerAlarm>(
                databaseServerAlarm =>
                    databaseServerAlarm.IdOwner.Equals(idOwner));
        }

        public IEnumerable<ServerAlarm> GetServerAlarmsForOwner(Guid idOwner)
        {
            var databaseServerAlarms = GetDatabaseServerAlarmsForOwner(idOwner);

            if (databaseServerAlarms == null)
                yield break;

            var memoryStream = new MemoryStream();

            foreach (var databaseServerAlarm in databaseServerAlarms)
            {
                ServerAlarm serverAlarm;

                try
                {
                    memoryStream.Write(databaseServerAlarm.RawData, 0, databaseServerAlarm.RawData.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var deserializer = new LwBinaryDeserializer<ServerAlarm>(memoryStream);

                    serverAlarm = deserializer.Deserialize();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    continue;
                }
                finally
                {
                    memoryStream.SetLength(0);
                }

                yield return serverAlarm;
            }

            memoryStream.Close();
        }
    }
}
