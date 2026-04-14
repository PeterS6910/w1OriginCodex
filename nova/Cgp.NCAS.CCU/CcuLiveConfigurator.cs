using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU
{
    public abstract class CcuLiveConfigurator<T> where T : class
    {
        public ObjectType DbObjectType { get; private set; }
        public Guid DbObjectId { get; private set; }

        private WeakReference _dbObjWeakReference;
        private volatile T _dbObjHardReference;

        public T DbObject
        {
            get
            {
                if (_dbObjHardReference != null)
                    return _dbObjHardReference;

                if (_dbObjWeakReference == null)
                {
                    _dbObjWeakReference = new WeakReference(null);
                }

                object weakObject = _dbObjWeakReference.Target;

                if (weakObject == null)
                {
                    weakObject = 
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            DbObjectType,
                            DbObjectId);

                    _dbObjWeakReference.Target = weakObject;    
                }

                return (T)weakObject;
            }
        }

        protected CcuLiveConfigurator(ObjectType objectType, Guid objectId)
        {
            DbObjectType = objectType;
            DbObjectId = objectId;
        }

        public void ApplyChanges()
        {
            _dbObjWeakReference = null;
            _dbObjHardReference = null;
            OnAfterApply();
        }

        public void PrepareChanges()
        {
            _dbObjHardReference = DbObject;
            OnAfterPrepare();
        }

        public void RevertChanges()
        {
            _dbObjHardReference = null;
            OnAfterRevert();
        }

        protected virtual void OnAfterApply()
        {

        }

        protected virtual void OnAfterPrepare()
        {

        }

        protected virtual void OnAfterRevert()
        {

        }
    }
}