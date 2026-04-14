using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    internal abstract class AStateAndSettingsObjectCollection<
            TStateAndSettingsObjectCollection, 
            TStateAndSettingsObject,
            TDbObject>
        : ASingleton<TStateAndSettingsObjectCollection>
        , IDbObjectChangeListener<TDbObject>
        where TStateAndSettingsObjectCollection : 
            AStateAndSettingsObjectCollection<
                TStateAndSettingsObjectCollection, 
                TStateAndSettingsObject,
                TDbObject>
        where TStateAndSettingsObject : AStateAndSettingsObjectBase<TDbObject>
        where TDbObject : class, IDbObject
    {
        protected readonly SyncDictionary<Guid, TStateAndSettingsObject> _objects = 
            new SyncDictionary<Guid, TStateAndSettingsObject>();

        protected AStateAndSettingsObjectCollection(ISingleton singletonReserved)
            : base(singletonReserved)
        {
        }

        public bool ObjectWithIdExists(Guid idObject)
        {
            return
                _objects.ContainsKey(idObject);
        }

        public void Create(IEnumerable<Guid> idObjects)
        {
            foreach (var idObject in idObjects)
            {
                var dbObject = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType,
                    idObject) as TDbObject;

                if (dbObject == null)
                    continue;
                
                _objects.GetOrAddValue(
                    idObject,
                    key => CreateNewStateAndSettingsObject(dbObject),
                    null);

                PrepareConfigure(dbObject);
            }
        }

        protected virtual void PrepareConfigure(TDbObject dbObject)
        {
        }

        public void Configure(IEnumerable<Guid> idObjects)
        {
            foreach (var idObject in idObjects)
            {
                var dbObject = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType,
                    idObject) as TDbObject;

                if (dbObject == null)
                    continue;

                TStateAndSettingsObject stateAndSettingsObject = null;

                _objects.TryGetValue(
                    idObject,
                    (key, found, value) =>
                    {
                        if (found)
                        {
                            stateAndSettingsObject = value;
                            value.ConfigureStart(dbObject);
                        }
                    });

                if (stateAndSettingsObject != null)
                    stateAndSettingsObject.FinishConfiguration();
            }
        }

        public void ApplyHwSetupOnDcuOnline(Guid idObject)
        {
            var dbObject = 
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType,
                    idObject) as TDbObject;

            if (dbObject == null)
                return;

            TStateAndSettingsObject stateAndSettingsObject = null;

            _objects.TryGetValue(
                idObject,
                (key, found, value) =>
                {
                    if (!found)
                        return;

                    stateAndSettingsObject = value;

                    stateAndSettingsObject.ApplyHwSetupOnDcuOnlineStart(dbObject);
                });

            if (stateAndSettingsObject != null)
                stateAndSettingsObject.FinishApplyHwSetupOnDcuOnline();
        }

        private void Unconfigure(
            Guid id,
            TDbObject newObject)
        {
            TStateAndSettingsObject stateAndSettingsObject = null;
            bool wasRemoved = false;

            _objects.Remove(
                id,
                (Guid key,
                    TStateAndSettingsObject value,
                    out bool continueInRemove) =>
                {
                    value.UnconfigureStart(newObject);
                    stateAndSettingsObject = value;

                    continueInRemove = newObject == null;
                },
                (key, removed, value) =>
                {
                    if (removed)
                    {
                        OnRemoved(value);
                        wasRemoved = true;
                    }
                });

            if (stateAndSettingsObject != null)
                stateAndSettingsObject.FinishUnconfiguration(wasRemoved);
        }

        protected virtual void OnRemoved(TStateAndSettingsObject removedValue)
        {
            
        }

        public abstract ObjectType ObjectType
        {
            get;
        }

        protected abstract TStateAndSettingsObject CreateNewStateAndSettingsObject(TDbObject dbObject);

        public void PrepareObjectUpdate(
            Guid idObject,
            TDbObject newObject)
        {
            Unconfigure(
                idObject,
                newObject);
        }

        public void OnObjectSaved(
            Guid id,
            TDbObject newObject)
        {
            TStateAndSettingsObject stateAndSettingsObject = null;

            _objects.TryGetValue(
                id,
                (key, found, value) =>
                {
                    if (!found)
                        return;

                    stateAndSettingsObject = value;

                    stateAndSettingsObject.StartOnObjectSaved(newObject);
                });

            if (stateAndSettingsObject != null)
                stateAndSettingsObject.FinishOnObjectSaved();
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            Unconfigure(
                idObject, 
                null);
        }
    }
}
