using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;


namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventHandler"></typeparam>
    public interface IEventHandlerGroup<TEventHandler> // avoid deriving from IEnumerable because Forach methods using the poolable snapshots
        where TEventHandler : class
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        bool IsNotEmpty { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void Add([NotNull] TEventHandler eventHandler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        bool Remove([NotNull] TEventHandler eventHandler);

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        void Register([NotNull] TEventHandler eventHandler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        void Unregister([NotNull] TEventHandler eventHandler);

        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventHandler"></typeparam>
    public abstract class AEventHandlerGroup<TEventHandler> : ADisposable,IEventHandlerGroup<TEventHandler>
        where TEventHandler : class
        
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public delegate void DProcessEventHandler([NotNull] TEventHandler eventHandler);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <param name="parameters"></param>
        public delegate void DProcessEventHandlerWithParameters([NotNull] TEventHandler eventHandler, params object[] parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambdaOverEventHandler"></param>
        /// <param name="stopOnException"></param>
        /// <param name="removeOnException"></param>
        public void ForEach(
            [NotNull] DProcessEventHandler lambdaOverEventHandler,
            bool stopOnException,
            bool removeOnException)
        {
            Validator.CheckForNull(lambdaOverEventHandler, "lambdaOverEventHandler");

            if (IsEmpty)
                return;

            var handlersSnapshot = GetEventHandlersSnapshot();

            if (null != handlersSnapshot)
            {
// ReSharper disable once PossibleMultipleEnumeration
                foreach (var eventHandler in handlersSnapshot)
                {
                    if (ReferenceEquals(eventHandler,null))
                        continue;

                    try
                    {
                        lambdaOverEventHandler(eventHandler);
                    }
                    catch (Exception err)
                    {
                        if (removeOnException)
                            Remove(eventHandler);

                        if (stopOnException)
                            return;

                        HandledExceptionAdapter.Examine(err);
                    }
                }

// ReSharper disable once PossibleMultipleEnumeration
                ReturnEventHandlersSnapshot(handlersSnapshot);
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerProcessingDelegate"></param>
        /// <param name="stopOnException"></param>
        /// <param name="removeOnException"></param>
        /// <param name="parameters"></param>
        public void ForEach(
            [NotNull] DProcessEventHandlerWithParameters eventHandlerProcessingDelegate,
            bool stopOnException,
            bool removeOnException,
            params object[] parameters)
        {
            Validator.CheckForNull(eventHandlerProcessingDelegate, "eventHandlerProcessingDelegate");

            if (IsEmpty)
                return;

            var handlersSnapshot = GetEventHandlersSnapshot();

            if (null != handlersSnapshot)
            {
// ReSharper disable once PossibleMultipleEnumeration
                foreach (var eventHandler in handlersSnapshot)
                {
                    if (ReferenceEquals(eventHandler, null))
                        continue;

                    try
                    {
                        eventHandlerProcessingDelegate(eventHandler, parameters);
                    }
                    catch (Exception err)
                    {
                        if (removeOnException)
                            Remove(eventHandler);

                        if (stopOnException)
                            return;

                        HandledExceptionAdapter.Examine(err);
                    }
                }

// ReSharper disable once PossibleMultipleEnumeration
                ReturnEventHandlersSnapshot(handlersSnapshot);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerProcessingDelegate"></param>
        /// <param name="parameters"></param>
        public void ForEach(
            [NotNull] DProcessEventHandlerWithParameters eventHandlerProcessingDelegate,
            params object[] parameters)
        {
            if (IsEmpty)
                return;

            ForEach(eventHandlerProcessingDelegate,false,false,parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambdaOverEventHandler"></param>
        public void ForEach([NotNull] DProcessEventHandler lambdaOverEventHandler)
        {
            if (IsEmpty)
                return;

            ForEach(lambdaOverEventHandler, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<TEventHandler> GetEventHandlersSnapshot();

        /// <summary>
        /// reserved for pooling approach
        /// </summary>
        /// <param name="reusableSnapshot"></param>
        protected virtual void ReturnEventHandlersSnapshot(IEnumerable<TEventHandler> reusableSnapshot)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// override if necessary
        /// </summary>
        public virtual bool IsNotEmpty { get { return !IsEmpty; } }

        public abstract void Add(TEventHandler eventHandler);
        public abstract bool Remove(TEventHandler eventHandler);
        public abstract void Clear();
        public abstract int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public void Register(TEventHandler eventHandler)
        {
            Add(eventHandler);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public void Unregister(TEventHandler eventHandler)
        {
            Remove(eventHandler);
        }

        protected class SnapshotPool<TEh> : AObjectPool<TEh[]>
        {
            private int _defaultArraySize = 4;

            /// only growing size
            public int DefaultArraySize
            {
                get { return _defaultArraySize; }
                set
                {
                    if (value > _defaultArraySize)
                        _defaultArraySize = value;
                }
            }

            protected override TEh[] CreateObject()
            {
                return new TEh[_defaultArraySize];
            }

            protected override TEh[] Get(out bool newlyAdded)
            {
                var potentialReturn= base.Get(out newlyAdded);

                if (!newlyAdded)
                {
                    if (potentialReturn == null ||
                        potentialReturn.Length < _defaultArraySize)
                    {
                        // do not return the potentialReturn buffer in this case, cause it wouldn't do any good in future in its former size

                        return new TEh[_defaultArraySize];
                    }
                }

                return potentialReturn;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override TEh[] Get()
            {
                bool newlyAdded;
                return Get(out newlyAdded);
            }

            public static readonly SnapshotPool<TEh> Singleton = new SnapshotPool<TEh>();

            public override void Return(TEh[] returnedObject)
            {
                if (returnedObject == null)
                    // silent
                    return;

                Array.Clear(returnedObject, 0, returnedObject.Length);

                base.Return(returnedObject);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventHandler">class or interface representing event handler</typeparam>
    public sealed class EventHandlerGroup<TEventHandler> : AEventHandlerGroup<TEventHandler>
        where TEventHandler : class
    {
        // just as optimisation not to read _handlers.Count all the time
        private volatile bool _isEmpty = true;

        // used as data source as well as sync object
        private volatile SyncDictionary<TEventHandler, LinkedListNode<TEventHandler>> _handlers = null;
        private LinkedList<TEventHandler> _handlersSorted = null;


        private readonly object _syncHandlers = new object();

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmpty
        {
            get { return _isEmpty; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void Add(TEventHandler eventHandler)
        {
            Validator.CheckForNull(eventHandler,"eventHandler");

            // on demand initialization
            if (ReferenceEquals(null,_handlers))
                lock (_syncHandlers)
                {
                    if (ReferenceEquals(null, _handlersSorted))
                    {
                        _handlersSorted = new LinkedList<TEventHandler>();
                    }

                    // should be last in the sequence
                    if (ReferenceEquals(null, _handlers))
                    {
                        _handlers = new SyncDictionary<TEventHandler, LinkedListNode<TEventHandler>>();
                    }
                }

            _handlers.GetOrAddValue(eventHandler,
                key =>
                {
                    var ret= _handlersSorted.AddLast(eventHandler);
                    _isEmpty = false;
                    return ret;
                },
                null);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override bool Remove(TEventHandler eventHandler)
        {
            Validator.CheckForNull(eventHandler,"eventHandler");

            if (ReferenceEquals(_handlers,null))
                return false;

            return
                _handlers.Remove(
                    eventHandler,
                    (key, removed, removedValue) =>
                    {
                        if (removed)
                        {
                            _handlersSorted.Remove(removedValue);
                        }

                        if (_handlers.Count == 0)
                            _isEmpty = true;
                    }
                    );
        }

        
        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            if (ReferenceEquals(_handlers, null))
                return;

            _handlers.Clear(() =>
            {
                _handlersSorted.Clear();
                _isEmpty = true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerGroup">if null, and used as += operator, the eventHandlerGroup would be created</param>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static EventHandlerGroup<TEventHandler> operator +(
            [CanBeNull] EventHandlerGroup<TEventHandler> eventHandlerGroup, 
            [NotNull] TEventHandler eventHandler)
        {
            if (eventHandlerGroup == null)
                eventHandlerGroup = new EventHandlerGroup<TEventHandler>();
            

            eventHandlerGroup.Add(eventHandler);

            return eventHandlerGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerGroup">if null, nothing happens</param>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static EventHandlerGroup<TEventHandler> operator -(
            [CanBeNull] EventHandlerGroup<TEventHandler> eventHandlerGroup, 
            [NotNull] TEventHandler eventHandler)
        {
            if (eventHandlerGroup == null)
                return null;

            eventHandlerGroup.Remove(eventHandler);

            return eventHandlerGroup;
        }

        

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<TEventHandler> GetEventHandlersSnapshot()
        {
            if (ReferenceEquals(_handlers, null))
                return null;

            TEventHandler[] handlersSnapshot = null;

            _handlers.GetCount(count =>
            {
                var pool = SnapshotPool<TEventHandler>.Singleton;
                // DefaultArraySize increased only if count is greated
                pool.DefaultArraySize = count;
                handlersSnapshot = pool.Get();

                _handlersSorted.CopyTo(handlersSnapshot,0);
            });

            return handlersSnapshot;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EventHandlerGroup<T> Define<T>() where T:class
        {
            return  new EventHandlerGroup<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Count
        {
            get
            {
                return
                    _handlers != null
                        ? _handlers.Count
                        : 0;
            }
        }


        protected override void InternalDispose(bool isExplicitDispose)
        {
			if (_handlers != null) 
            {
	            Clear();
	            _handlers = null;
			}
        }

        protected override void ReturnEventHandlersSnapshot(IEnumerable<TEventHandler> reusableSnapshot)
        {
            var reusableArray = reusableSnapshot as TEventHandler[];

            try
            {
                SnapshotPool<TEventHandler>.Singleton.Return(reusableArray);
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEventHandler"></typeparam>
    public sealed class TinyEventHandlerGroup<TEventHandler> : AEventHandlerGroup<TEventHandler>
        where TEventHandler : class
    {
#if DEBUG
        private const int MaxSize = 8;
#endif

        private volatile TEventHandler[] _handlers;
        private int _handlersCount = 0;

        private readonly object _syncRoot = new object();

        


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<TEventHandler> GetEventHandlersSnapshot()
        {
            
            // outside of lock because of volatile
            if (_handlers == null)
                    return null;

            var pool = SnapshotPool<TEventHandler>.Singleton;
            TEventHandler[] handlersSnaphot;

            lock (_syncRoot)
            {
                pool.DefaultArraySize = _handlersCount;
                handlersSnaphot = pool.Get();
                
                Array.Copy(_handlers, handlersSnaphot, _handlersCount);

            }

            return handlersSnaphot;

        }

        protected override void ReturnEventHandlersSnapshot(IEnumerable<TEventHandler> reusableSnapshot)
        {
            var reusableArray = reusableSnapshot as TEventHandler[];

            try
            {
                SnapshotPool<TEventHandler>.Singleton.Return(reusableArray);
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmpty
        {
            get
            {
                lock (_syncRoot)
                    return _handlersCount == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsNotEmpty
        {
            get
            {
                lock (_syncRoot)
                    return _handlersCount != 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        public override void Add(TEventHandler eventHandler)
        {
            Validator.CheckForNull(eventHandler,"eventHandler");

            if (null == _handlers)
                lock(_syncRoot)
                    if (null == _handlers)
                    {
                        var newHandlers = new TEventHandler[2];

                        newHandlers[0] = eventHandler;

                        _handlers = newHandlers;
                        _handlersCount = 1;
                        return;
                    }

            lock (_syncRoot)
            {

                for(int i=0;i<_handlersCount;i++)
                    if (ReferenceEquals(_handlers[i],eventHandler))
                        // silently return if the object/event handler is already registerred
                        return;

#if DEBUG
                if (_handlersCount >= MaxSize)
                    DebugHelper.TryBreak("TinyEventHandlerGroup Registering event handlers over maximum=" + MaxSize + " is suspicious");
#endif

                if (_handlersCount >= _handlers.Length)
                {
                    var newHandlers = new TEventHandler[_handlersCount*2];

                    Array.Copy(_handlers,newHandlers,_handlersCount);

                    _handlers = newHandlers;
                }


                _handlers[_handlersCount] = eventHandler;
                _handlersCount++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandler"></param>
        /// <returns></returns>
        public override bool Remove(TEventHandler eventHandler)
        {
            Validator.CheckForNull(eventHandler, "eventHandler");

            lock (_syncRoot)
            {
                if (_handlers == null)
                    return false;

                int indexToRemove = -1;

                for (int i = 0; i < _handlersCount; i++)
                    if (ReferenceEquals(_handlers[i], eventHandler))
                    {
                        indexToRemove = i;
                        break;
                    }

                if (indexToRemove < 0)
                    return false;

                for (int i = indexToRemove; i < _handlersCount - 1; i++)
                {
                    _handlers[i] = _handlers[i+1];
                }

                _handlers[_handlersCount - 1] = null;

                _handlersCount--;

                return true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            Clear(true);
        }

        private void Clear(bool synchronize)
        {
            if (synchronize)
                Monitor.Enter(_syncRoot);
            try
            {
                if (_handlers != null)
                {
                    Array.Clear(_handlers, 0, _handlers.Length);
                    _handlers = null;
                }
                _handlersCount = 0;
            }
            finally
            {
                if (synchronize)
                    Monitor.Exit(_syncRoot);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int Count
        {
            get
            {
                lock (_syncRoot)
                    return _handlersCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TinyEventHandlerGroup<T> Define<T>() where T : class
        {
            return new TinyEventHandlerGroup<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerGroup">if null, and used as += operator, the eventHandlerGroup would be created</param>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TinyEventHandlerGroup<TEventHandler> operator +(
            [CanBeNull] TinyEventHandlerGroup<TEventHandler> eventHandlerGroup,
            [NotNull] TEventHandler eventHandler)
        {
            if (eventHandlerGroup == null)
                eventHandlerGroup = new TinyEventHandlerGroup<TEventHandler>();


            eventHandlerGroup.Add(eventHandler);

            return eventHandlerGroup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventHandlerGroup">if null, nothing happens</param>
        /// <param name="eventHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TinyEventHandlerGroup<TEventHandler> operator -(
            [CanBeNull] TinyEventHandlerGroup<TEventHandler> eventHandlerGroup,
            [NotNull] TEventHandler eventHandler)
        {
            if (eventHandlerGroup == null)
                return null;

            eventHandlerGroup.Remove(eventHandler);

            return eventHandlerGroup;
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            try
            {
                Clear(isExplicitDispose);
            }
            catch
            {
                
            }
        }
    }
}
