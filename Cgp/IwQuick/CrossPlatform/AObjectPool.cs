using System;
using System.Collections.Generic;
using System.Linq;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public interface IObjectPool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        Type GetPooledType();

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AObjectPool : ADisposable, IObjectPool 
    {
        public abstract Type GetPooledType();
        public abstract void Clear();
        public abstract int Count { get; }

        private static readonly HashSet<WeakReference> _pools = new HashSet<WeakReference>();
        private static readonly object _syncRoot = new object();
        
        protected static void RegisterOrNot(AObjectPool objectPool,bool toRegister)
        {
            if (ReferenceEquals(objectPool,null))
                return;

            if (toRegister)
            {
                var weakRef = new WeakReference(objectPool);

                lock(_syncRoot)
                    _pools.Add(weakRef);

            }
            else
            {
                lock (_syncRoot)
                    _pools.RemoveWhere(weakReference =>
                    {
                        // some cleanup
                        if (ReferenceEquals(weakReference.Target,null))
                            return true;

                        return ReferenceEquals(objectPool, weakReference.Target);
                    });

            }
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            RegisterOrNot(this,false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AObjectPool> MakeReport()
        {
            lock (_syncRoot)
            {
                return _pools
                    .Where(weakReference => !ReferenceEquals(weakReference.Target,null))
                    .Select(reference => (AObjectPool)reference.Target);
            }
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class AObjectPool<TObject> : AObjectPool
        
        // this condition has been avoided as there are numerous types
        // that can be pooled without necessity to be IDisposable
        //where TObject : IDisposable 
    {
        private readonly LinkedList<TObject> _freeObjects =
            new LinkedList<TObject>();

        protected AObjectPool()
        {
            RegisterOrNot(this,true);
        }


        /// <summary>
        /// necessary to implement for instantiation of the objects on Get
        /// </summary>
        /// <returns></returns>
        protected abstract TObject CreateObject();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="foundObject"></param>
        /// <returns></returns>
        private bool FindFirst(out TObject foundObject)
        {
            lock (_freeObjects)
                if (_freeObjects.Count > 0)
                {
                    TObject result = _freeObjects.First.Value;
                    _freeObjects.RemoveFirst();

                    ValidateAverageCount();
                    
                    foundObject = result;
                    return true;
                    
                }

            foundObject = default(TObject);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newlyAdded"></param>
        /// <returns></returns>
        protected virtual TObject Get(out bool newlyAdded)
        {
            TObject obj;
            if (FindFirst(out obj))
            {
                newlyAdded = false;
                return obj;
            }

            // can throw any exception
            obj = CreateObject();

            // at last
            newlyAdded = true;
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual TObject Get()
        {
            TObject obj;
            if (FindFirst(out obj))
            {
                return obj;
            }

            // can throw any exception
            return CreateObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnedObject"></param>.
        /// <exception cref="ArgumentNullException">if returned instance is null</exception>
        public virtual void Return(TObject returnedObject)
        {
            if (ReferenceEquals(returnedObject, null))
                throw new ArgumentNullException(
                    "returnedObject",
                    string.Concat("Cannot return null instance of ",typeof(TObject)," type to object pool"));

            int freeObjectsCountSnapshot;
            double averagefreeObjectsCount;
            int maxFreeObjectsCount;

            lock (_freeObjects)
            {
                _freeObjects.AddLast(returnedObject); // actual returning of the instance

                freeObjectsCountSnapshot = _freeObjects.Count;

                averagefreeObjectsCount = ValidateAverageCount();

                maxFreeObjectsCount = _maxCount;
            }

            int reduceToCount = 
                ValidateReducingCondition(
                freeObjectsCountSnapshot,
                averagefreeObjectsCount,
                maxFreeObjectsCount
                );
            
            if (reduceToCount >= 0)
            {
                TObject[] removedObjects;
                int removedCount = 0;

                lock (_freeObjects)
                {
                    if (reduceToCount >= _freeObjects.Count)
                        return;

                    removedObjects = new TObject[_freeObjects.Count]; // leave filled with nulls

                    while (_freeObjects.Count > reduceToCount)
                    {
                        removedObjects[removedCount++] = _freeObjects.First.Value;
                        _freeObjects.RemoveFirst();
                    }
                }

                DisposeObjectsSnapshot(removedObjects);
            }
        }

        //private readonly object _statisticLock = new object();

        private ulong _operationCount = 0;

        private ulong _snapshotCountSum = 0;

        private int _maxCount = 0;

        private double ValidateAverageCount()
        {
            //lock (_statisticLock) // lock not needed, always called within lock(_freeObjects)
            {
                _operationCount++;

                int actualCount = _freeObjects.Count;
                
                _snapshotCountSum += (ulong) actualCount;

                if (actualCount > _maxCount)
                    _maxCount = actualCount;

                return _snapshotCountSum/(double) _operationCount;
            }
        }

        /// <summary>
        /// validates, whether there should be a reduce of the free objects ; 
        /// the return value marks the suggested count of free objects, or no reducing if -1
        /// </summary>
        /// <param name="freeObjectsCountSnapshot"></param>
        /// <param name="averageFreeObjectsCount"></param>
        /// <param name="maxFreeObjectsCount"></param>
        /// <returns></returns>
        protected virtual int ValidateReducingCondition(int freeObjectsCountSnapshot,double averageFreeObjectsCount,int maxFreeObjectsCount)
        {
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectsSnapshot"></param>
        private static void DisposeObjectsSnapshot(IEnumerable<TObject> objectsSnapshot)
        {
            // picked out from the lock, because the Dispose operation
            // can take long
            foreach (TObject freeObject in objectsSnapshot)
                try
                {
                    if (!ReferenceEquals(freeObject, null))
                    {
                        var disposable = freeObject as IDisposable;
                        if (null != disposable)
                            disposable.Dispose();
                    }
                }
                catch
                {

                }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            TObject[] objectsSnapshot;

            lock (_freeObjects)
            {
                objectsSnapshot = new TObject[_freeObjects.Count];
                _freeObjects.CopyTo(objectsSnapshot,0);

                _freeObjects.Clear();

                ValidateAverageCount();
            }

            DisposeObjectsSnapshot(objectsSnapshot);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InternalDispose(bool isExplicitDispose)
        {
            Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Count
        {
            get
            {
                lock (_freeObjects)
                    return _freeObjects.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Type GetPooledType()
        {
            return typeof(TObject);
        }

    }
}
