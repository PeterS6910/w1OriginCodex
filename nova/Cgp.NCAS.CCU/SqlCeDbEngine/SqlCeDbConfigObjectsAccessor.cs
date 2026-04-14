using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public class SqlCeDbConfigObjectsAccessor<TObject> :
        ASqlCeDbAccessor<TObject, Guid>,
        ISqlCeDbAccessor
        where TObject : class, IDbObject
    {
        protected class GuidPrimaryKeyDbColumn : IPrimaryKeyDbColumn<TObject, Guid>
        {
            public object GetValue(TObject obj)
            {
                return obj.GetGuid();
            }

            public string Name { get; private set; }

            public string DbTypeString
            {
                get { return "uniqueidentifier"; }
            }

            public SqlDbType DbType { get { return SqlDbType.UniqueIdentifier; } }
            public int? DbSize { get { return null; } }
            public bool AllowNull{ get { return false; } }

            public GuidPrimaryKeyDbColumn(string columnName)
            {
                Name = columnName;
            }

            public object GetValueFromPrimaryKey(Guid primaryKey)
            {
                return primaryKey;
            }
        }

        private readonly string _primaryKeyDbColumnName;
        private readonly HashSet<Guid> _recentlySavedObjectIds = new HashSet<Guid>();

        protected volatile object OperationLock = new object();

        private readonly ObjectType _objectType;

        private readonly IDbObjectChangeListener<TObject> _dbObjectChangedListener;
        private readonly EventHandlerGroup<IDbObjectRemovalListener> _dbObjectRemovalListener;

        public SqlCeDbConfigObjectsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            ObjectType objectType,
            string primaryKeyDbColumnName,
            IObjectCreator<TObject> objectCreator,
            IDbObjectChangeListener<TObject> dbObjectChangedListener,
            [NotNull] EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener)
            : this(
                sqlCeDbCommandFactory,
                typeof(TObject).Name,
                objectType,
                primaryKeyDbColumnName,
                null,
                objectCreator,
                dbObjectChangedListener,
                dbObjectRemovalListener)
        {
        }

        protected SqlCeDbConfigObjectsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            string tableName,
            ObjectType objectType,
            string primaryKeyDbColumnName,
            ICollection<DbIndex<TObject>> dbIndexes,
            IObjectCreator<TObject> objectCreator,
            IDbObjectChangeListener<TObject> dbObjectChangedListener,
            EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener)
            : this(
                sqlCeDbCommandFactory,
                tableName,
                objectType,
                primaryKeyDbColumnName,
                Enumerable.Repeat<IPrimaryKeyDbColumn<TObject, Guid>>(
                    new GuidPrimaryKeyDbColumn(primaryKeyDbColumnName),
                    1),
                dbIndexes,
                objectCreator,
                dbObjectChangedListener,
                dbObjectRemovalListener)
        {
        }

        protected SqlCeDbConfigObjectsAccessor(
            ISqlCeDbCommandFactory sqlCeDbCommandFactory,
            string tableName,
            ObjectType objectType,
            string primaryKeyDbColumnName,
            IEnumerable<IPrimaryKeyDbColumn<TObject, Guid>> primaryKeyDbColumns,
            ICollection<DbIndex<TObject>> dbIndexes,
            IObjectCreator<TObject> objectCreator,
            IDbObjectChangeListener<TObject> dbObjectChangedListener,
            EventHandlerGroup<IDbObjectRemovalListener> dbObjectRemovalListener)
            : base(
                sqlCeDbCommandFactory,
                tableName,
                primaryKeyDbColumns,
                dbIndexes,
                objectCreator)
        {
            _objectType = objectType;
            _primaryKeyDbColumnName = primaryKeyDbColumnName;

            _dbObjectChangedListener = dbObjectChangedListener;
            _dbObjectRemovalListener = dbObjectRemovalListener;
        }

        public void Load()
        {
            lock (OperationLock)
            {
                LoadPrimaryKeysInternal();

                foreach (var primaryKey in PrimaryKeyValues)
                {
                    _recentlySavedObjectIds.Add(primaryKey);
                }
            }
        }

        public ICollection<Guid> GetPrimaryKeys()
        {
            lock (OperationLock)
                return new HashSet<Guid>(PrimaryKeyValues);
        }

        public bool ContainsAnyObjects()
        {
            lock (OperationLock)
                return PrimaryKeyValues.Count > 0;
        }

        public object GetFromDatabase(Guid idObject)
        {
            lock (OperationLock)
                return GetFromDatabaseInternal(idObject);
        }

        public bool Exists(Guid idObject)
        {
            lock (OperationLock)
                return PrimaryKeyValues.Contains(idObject);
        }

        public void SaveToDatabase(
            IEnumerable<IDbObject> newObjects,
            bool executePrepareDeleteOrUpdate)
        {
            if (newObjects == null)
                return;

            SaveToDatabaseInternal(
                newObjects.Select(
                    obj =>
                        new KeyValuePair<Guid, TObject>(
                            obj.GetGuid(),
                            (TObject) obj)),
                obj => BeforSaveToDatase(obj, executePrepareDeleteOrUpdate),
                AfterSaveToDatabase);
        }

        private void BeforSaveToDatase(TObject obj, bool executePrepareDeleteOrUpdate)
        {
            var idObj = obj.GetGuid();

            if (_dbObjectChangedListener != null)
            {
                lock (OperationLock)
                    if (!PrimaryKeyValues.Contains(idObj))
                        executePrepareDeleteOrUpdate = false;

                if (executePrepareDeleteOrUpdate)
                {
                    _dbObjectChangedListener.PrepareObjectUpdate(
                        idObj,
                        obj);
                }
            }
        }

        private void AfterSaveToDatabase(TObject obj)
        {
            var idObj = obj.GetGuid();

            _recentlySavedObjectIds.Add(idObj);

            if (_dbObjectChangedListener != null)
                _dbObjectChangedListener.OnObjectSaved(
                    idObj,
                    obj);
        }

        public void DeleteFromDatabase(Guid idObject)
        {
            lock (OperationLock)
                if (!PrimaryKeyValues.Contains(idObject))
                    return;

            if (_dbObjectChangedListener != null)
                _dbObjectChangedListener.PrepareObjectDelete(idObject);

            _dbObjectRemovalListener.ForEach(
                listener => listener.PrepareObjectDelete(
                    idObject,
                    _objectType));

            lock (OperationLock)
            {
                DeleteFromDatabaseInternal(idObject);

                if (PrimaryKeyValues.Count == 0)
                    SaveVersion(-1);
            }
        }

        public void DeleteAllFromDatabase()
        {
            foreach (var primaryKey in GetPrimaryKeys())
            {
                if (_dbObjectChangedListener != null)
                    _dbObjectChangedListener.PrepareObjectDelete(primaryKey);

                var idObject = primaryKey;

                _dbObjectRemovalListener.ForEach(
                    listener => listener.PrepareObjectDelete(
                        idObject,
                        _objectType));
            }

            lock (OperationLock)
            {
                DeleteAllFromDatabaseInternal();

                SaveVersion(-1);
            }
        }

        protected override Guid CreatePrimaryKey(Dictionary<string, object> primaryKeyDbColumnsValues)
        {
            return (Guid)primaryKeyDbColumnsValues[_primaryKeyDbColumnName];
        }

        public void SaveVersion(int version)
        {
            SqlCeDbVersionHelper.Singleton.SaveVersion(
                _objectType,
                version);
        }

        public MaximumVersionAndIds GetMaximumVersionAndIds()
        {
            return new MaximumVersionAndIds(
                SqlCeDbVersionHelper.Singleton.ReadVersion(_objectType),
                PrimaryKeyValues);
        }

        public ICollection<Guid> GetIdsOfRecentlySavedObjects()
        {
            lock (OperationLock)
                return _recentlySavedObjectIds.Count > 0
                    ? new HashSet<Guid>(_recentlySavedObjectIds)
                    : null;
        }

        public void OnApplyChangesDone()
        {
            lock (OperationLock)
                _recentlySavedObjectIds.Clear();
        }
    }
}
