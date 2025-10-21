using System;
using System.Linq;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public abstract class ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> 
        : ASqlCeDbAccessor<TObject, TPrimaryKey>
        , ISqlCeDbAccessorBase
        where TObject : class
    {
        private class ObjectReference
        {
            public TObject Object { get; set; }
            public bool InProgress { get; set; }
            public bool UpdatedDuringProgress { get; set; }

            public ObjectReference(TObject objectReference)
            {
                Object = objectReference;
                InProgress = false;
                UpdatedDuringProgress = false;
            }
        }

        private abstract class AWorkerItem
        {
            protected TPrimaryKey PrimaryKey { get; private set; }

            protected AWorkerItem(TPrimaryKey primaryKey)
            {
                PrimaryKey = primaryKey;
            }

            public abstract void Execute(ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> sqlCeDbDelayedAccessor);
        }

        private class DeleteFromDatabaseWorker : AWorkerItem
        {
            public DeleteFromDatabaseWorker(TPrimaryKey primaryKey)
                : base (primaryKey)
            {
                
            }

            public override void Execute(ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> sqlCeDbDelayedAccessor)
            {
                sqlCeDbDelayedAccessor.PerformDeleteFromDatabase(PrimaryKey);
            }
        }

        private class BatchExecutor : ABatchExecutor<AWorkerItem, ASqlCeDbDelayedAccessor<TObject, TPrimaryKey>>
        {
            public BatchExecutor(ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> sqlCeDbDelayedAccessor) 
                : base(sqlCeDbDelayedAccessor)
            {
            }

            protected override bool OnError(AWorkerItem request, Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return true;
            }

            protected override void ExecuteInternal(
                AWorkerItem request, 
                ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> param)
            {
                request.Execute(param);
            }
        }

        private void PerformDeleteFromDatabase(TPrimaryKey primaryKey)
        {
            lock (_databaseOperationLock)
            {
                lock (_inMemoryDataLock)
                    if (!_deletedObjects.Remove(primaryKey))
                        return;

                DeleteFromDatabaseInternal(primaryKey);
            }
        }

        private class SaveToDatabaseWorker : AWorkerItem
        {
            public SaveToDatabaseWorker(TPrimaryKey primaryKey)
                : base(primaryKey)
            {

            }

            public override void Execute(ASqlCeDbDelayedAccessor<TObject, TPrimaryKey> sqlCeDbDelayedAccessor)
            {
                sqlCeDbDelayedAccessor.PerformSaveToDatabase(PrimaryKey);
            }
        }

        private void PerformSaveToDatabase(TPrimaryKey primaryKey)
        {
            lock (_databaseOperationLock)
            {
                ObjectReference savedObjectReference;

                lock (_inMemoryDataLock)
                {
                    if (!_savedObjectReferences.TryGetValue(
                        primaryKey,
                        out savedObjectReference))
                    {
                        return;
                    }

                    savedObjectReference.InProgress = true;
                }

                SaveToDatabaseInternal(
                    Enumerable.Repeat(
                        new KeyValuePair<TPrimaryKey, TObject>(
                            primaryKey,
                            savedObjectReference.Object),
                        1),
                    null,
                    null);

                lock (_inMemoryDataLock)
                {
                    if (savedObjectReference.UpdatedDuringProgress)
                    {
                        savedObjectReference.InProgress = false;
                        savedObjectReference.UpdatedDuringProgress = false;

                        return;
                    }

                    _savedObjectReferences.Remove(primaryKey);
                }
            }
        }

        private const int DELAY_BETWEEN_DATABASE_OPERATIONS = 1000;
        private const int MAX_OBJECTS_COUNT_FOR_DATABSE_OPERATION = 2000;

        private readonly Dictionary<TPrimaryKey, ObjectReference> _savedObjectReferences =
            new Dictionary<TPrimaryKey, ObjectReference>();

        private readonly HashSet<TPrimaryKey> _deletedObjects = new HashSet<TPrimaryKey>();

        private volatile object _inMemoryDataLock = new object();

        private volatile object _databaseOperationLock = new object();

        private readonly BatchWorker<AWorkerItem> _batchWorker;

        protected ASqlCeDbDelayedAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            string tableName,
            [NotNull] IEnumerable<IPrimaryKeyDbColumn<TObject, TPrimaryKey>> primaryKeyDbColumns,
            IObjectCreator<TObject> objectCreator)
            : base(
                sqlCeDbCommandFactory,
                tableName,
                primaryKeyDbColumns,
                null,
                objectCreator)
        {
            _batchWorker = new BatchWorker<AWorkerItem>(
                new BatchExecutor(this),
                DELAY_BETWEEN_DATABASE_OPERATIONS,
                MAX_OBJECTS_COUNT_FOR_DATABSE_OPERATION);
        }

        public ICollection<TPrimaryKey> GetPrimaryKeys()
        {
            lock (_databaseOperationLock)
                lock (_inMemoryDataLock)
                    return
                        new HashSet<TPrimaryKey>(
                            PrimaryKeyValues
                                .Concat(
                                    _savedObjectReferences.Keys)
                                .Where(
                                    primaryKey =>
                                        !_deletedObjects.Contains(primaryKey)));
        }

        public TObject GetFromDatabase(TPrimaryKey primaryKey)
        {
            lock (_databaseOperationLock)
            {
                lock (_inMemoryDataLock)
                {
                    if (_deletedObjects.Contains(primaryKey))
                        return null;

                    ObjectReference savedObjectReference;

                    if (_savedObjectReferences.TryGetValue(
                        primaryKey,
                        out savedObjectReference))
                    {
                        return savedObjectReference.Object;
                    }
                }

                return GetFromDatabaseInternal(primaryKey);
            }
        }

        protected abstract TPrimaryKey GetPrimaryKey(TObject obj);

        protected ICollection<TObject> GetAllFromDatabase()
        {
            ICollection<TObject> result = new LinkedList<TObject>();

            lock (_databaseOperationLock)
            {
                lock (_inMemoryDataLock)
                {
                    var savedObjectsPrimaryKeys = new HashSet<TPrimaryKey>();

                    foreach (var savedObjectReference in _savedObjectReferences.Values)
                    {
                        result.Add(savedObjectReference.Object);
                        savedObjectsPrimaryKeys.Add(GetPrimaryKey(savedObjectReference.Object));
                    }

                    var objectsFromDatabase = GetAllFromDatabaseInternal();

                    if (objectsFromDatabase != null)
                        foreach (var objectFromDatabase in objectsFromDatabase)
                        {
                            var primaryKey = GetPrimaryKey(objectFromDatabase);

                            if (_deletedObjects.Contains(primaryKey)
                                || savedObjectsPrimaryKeys.Contains(primaryKey))
                            {
                                continue;
                            }

                            result.Add(objectFromDatabase);
                        }
                }
            }

            return result.Count > 0
                ? result
                : null;
        }

        public void SaveToDatabase(TPrimaryKey primaryKey, TObject databaseObject)
        {
            lock (_inMemoryDataLock)
            {
                _deletedObjects.Remove(primaryKey);

                ObjectReference savedObjectReference;

                if (_savedObjectReferences.TryGetValue(
                    primaryKey,
                    out savedObjectReference))
                {
                    savedObjectReference.Object = databaseObject;

                    if (!savedObjectReference.InProgress)
                        return;

                    savedObjectReference.UpdatedDuringProgress = true;
                }
                else
                {
                    _savedObjectReferences.Add(
                        primaryKey,
                        new ObjectReference(databaseObject));
                }

                _batchWorker.Add(new SaveToDatabaseWorker(primaryKey));
            }
        }

        public void DeleteFromDatabase(TPrimaryKey primaryKey)
        {
            lock (_inMemoryDataLock)
            {
                if (!_deletedObjects.Add(primaryKey))
                    return;

                _savedObjectReferences.Remove(primaryKey);

                _batchWorker.Add(new DeleteFromDatabaseWorker(primaryKey));
            }
        }

        public void Load()
        {
            lock (_databaseOperationLock)
                LoadPrimaryKeysInternal();
        }

        public void DeleteAllFromDatabase()
        {
            lock (_databaseOperationLock)
            {
                lock (_inMemoryDataLock)
                {
                    _savedObjectReferences.Clear();
                    _deletedObjects.Clear();

                    _batchWorker.Clear(
                        () =>
                        {
                            lock (_databaseOperationLock)
                                DeleteAllFromDatabaseInternal();
                        });
                }
            }
        }
    }
}
