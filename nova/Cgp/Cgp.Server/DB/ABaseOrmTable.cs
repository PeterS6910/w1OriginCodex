using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.ORM;
using Contal.Cgp.Server.Alarms;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

using NHibernate;
using NHibernate.Criterion;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

using SqlDeleteReferenceConstraintException = Contal.IwQuick.SqlDeleteReferenceConstraintException;

namespace Contal.Cgp.Server.DB
{
    public interface ICudPreparation<T>
    {
        void BeforeCreate(T obj);
        void BeforeUpdate(T obj);
        void BeforeDelete(T obj);
    }

    public abstract class ABaseOrmTable<TSingleton, T> : 
        ATableORM<TSingleton>, 
        ITableORM<T>, 
        IBaseOrmTable<T>,
        ICudSpecialExecutor<T>
        where TSingleton : ATableORM<TSingleton>
        where T : AOrmObject
    {
        private abstract class Request
        {
            public abstract void Execute(ABaseOrmTable<TSingleton, T> baseOrmTable);
        }

        private ICudPreparation<T> _beforeCUD;
        private ProcessingQueue<Request> _requestProcessingQueue;

        protected ABaseOrmTable(TSingleton singleton) :
            this(singleton, null)
        {
        }

        protected ABaseOrmTable(TSingleton singleton, ICudPreparation<T> beforeIUD) :
            base(singleton)
        {
            NhHelper.Singleton.ConnectionString =
                ConnectionString.LoadFromRegistry(
                    CgpServerGlobals.REGISTRY_CONNECTION_STRING);

            ThisAssembly = typeof(T).Assembly;

            _requestProcessingQueue = new ProcessingQueue<Request>();
            _requestProcessingQueue.ItemProcessing += _requestProcessingQueue_ItemProcessing;

            _beforeCUD = beforeIUD;
        }

        void _requestProcessingQueue_ItemProcessing(Request request)
        {
            request.Execute(this);
        }

        #region ITableORM<T> Members

        public virtual bool Insert(ref T ormObject)
        {
            return InsertWithObjectId(
                ref ormObject,
                null,
                null);
        }

        public virtual bool Insert(
            ref T ormObject,
            int? idStructuredSubSite)
        {
            return InsertWithObjectId(
                ref ormObject,
                null,
                idStructuredSubSite);
        }

        public bool InsertWithObjectId(
            ref T ormObject,
            object objectId)
        {
            return InsertWithObjectId(
                ref ormObject,
                objectId,
                null);
        }

        public bool InsertWithObjectId(
            ref T ormObject,
            object objectId,
            int? idStructuredSubSite)
        {
            return Insert(
                ref ormObject,
                objectId,
                idStructuredSubSite,
                true);
        }

        public bool InsertOnlyInDatabase(ref T ormObject)
        {
            return InsertOnlyInDatabaseWithObjectId(
                ref ormObject,
                null,
                null);
        }

        public bool InsertOnlyInDatabase(
            ref T ormObject,
            int? idStructuredSubSite)
        {
            return InsertOnlyInDatabaseWithObjectId(
                ref ormObject,
                null,
                idStructuredSubSite);
        }

        public bool InsertOnlyInDatabaseWithObjectId(
            ref T ormObject,
            object objectId)
        {
            return InsertOnlyInDatabaseWithObjectId(
                ref ormObject,
                objectId,
                null);
        }

        public bool InsertOnlyInDatabaseWithObjectId(
            ref T ormObject,
            object objectId,
            int? idStructuredSubSite)
        {
            return Insert(
                ref ormObject,
                objectId,
                idStructuredSubSite,
                false);
        }

        private bool Insert(
            ref T ormObject,
            object objectId,
            int? idStructuredSubSite,
            bool doCudSpecial)
        {
            Exception error;

            if (!CheckData(ormObject, out error))
                return false;

            SaveOnOffObjects(ormObject);

            if (_beforeCUD != null)
                _beforeCUD.BeforeCreate(ormObject);

            SetFullTextSearchString(ormObject);

            if (!base.Insert(ormObject, objectId))
                return false;

            DoCUDObjectEvent(
                ormObject.GetObjectType(),
                ormObject.GetId(),
                true);

            if (idStructuredSubSite != null)
                StructuredSubSites.Singleton.AddObjectToSite(
                    ormObject.GetId(),
                    ormObject.GetObjectType(),
                    idStructuredSubSite.Value);

            AfterInsert(ormObject);

            if (doCudSpecial)
            {
                CudSpecialQueue.Singleton.Enqueue(
                    GetById(ormObject.GetId()),
                    ObjectDatabaseAction.Insert,
                    this);
            }

            return true;
        }

        public virtual bool Insert(ref T ormObject, out Exception error)
        {
            if (!HasAccessInsert())
            {
                error = new AccessDeniedException();
                return false;
            }

            if (!CheckData(ormObject, out error))
                return false;

            SaveOnOffObjects(ormObject);

            if (_beforeCUD != null)
                _beforeCUD.BeforeCreate(ormObject);

            SetFullTextSearchString(ormObject);

            if (!base.Insert(ormObject, out error))
                return false;

            DoCUDObjectEvent(ormObject.GetObjectType(), ormObject.GetId(), true);

            AfterInsert(ormObject);

            CudSpecialQueue.Singleton.Enqueue(
                GetById(ormObject.GetId()),
                ObjectDatabaseAction.Insert,
                this);

            return true;
        }

        private void SetFullTextSearchString(AOrmObject ormObject)
        {
            var iSetFullTextSearchString = ormObject as ISetFullTextSearchString;

            if (iSetFullTextSearchString != null)
            {
                var fullTextSearchString = new StringBuilder(iSetFullTextSearchString.Name);

                var alaternateName = iSetFullTextSearchString.AlternateName;

                if (!string.IsNullOrEmpty(alaternateName))
                    fullTextSearchString.AppendFormat(
                        "\t{0}",
                        alaternateName);

                var otherFullTextSearchStrings = iSetFullTextSearchString.OtherFullTextSearchStrings;

                if (otherFullTextSearchStrings != null)
                    foreach (var otherFullTextSearchString in otherFullTextSearchStrings)
                    {
                        fullTextSearchString.AppendFormat(
                        "\t{0}",
                        otherFullTextSearchString);
                    }

                iSetFullTextSearchString.FullTextSearchString =
                    fullTextSearchString.ToString();
            }
        }

        public bool Update(T ormObject)
        {
            return Update(ormObject, true);
        }

        public bool UpdateOnlyInDatabase(T ormObject)
        {
            return Update(ormObject, false);
        }

        private bool Update(T ormObject, bool doCUDSpecial)
        {
            Exception error;
            if (!CheckData(ormObject, out error))
                return false;

            var oldObjectBeforUpdate = GetById(ormObject.GetId());
            BeforeUpdate(oldObjectBeforUpdate);

            SaveOnOffObjects(ormObject);
            ReloadBeforeUpdateDelete(ormObject);

            var retValue = false;

            if (doCUDSpecial && _beforeCUD != null)
                _beforeCUD.BeforeUpdate(ormObject);

            var guidAndObjectType =
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

            SetFullTextSearchString(ormObject);

            if (CentralTransactionControl.Singleton.BeginUpdate(guidAndObjectType))
            {
                try
                {
                    retValue = base.Update(ormObject);
                }
                finally
                {
                    CentralTransactionControl.Singleton.EndUpdate(
                        new IdAndObjectType(
                            ormObject.GetId(),
                            ormObject.GetObjectType()), 
                        retValue);
                }

                if (retValue)
                {
                    DoCUDObjectEvent(ormObject.GetObjectType(), ormObject.GetId(), false);

                    AfterUpdate(
                        ormObject,
                        oldObjectBeforUpdate);

                    if (doCUDSpecial)
                    {
                        CudSpecialQueue.Singleton.Enqueue(
                            GetById(ormObject.GetId()),
                            ObjectDatabaseAction.Update,
                            this);
                    }

                    return true;
                }
            }

            if (doCUDSpecial)
                AfterUDFailed(ormObject);

            return false;
        }

        public bool Update(T ormObject, out Exception error)
        {
            return Update(ormObject, out error, true);
        }

        public bool UpdateOnlyInDatabase(T ormObject, out Exception error)
        {
            return Update(ormObject, out error, false);
        }

        public bool Update(T ormObject, out Exception error, bool doCUDSpecial)
        {
            if (!HasAccessUpdate() ||
                !StructuredSubSites.Singleton.HasAccessUpdate(ormObject))
            {
                error = new AccessDeniedException();
                return false;
            }

            if (!CheckData(ormObject, out error))
                return false;

            var oldObjectBeforUpdate = GetById(ormObject.GetId());
            BeforeUpdate(oldObjectBeforUpdate);

            SaveOnOffObjects(ormObject);
            ReloadBeforeUpdateDelete(ormObject);

            var retValue = false;

            if (doCUDSpecial && _beforeCUD != null)
                _beforeCUD.BeforeUpdate(ormObject);

            var guidAndObjectType =
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

            SetFullTextSearchString(ormObject);

            if (CentralTransactionControl.Singleton.BeginUpdate(guidAndObjectType))
            {
                try
                {
                    retValue = base.Update(ormObject, out error);
                }
                finally
                {
                    CentralTransactionControl.Singleton
                        .EndUpdate(
                            new IdAndObjectType(
                                ormObject.GetId(),
                                ormObject.GetObjectType()), 
                            retValue);
                }

                if (retValue)
                {
                    DoCUDObjectEvent(ormObject.GetObjectType(), ormObject.GetId(), false);

                    AfterUpdate(
                        ormObject,
                        oldObjectBeforUpdate);

                    if (doCUDSpecial)
                    {
                        CudSpecialQueue.Singleton.Enqueue(
                            GetById(ormObject.GetId()),
                            ObjectDatabaseAction.Update,
                            this);
                    }

                    return true;
                }
            }
            else
                error = new IncoherentDataException();

            if (doCUDSpecial)
                AfterUDFailed(ormObject);

            return false;
        }

        protected virtual void SaveOnOffObjects(T ormObject)
        {
        }

        /// <summary>
        /// Reload object before updating or deleting
        /// </summary>
        /// <param name="ormObject"></param>
        protected virtual void ReloadBeforeUpdateDelete(T ormObject)
        {
        }

        public virtual bool CheckData(T ormObject, out Exception error)
        {
            error = null;
            return true;
        }

        public virtual bool Delete(T ormObject)
        {
            var referencedObjects =
                GetReferencedObjectsAllPlugins(ormObject.GetId());

            if (referencedObjects != null &&
                referencedObjects.Count > 0 &&
                !DeleteIfReferenced(ormObject.GetId(), referencedObjects))
                return false;

            if (IsReferencedSubObjects(ormObject))
                return false;

            ReloadBeforeUpdateDelete(ormObject);
            BeforeDelete(ormObject);

            if (_beforeCUD != null)
                _beforeCUD.BeforeDelete(ormObject);

            var retValue = false;

            var guidAndObjectType =
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

            var isObjectBeingEdited = CentralTransactionControl.Singleton.BeginDelete(guidAndObjectType);

            try
            {
                // TODO pass on some sensible session for this call as well
                retValue = Delete(ormObject, null);
            }
            finally
            {
                if (isObjectBeingEdited)
                    CentralTransactionControl.Singleton.EndDelete(
                        guidAndObjectType, 
                        retValue);
            }

            if (retValue)
            {
                UserFoldersStructureObjects.Singleton.DeleteObject(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                AfterDelete(ormObject);
                DoCUDObjectEvent(ormObject.GetObjectType(), ormObject.GetId(), false);
                DoCUDSpecialDelete(ormObject);

                return true;
            }

            AfterUDFailed(ormObject);
            return false;
        }

        public virtual bool Delete(T ormObject, out Exception error)
        {
            error = null;

            if (!HasAccessDelete() ||
                !StructuredSubSites.Singleton.HasAccessDelete(ormObject))
            {
                error = new AccessDeniedException();
                return false;
            }

            var referencedObjects =
                GetReferencedObjectsAllPlugins(ormObject.GetId());

            if (referencedObjects != null &&
                referencedObjects.Count > 0 &&
                !DeleteIfReferenced(ormObject.GetId(), referencedObjects))
            {
                error = new SqlDeleteReferenceConstraintException();
                return false;
            }

            if (IsReferencedSubObjects(ormObject))
            {
                error = new SqlDeleteReferenceConstraintException();
                return false;
            }

            ReloadBeforeUpdateDelete(ormObject);
            BeforeDelete(ormObject);

            if (_beforeCUD != null)
                _beforeCUD.BeforeDelete(ormObject);

            var retValue = false;

            var guidAndObjectType = 
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

            var isObjectBeingEdited = CentralTransactionControl.Singleton.BeginDelete(guidAndObjectType);

            try
            {
                retValue = Delete(ormObject, null, out error);
            }
            finally
            {
                if (isObjectBeingEdited)
                    CentralTransactionControl.Singleton.EndDelete(
                        guidAndObjectType, 
                        retValue);
            }

            if (retValue)
            {
                UserFoldersStructureObjects.Singleton.DeleteObject(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                AfterDelete(ormObject);
                DoCUDObjectEvent(ormObject.GetObjectType(), ormObject.GetId(), false);
                DoCUDSpecialDelete(ormObject);

                return true;
            }

            AfterUDFailed(ormObject);
            return false;
        }

        public virtual bool DeleteIfReferenced(
            object id,
            IList<AOrmObject> referencedObjects)
        {
            return false;
        }

        public bool DeleteById(object id, out Exception error)
        {
            if (!HasAccessDelete())
            {
                error = new AccessDeniedException();
                return false;
            }

            ISession session;
            var ormObject = GetById(id, out session);

            if (!StructuredSubSites.Singleton.HasAccessDelete(ormObject))
            {
                error = new AccessDeniedException();
                return false;

            } 

            return DeleteInternal(ormObject, session, out error);
        }

        private bool DeleteInternal(
            T ormObject, 
            ISession session,
            out Exception error)
        {
            error = null;

            var referencedObjects =
                GetReferencedObjectsAllPlugins(ormObject.GetId());

            if (referencedObjects != null &&
                referencedObjects.Count > 0 &&
                !DeleteIfReferenced(
                    ormObject.GetId(),
                    referencedObjects))
            {
                error = new SqlDeleteReferenceConstraintException();
                return false;
            }

            if (IsReferencedSubObjects(ormObject))
            {
                error = new SqlDeleteReferenceConstraintException();
                return false;
            }

            ReloadBeforeUpdateDelete(ormObject);
            BeforeDelete(ormObject);

            if (_beforeCUD != null)
                _beforeCUD.BeforeDelete(ormObject);

            var retValue = false;

            var guidAndObjectType =
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

            var isObjectBeingEdited =
                CentralTransactionControl.Singleton
                    .BeginDelete(guidAndObjectType);

            try
            {
                retValue = Delete(ormObject, session, out error);
            }
            finally
            {
                if (isObjectBeingEdited)
                    CentralTransactionControl.Singleton.EndDelete(
                        guidAndObjectType,
                        retValue);
            }

            if (retValue)
            {
                UserFoldersStructureObjects.Singleton.DeleteObject(
                    new IdAndObjectType(
                        ormObject.GetId(),
                        ormObject.GetObjectType()));

                AfterDelete(ormObject);

                DoCUDObjectEvent(
                    ormObject.GetObjectType(), 
                    ormObject.GetId(), 
                    false);

                DoCUDSpecialDelete(ormObject);

                return true;
            }

            AfterUDFailed(ormObject);
            return false;
        }

        public void DeleteByCriteria(
            System.Linq.Expressions.Expression<Func<T, bool>> func,
            ISession session)
        {
            foreach (var obj in SelectLinq(func, ref session))
            {
                Exception err;
                DeleteInternal(obj, session, out err);
            }
        }

        public bool DeleteById(object id)
        {
            Exception err;

            ISession session;
            var ormObject = GetById<T>(id, out session);

            return DeleteInternal(ormObject, session, out err);
        }

        public virtual bool IsReferencedSubObjects(T ormObject)
        {
            return false;
        }

        public virtual void AfterUDFailed(T obj)
        {
        }

        public virtual void CUDSpecial(T ormObject, ObjectDatabaseAction objectDatabaseAction)
        {
        }

        public virtual void AfterInsert(T newObject)
        {
        }

        public virtual void BeforeUpdate(T oldObj)
        {
        }

        public virtual void AfterUpdate(T newObject, T oldObjectBeforUpdate)
        {
        }

        public virtual void BeforeDelete(T objToDelete)
        {
        }

        public virtual void AfterDelete(T deletedObject)
        {
        }

        private void DoCUDSpecialDelete(T ormObject)
        {
            CudSpecialQueue.Singleton.Enqueue(
                ormObject,
                ObjectDatabaseAction.Delete,
                this);

            object[] deletedObjParams =
            {
                ormObject.GetObjectType(),
                ormObject.GetId()
            };

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunDeletedObjectEvent,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                deletedObjParams);
        }

        private static void RunDeletedObjectEvent(
            ARemotingCallbackHandler remoteHandler,
            object[] values)
        {
            if (values.Length != 2 || !(values[0] is ObjectType) || !(values[1] is object))
                return;

            var deletedObjectHandler =
                remoteHandler as DeletedObjectHandler;

            if (deletedObjectHandler != null)
                deletedObjectHandler.RunEvent(
                    (ObjectType)values[0],
                    values[1]);
        }

        public static void DoCUDObjectEvent(ObjectType objectType, object objectGuid, bool isInserted)
        {
            DoCUDObjectEventForServer(
                objectType,
                objectGuid,
                isInserted);

            object[] objs =
            {
                objectType, 
                objectGuid, 
                isInserted
            };

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunCUDObjectEvent,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                objs);
        }

        private static void DoCUDObjectEventForServer(
            ObjectType objectType,
            object objectGuid,
            bool isInserted)
        {
            CUDObjectHandlerForServer.Singleton.RunEvent(
                objectType,
                objectGuid,
                isInserted);
        }

        private static void RunCUDObjectEvent(ARemotingCallbackHandler remoteHandler, object[] obj)
        {
            if (obj == null || obj.Length != 3 || !(obj[0] is ObjectType) || !(obj[2] is bool))
                return;

            var cudObjectHandler =
                remoteHandler as CUDObjectHandler;

            if (cudObjectHandler != null)
                cudObjectHandler.RunEvent(
                    (ObjectType)obj[0],
                    obj[1],
                    (bool)obj[2]);
        }

        public T GetById(object id, out ISession session)
        {
            var obj = GetById<T>(id, out session);

            if (obj == null)
                return null;

            LoadObjectsInRelationshipGetById(obj);
            ReadOnOffObject(obj);

            return obj;
        }

        public T GetById(object id)
        {
            var obj = GetById<T>(id);

            if (obj == null)
                return null;

            LoadObjectsInRelationshipGetById(obj);
            ReadOnOffObject(obj);

            return obj;
        }

        protected virtual void LoadObjectsInRelationshipGetById(T obj)
        {
        }

        protected virtual void ReadOnOffObject(T obj)
        {
        }

        AOrmObject ITableORM.GetObjectById(object objectId)
        {
            return GetObjectById(objectId);
        }

        public virtual object ParseId(string strObjectId)
        {
            return new Guid(strObjectId);
        }

        public T GetObjectById(object id)
        {
            var obj = GetById<T>(id);

            if (obj == null)
                return null;

            LoadObjectsInRelationship(obj);
            LoadObjectsInRelationshipGetById(obj);

            ReadOnOffObject(obj);

            return obj;
        }

        protected virtual void LoadObjectsInRelationship(T obj)
        {
        }

        public T GetObjectForEdit(object id)
        {
            var ormObject = GetById(id);

            if (ormObject == null)
                return null;

            CentralTransactionControl.Singleton.EditStart(
                new IdAndObjectType(
                    id, 
                    ormObject.GetObjectType()));

            return ormObject;
        }

        public T GetObjectForEdit(object id, out Exception error)
        {
            error = null;

            if (!HasAccessUpdate())
            {
                error = new AccessDeniedException();
                return null;
            }

            var obj = GetObjectForEdit(id);

            if (obj != null)
            {
                if (!StructuredSubSites.Singleton.HasAccessUpdate(obj))
                {
                    error = new AccessDeniedException();
                    return null;
                }

                LoadObjectsInRelationship(obj);
                ReadOnOffObject(obj);
            }

            return obj;
        }

        public void RenewObjectForEdit(object id, out Exception error)
        {
            error = null;

            if (!HasAccessUpdate())
            {
                error = new AccessDeniedException();
                return;
            }

            if (!CentralTransactionControl.Singleton.
                RenewEditingObject(
                new IdAndObjectType(
                    id,
                    ObjectType)))
            {
                error = new AccessDeniedException();
            }
        }

        public T GetObjectForEditById(object id, out bool editAllowed)
        {
            editAllowed = false;
            Exception error;

            var objEdit = GetObjectForEdit(id, out error);

            if (objEdit != null)
            {
                editAllowed = true;
                return objEdit;
            }

            return
                error is AccessDeniedException
                    ? GetObjectById(id)
                    : null;
        }

        public IEnumerable<AOrmObject> GetDirectReferences(object idObj)
        {
            var obj = GetById(idObj);

            if (obj == null)
                return null;

            var directReferencesFromTable = 
                GetDirectReferencesInternal(obj)
                    ?? Enumerable.Empty<AOrmObject>();

            var directReferencesFromPlugins = 
                CgpServer.Singleton.PluginManager
                    .Select(pluginDescriptor => pluginDescriptor._plugin.GetDirectReferences(obj))
                    .Where(directReferences => directReferences != null);

            return
                directReferencesFromTable
                    .Concat(directReferencesFromPlugins.SelectMany(directReferences => (directReferences)))
                    .Where(directReference => directReference != null);
        }

        protected virtual IEnumerable<AOrmObject> GetDirectReferencesInternal(T obj)
        {
            return null;
        }

        public ICollection<T> List()
        {
            return base.List<T>();
        }

        public ICollection<T> List(out Exception error)
        {
            return List(AccessChecker.GetActualLogin(), out error);
        }

        public ICollection<T> List(Login login, out Exception error)
        {
            if (login == null ||
                !HasAccessGetList())
            {
                error = new AccessDeniedException();
                return null;
            }

            error = null;
            return StructuredSubSites.Singleton.GetObjectsForLogin(
                login,
                List<T>());
        }

        public ICollection<T> SelectByCriteria(IList<FilterSettings> filterSettings)
        {
            return SelectByCriteria(LogicalOperators.AND, filterSettings);
        }

        public ICollection<T> SelectByCriteria(
            LogicalOperators filterJoinOperator,
            params ICollection<FilterSettings>[] filterSettings)
        {
            var criteria = PrepareCriteria<T>();

            if (filterSettings != null)
            {
                int usedFilterCount = 0;

                // Create joining junction
                Junction filterJunction =
                    filterJoinOperator == LogicalOperators.OR
                        ? (Junction)Restrictions.Disjunction()
                        : Restrictions.Conjunction();

                // Add all filters to joining junction
                foreach (var filter in filterSettings)
                {
                    if (filter == null || filter.Count == 0)
                        continue;

                    Junction junction = null;
                    // Add all items in filter to current junction
                    foreach (var filterSetting in filter)
                    {
                        if (AddCriteriaSpecial(
                            ref criteria,
                            filterSetting))
                        {
                            continue;
                        } 

                        // TODO Extend filter to check all logical operators and change current junction dynamically
                        if (junction == null)
                            junction =
                                filterSetting.LogicalOperator == LogicalOperators.OR
                                    ? (Junction)Restrictions.Disjunction()
                                    : Restrictions.Conjunction();

                        junction =
                            FilterSettingsToCriteria.AddJunction(
                                junction,
                                filterSetting);
                    }

                    if (junction != null)
                    {
                        filterJunction.Add(junction);
                        usedFilterCount++;
                    }
                }

                if (filterJunction != null && usedFilterCount > 0)
                    criteria.Add(filterJunction);
            }

            AddOrder(ref criteria);
            return base.Select<T>(criteria);
        }

        public virtual ICollection<T> SelectRangeByCriteria(
            IList<FilterSettings> filterSettings,
            int firstResult,
            int maxResults)
        {
            return
                SelectRangeByCriteria(
                    filterSettings,
                    firstResult,
                    maxResults,
                    string.Empty);
        }

        public ICollection<T> SelectRangeByCriteria(
            IList<FilterSettings> filterSettings,
            int firstResult,
            int maxResults,
            string alias)
        {
            var criteria =
                !string.IsNullOrEmpty(alias)
                    ? PrepareCriteria<T>(alias)
                    : PrepareCriteria<T>();

            if (filterSettings != null)
                foreach (var filterSetting in filterSettings)
                    if (!AddCriteriaSpecial(
                            ref criteria,
                            filterSetting))
                        criteria =
                            FilterSettingsToCriteria.AddCriteria(
                                criteria,
                                filterSetting);

            criteria.SetFirstResult(firstResult);
            criteria.SetMaxResults(maxResults);

            AddOrder(ref criteria);
            return base.Select<T>(criteria);
        }

        public int SelectCount(out Exception error)
        {
            return SelectCount(null, out error);
        }

        public virtual int SelectCount(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            return SelectCount(filterSettings, out error, string.Empty);
        }

        public int SelectCount(
            IList<FilterSettings> filterSettings,
            out Exception error,
            string alias)
        {
            error = null;

            var criteria =
                !string.IsNullOrEmpty(alias)
                    ? PrepareCriteria<T>(alias)
                    : PrepareCriteria<T>();

            SetProjection(criteria);

            if (filterSettings != null)
                foreach (var filterSetting in filterSettings)
                    if (!AddCriteriaSpecial(
                            ref criteria,
                            filterSetting))
                        criteria =
                            FilterSettingsToCriteria.AddCriteria(
                                criteria,
                                filterSetting);

            var result = criteria.UniqueResult();
            return result != null ? (int)result : -1;
        }

        public ICollection<T> SelectByCriteria(ICollection<FilterSettings> filterSettings)
        {
            return SelectByCriteria(LogicalOperators.AND, filterSettings);
        }

        public ICollection<T> SelectByCriteria(ICollection<FilterSettings> filterSettings, out Exception error)
        {
            return SelectByCriteria(out error, LogicalOperators.AND, filterSettings);
        }

        public ICollection<T> SelectByCriteria(out Exception error, LogicalOperators filterJoinOperator, params ICollection<FilterSettings>[] filterSettings)
        {
            // SB
            error = null;

            try
            {
                var login = AccessChecker.GetActualLogin();

                if (login != null && !HasAccessGetList())
                {
                    error = new AccessDeniedException();
                    return null;
                }

                var objectsFromDatabase = StructuredSubSites.Singleton.GetObjectsForLogin(login, SelectByCriteria(filterJoinOperator, filterSettings));

                if (objectsFromDatabase == null)
                {
                    return null;
                }

                foreach (var objectFromDatabase in objectsFromDatabase)
                {
                    ReadOnOffObject(objectFromDatabase);
                }

                return objectsFromDatabase;
            }
            catch (Exception ex)
            {
                error = ex;
                return null;
            }
        }

        protected virtual bool AddCriteriaSpecial(
            ref ICriteria c, 
            FilterSettings filterSetting)
        {
            return false;
        }

        protected virtual void AddOrder(ref ICriteria c)
        {
        }

        #endregion

        public void EditEnd(T ormObject)
        {
            CentralTransactionControl.Singleton.EditEnd(
                new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType()));

            EditEndForCollection(ormObject);
        }

        public virtual void EditEndForCollection(T ormObject)
        {
        }

        public virtual bool HasAccessGetList()
        {
            return true;
        }

        public bool HasAccessView()
        {
            return HasAccessView(AccessChecker.GetActualLogin());
        }
        
        public abstract bool HasAccessView(Login login);

        public bool HasAccessViewForObject(T ormObject)
        {
            return HasAccessViewForObject(ormObject, AccessChecker.GetActualLogin());
        }

        public bool HasAccessViewForObject(AOrmObject ormObject, Login login)
        {
            return HasAccessView(login) &&
                   StructuredSubSites.Singleton.HasAccessView(ormObject, login);
        }

        public bool HasAccessInsert()
        {
            return HasAccessInsert(AccessChecker.GetActualLogin());
        }
        
        public abstract bool HasAccessInsert(Login login);

        public bool HasAccessUpdate()
        {
            return HasAccessUpdate(AccessChecker.GetActualLogin());
        }
        
        public abstract bool HasAccessUpdate(Login login);

        public bool HasAccessDelete()
        {
            return HasAccessDelete(AccessChecker.GetActualLogin());
        }
        
        public abstract bool HasAccessDelete(Login login);

        public IList<AOrmObject> GetReferencedObjectsAllPlugins(object idObj)
        {
            return GetReferencedObjects(
                idObj,
                true,
                null);
        }

        IEnumerable<AOrmObject> ITableORM.List()
        {
            return List<T>().Cast<AOrmObject>();
        }

        public IList<AOrmObject> GetReferencedObjects(object idObj, List<string> plugins)
        {
            return GetReferencedObjects(idObj, false, plugins);
        }

        private IList<AOrmObject> GetReferencedObjects(object idObj, bool allPlugins, List<string> plugins)
        {
            IList<AOrmObject> ormObjects = new List<AOrmObject>();

            var referencedObjects = GetReferencedObjectsInternal(idObj);

            if (referencedObjects != null && referencedObjects.Count > 0)
                foreach (var ormObject in referencedObjects)
                    ormObjects.Add(ormObject);

            var objRef = GetById(idObj);

            if (objRef != null && CgpServer.Singleton.PluginManager != null)
                foreach (var plugin in CgpServer.Singleton.PluginManager)
                    if (allPlugins || plugins.Contains(plugin._plugin.FriendlyName))
                    {
                        var pluginReferencedObjects =
                            plugin._plugin.GetReferencedByObject(objRef);

                        if (pluginReferencedObjects == null)
                            continue;

                        foreach (var ormObject in pluginReferencedObjects)
                            ormObjects.Add(ormObject);
                    }

            return
                ormObjects.Count > 0
                    ? ormObjects
                        .OrderBy(orm => orm.ToString())
                        .ToList()
                    : null;
        }

        protected virtual IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            return null;
        }

        protected virtual bool AddObjectNotInStructuredSubSite(T ormObject, bool getExistObjects)
        {
            return true;
        }

        public virtual AOrmObject GetObjectParent(AOrmObject ormObject)
        {
            return null;
        }

        public virtual string GetPluginName()
        {
            return string.Empty;
        }

        public virtual bool CanCreateObject()
        {
            return true;
        }

        public abstract ObjectType ObjectType { get; }

        #region SearchFunctions

        public virtual ICollection<AOrmObject> GetSearchList(string name,
            bool single)
        {
            return null;
        }

        public virtual ICollection<AOrmObject> FulltextSearch(string name)
        {
            return null;
        }
        
        public virtual ICollection<AOrmObject> ParametricSearch(string name)
        {
            return null;
        }

        public virtual bool ImplementsSearchFunctions()
        {
            return false;
        }

        #endregion

        #region ITableORM Members 

        public IEnumerable<IModifyObject> GetIModifyObjects()
        {
            Exception error;
            var objects = List(out error);
            if (objects == null)
                return Enumerable.Empty<IModifyObject>();

            return
                objects
                    .Select(ormObject => CreateModifyObject(ormObject))
                    .Where(ormObjectShort => ormObjectShort != null)
                    .OrderBy(ormObjectShort => ormObjectShort.ToString());
        }

        protected virtual IModifyObject CreateModifyObject(T ormbObject)
        {
            return null;
        }

        #endregion
    }

    public enum ObjectDatabaseAction : byte
    {
        Insert = 0,
        Update = 1,
        Delete = 2
    }

    public interface IBaserOrmTableWithAlarmInstruction
    {
        bool HasLocalAlarmInstruction(object id);
    }

    public abstract class ABaserOrmTableWithAlarmInstruction<TSingleton, T> :
        ABaseOrmTable<TSingleton, T>,
        IBaserOrmTableWithAlarmInstruction
        where TSingleton : ATableORM<TSingleton>
        where T : AOrmObject, IOrmObjectWithAlarmInstructions
    {
        private readonly HashSet<object> _objectIdsWithLocalAlarmInstruction = new HashSet<object>();
        private bool _objectIdsWithLocalAlarmInstructionWasInitialized;

        protected ABaserOrmTableWithAlarmInstruction(TSingleton singleton)
            : base(singleton)
        {
        }

        protected ABaserOrmTableWithAlarmInstruction(TSingleton singleton, ICudPreparation<T> beforeIUD)
            : base(singleton, beforeIUD)
        {
        }

        public bool HasLocalAlarmInstruction(object id)
        {
            lock (_objectIdsWithLocalAlarmInstruction)
            {
                if (!_objectIdsWithLocalAlarmInstructionWasInitialized)
                    InitializeObjectsWithLocalAlarmInstruction();

                return _objectIdsWithLocalAlarmInstruction.Contains(id);
            }
        }

        private void InitializeObjectsWithLocalAlarmInstruction()
        {
            lock (_objectIdsWithLocalAlarmInstruction)
            {
                var objectsWithLocalAlarmInstruction = GetObjectsWithLocalAlarmInstruction();

                if (objectsWithLocalAlarmInstruction != null)
                {
                    foreach (var objectWithLocalAlarmInstruction in objectsWithLocalAlarmInstruction)
                    {
                        _objectIdsWithLocalAlarmInstruction.Add(objectWithLocalAlarmInstruction.GetId());
                    }
                }

                _objectIdsWithLocalAlarmInstructionWasInitialized = true;
            }
        }

        protected abstract IEnumerable<T> GetObjectsWithLocalAlarmInstruction();

        public override void AfterInsert(T newObject)
        {
            lock (_objectIdsWithLocalAlarmInstruction)
                if (_objectIdsWithLocalAlarmInstructionWasInitialized)
                    if (!string.IsNullOrEmpty(newObject.GetLocalAlarmInstruction()))
                        _objectIdsWithLocalAlarmInstruction.Add(newObject.GetId());
        }

        public override void AfterUpdate(T newObject, T oldObjectBeforUpdate)
        {
            lock (_objectIdsWithLocalAlarmInstruction)
                if (_objectIdsWithLocalAlarmInstructionWasInitialized)
                    if (!string.IsNullOrEmpty(newObject.GetLocalAlarmInstruction()))
                        _objectIdsWithLocalAlarmInstruction.Add(newObject.GetId());
        }

        public override void AfterDelete(T deletedObject)
        {
            lock (_objectIdsWithLocalAlarmInstruction)
                if (_objectIdsWithLocalAlarmInstructionWasInitialized)
                    _objectIdsWithLocalAlarmInstruction.Add(deletedObject.GetId());
        }
    }

    public interface ICudSpecialExecutor<T>
    {
        void CUDSpecial(T ormObject, ObjectDatabaseAction objectDatabaseAction);
    }

    public sealed class CudSpecialQueue : ASingleton<CudSpecialQueue>
    {
        private abstract class CudSpecialBaseRequest
        {
            public abstract void CudSpecial();
        }

        private class CudSpecialRequest<T> : CudSpecialBaseRequest
        {
            private readonly T _ormObject;
            private readonly ObjectDatabaseAction _objectDatabaseAction;
            private readonly ICudSpecialExecutor<T> _cudSpecialExecutor;

            public CudSpecialRequest(
                T ormObject,
                ObjectDatabaseAction objectDatabaseAction,
                ICudSpecialExecutor<T> cudSpecialExecutor)
            {
                _ormObject = ormObject;
                _objectDatabaseAction = objectDatabaseAction;
                _cudSpecialExecutor = cudSpecialExecutor;
            }

            public override void CudSpecial()
            {
                _cudSpecialExecutor.CUDSpecial(
                    _ormObject,
                    _objectDatabaseAction);
            }
        }

        private readonly ProcessingQueue<CudSpecialBaseRequest> _cudSpecialProcessingQueue;

        private CudSpecialQueue() :
            base(null)
        {
            _cudSpecialProcessingQueue = new ProcessingQueue<CudSpecialBaseRequest>();
            _cudSpecialProcessingQueue.ItemProcessing += _cudSpecialProcessingQueue_ItemProcessing;
        }

        void _cudSpecialProcessingQueue_ItemProcessing(CudSpecialBaseRequest cudSpecialBaseRequest)
        {
            cudSpecialBaseRequest.CudSpecial();
        }

        public void Enqueue<T>(
            T ormObject,
            ObjectDatabaseAction objectDatabaseAction,
            ICudSpecialExecutor<T> cudSpecialExecutor)
        {
            _cudSpecialProcessingQueue.Enqueue(
                new CudSpecialRequest<T>(
                    ormObject,
                    objectDatabaseAction,
                    cudSpecialExecutor));
        }
    }
}
