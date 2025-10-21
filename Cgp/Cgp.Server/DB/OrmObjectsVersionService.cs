using System;
using System.Linq;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using JetBrains.Annotations;
using Contal.Cgp.ORM;

namespace Contal.Cgp.Server.DB
{
    public abstract class OrmObjectVersionChangedListener
        : IEquatable<OrmObjectVersionChangedListener>
    {
        private static int LastUniqueId = 0;
        private static readonly object LockGetUniqueId = new object();

        private int _uniqueId;

        public OrmObjectVersionChangedListener()
        {
            lock (LockGetUniqueId)
                _uniqueId = LastUniqueId++;
        }

        public bool Equals(OrmObjectVersionChangedListener other)
        {
            if (other == null)
                return false;

            return other._uniqueId == _uniqueId;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as OrmObjectVersionChangedListener);
        }

        public override int GetHashCode()
        {
            return _uniqueId;
        }

        public abstract void OnVersionChanged(int newVersion);
    }

    public sealed class OrmObjectsVersionService
        : ASingleton<OrmObjectsVersionService>
    {
        private sealed class MaximumVersionForObjectTypes : ATableORM<MaximumVersionForObjectTypes>
        {
            private readonly object _getAndUpdateLock = new object();

            private MaximumVersionForObjectTypes() : base(null)
            {
                NhHelper.Singleton.ConnectionString =
                    ConnectionString.LoadFromRegistry(CgpServerGlobals.REGISTRY_CONNECTION_STRING);

                ThisAssembly = typeof(Cgp.Server.Beans.SystemVersion).Assembly;
            }

            public int IncrementAndGet(ObjectType objectType)
            {
                lock (_getAndUpdateLock)
                {
                    var maximumVersions = SelectLinq<MaximumVersionForObjectType>(
                        maximumVersionForObjectType => maximumVersionForObjectType.ObjectType == (int)objectType);

                    var maximumVersion = maximumVersions != null
                        ? maximumVersions.FirstOrDefault()
                        : null;

                    if (maximumVersion == null)
                    {
                        maximumVersion = new MaximumVersionForObjectType()
                        {
                            ObjectType = (int)objectType,
                            Version = 0
                        };

                        Insert(maximumVersion, null);
                    }
                    else
                    {
                        ++maximumVersion.Version;
                        Update(maximumVersion);
                    }

                    return maximumVersion.Version;
                }
            }
        }

        private SyncDictionary<IdAndObjectType, EventHandlerGroup<OrmObjectVersionChangedListener>> _handlerGroups;

        private OrmObjectsVersionService()
            : base(null)
        {
            _handlerGroups = new SyncDictionary<IdAndObjectType, EventHandlerGroup<OrmObjectVersionChangedListener>>();
        }

        public void IncrementVersions([NotNull] IEnumerable<AOrmObjectWithVersion> ormObjects)
        {
            var maximumVersionsForObjectType = new Dictionary<ObjectType, int>();

            foreach (var ormObject in ormObjects)
            {
                var objectType = ormObject.GetObjectType();
                int version;

                if (!maximumVersionsForObjectType.TryGetValue(
                    objectType, 
                    out version))
                {
                    version = MaximumVersionForObjectTypes.Singleton.IncrementAndGet(objectType);

                    maximumVersionsForObjectType.Add(
                        objectType, 
                        version);
                }

                ormObject.Version = version;

                var idAndObjectType = new IdAndObjectType(
                    ormObject.GetId(),
                    ormObject.GetObjectType());

                EventHandlerGroup<OrmObjectVersionChangedListener> handlerGroup;

                if (_handlerGroups.TryGetValue(
                    idAndObjectType,
                    out handlerGroup))
                {
                    handlerGroup.ForEach(
                        listener =>
                            listener.OnVersionChanged(version));
                }
            }
        }

        public void AddListener(
            IdAndObjectType idAndObjectType,
            OrmObjectVersionChangedListener listener)
        {
            EventHandlerGroup<OrmObjectVersionChangedListener> handlerGroup;

            if (!_handlerGroups.TryGetValue(idAndObjectType, out handlerGroup))
            {
                handlerGroup = new EventHandlerGroup<OrmObjectVersionChangedListener>();
                _handlerGroups.Add(idAndObjectType, handlerGroup);
            }

            handlerGroup.Add(listener);
        }

        public void RemoveListener(
            IdAndObjectType idAndObjectType,
            OrmObjectVersionChangedListener listener)
        {
            EventHandlerGroup<OrmObjectVersionChangedListener> handlerGroup;

            if (!_handlerGroups.TryGetValue(idAndObjectType, out handlerGroup))
                return;

            handlerGroup.Remove(listener);

            if (handlerGroup.IsEmpty)
                _handlerGroups.Remove(idAndObjectType);
        }
    }
}
