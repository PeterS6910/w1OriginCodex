using System.Collections.Generic;
using System.Linq;

namespace Contal.IwQuick.CrossPlatform.Data
{
    public abstract class AObjectPoolWithSizeLimit<TObject>
        : ADisposable
    {
        private int _sizeLimit;

        private readonly LinkedList<TObject> _pooledObjects;
        private volatile object _lockObject = new object();

        public int SizeLimit
        {
            get
            {
                return _sizeLimit;
            }
            set
            {
                _sizeLimit = value;

                lock (_lockObject)
                    while (_pooledObjects.Count > _sizeLimit)
                    {
                        var objToDispose = _pooledObjects.First();
                        _pooledObjects.RemoveFirst();

                        DisposeObject(objToDispose);
                    }
            }
        }

        protected AObjectPoolWithSizeLimit(int sizeLimit)
        {
            _pooledObjects = new LinkedList<TObject>();
            _sizeLimit = sizeLimit;
        }

        public TObject Get()
        {
            lock (_lockObject)
            {
                if (_pooledObjects.Count > 0)
                {
                    var obj = _pooledObjects.First();
                    _pooledObjects.RemoveFirst();

                    return obj;
                }
            }

            return CreateObject();
        }

        protected abstract TObject CreateObject();

        public bool Return(TObject obj)
        {
            lock (_lockObject)
            {
                if (AimedToBeDisposed || _pooledObjects.Count >= _sizeLimit) 
                    return false;

                BeforeReturnObject(obj);
                _pooledObjects.AddLast(obj);

                return true;
            }
        }

        protected abstract void DisposeObject(TObject obj);
        protected abstract void BeforeReturnObject(TObject obj);

        protected override void InternalDispose(bool isExplicitDispose)
        {
            foreach (var pooledObject in _pooledObjects)
                DisposeObject(pooledObject);

            _pooledObjects.Clear();
        }
    }
}
