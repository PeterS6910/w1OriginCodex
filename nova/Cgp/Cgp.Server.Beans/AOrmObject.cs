using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public abstract class AOrmObject
    {
        public abstract bool Compare(object obj);

        public virtual bool Contains(string expression)
        {
            return ToString().ToLower().Contains(expression.ToLower());
        }

        public abstract string GetIdString();

        public virtual string GetDeviceShortName()
        {
            return string.Empty;
        }

        public virtual string GetSubTypeImageString(string subType)
        {
            return string.Empty;
        }

        public static string RemoveDiacritism(string Text)
        {
            string stringFormD = Text.Normalize(System.Text.NormalizationForm.FormD);
            var retVal = new System.Text.StringBuilder();
            for (int index = 0; index < stringFormD.Length; index++)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stringFormD[index]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    retVal.Append(stringFormD[index]);
            }
            return retVal.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
        /// <summary>
        /// Get object type in enum ObjectTypes
        /// </summary>
        /// <returns>Returns enum ObjectTypes</returns>
        public abstract ObjectType GetObjectType();

        public abstract object GetId();

        public abstract IModifyObject CreateModifyObject();
    }

    public class AOrmObjectComparer<TOrmObject> : IEqualityComparer<TOrmObject>
        where TOrmObject : AOrmObject
    {
        public bool Equals(TOrmObject x, TOrmObject y)
        {
            return x.GetId().Equals(y.GetId());
        }

        public int GetHashCode(TOrmObject obj)
        {
            return obj.GetId().GetHashCode();
        }
    }

    [Serializable]
    public abstract class AOrmObjectWithVersion :
        AOrmObject
    {
        public virtual int Version { get; set; }
    }

    public class DeletedObjectHandler : ARemotingCallbackHandler
    {
        private static volatile DeletedObjectHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<ObjectType, object> _deletedObject;

        public static DeletedObjectHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DeletedObjectHandler();
                    }
                return _singleton;
            }
        }

        public DeletedObjectHandler()
            : base("DeletedObjectHandler")
        {
        }

        public void Register(Action<ObjectType, object> deletedObject)
        {
            _deletedObject += deletedObject;
        }

        public void Unregister(Action<ObjectType, object> deletedObject)
        {
            _deletedObject -= deletedObject;
        }

        public void RunEvent(ObjectType objType, object objId)
        {
            if (_deletedObject != null)
                _deletedObject(objType, objId);
        }
    }

    [Serializable]
    public class TableTypeSettings
    {
        public ObjectType ObjectType { get; private set; }
        public string TypeName { get { return string.Format("{0}s", ObjectType.ToString()); } }
        public string PluginName { get; private set; }
        public bool CanCreate { get; private set; }

        public TableTypeSettings(ObjectType objectType, string pluginName, bool canCreate)
        {
            ObjectType = objectType;
            PluginName = pluginName;
            CanCreate = canCreate;
        }
    }

    [Serializable]
    public enum OnlineState : byte
    {
        Unknown = 0,
        Offline = 1,
        Online = 2,
        Upgrading = 3,
        WaitingForUpgrade = 4,
        AutoUpgrading = 5,
        Reseting = 6
    }

    [Serializable]
    public class AOrmObjectWithReferences
    {
        private readonly ObjectType _objectType;
        private readonly Guid _objectGuid;
        private List<AOrmObjectWithReferences> _references;

        public ObjectType ObjectType { get { return _objectType; } }
        public Guid ObjectGuid { get { return _objectGuid; } }
        public List<AOrmObjectWithReferences> References { get { return _references; } set { _references = value; } }

        public AOrmObjectWithReferences(ObjectType objectType, Guid objectGuid)
        {
            _objectType = objectType;
            _objectGuid = objectGuid;
            _references = new List<AOrmObjectWithReferences>();
        }
    }

    public class CUDObjectHandler : ARemotingCallbackHandler
    {
        private static volatile CUDObjectHandler _singleton;
        private static readonly object _syncRoot = new object();


        private readonly Dictionary<ObjectType, Action<ObjectType, object, bool>> _eventsCUDObject = 
            new Dictionary<ObjectType, Action<ObjectType, object, bool>>();

        public static CUDObjectHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CUDObjectHandler();
                    }

                return _singleton;
            }
        }

        public CUDObjectHandler()
            : base("CUDObjectHandler")
        {
        }

        public void Register(Action<ObjectType, object, bool> eventCUDObject, params ObjectType[] objectsType)
        {
            if (objectsType != null && objectsType.Length > 0)
            {
                foreach (ObjectType objectType in objectsType)
                {
                    if (_eventsCUDObject.ContainsKey(objectType))
                    {
                        _eventsCUDObject[objectType] += eventCUDObject;
                    }
                    else
                    {
                        _eventsCUDObject.Add(objectType, eventCUDObject);
                    }
                }
            }
        }

        public void Unregister(Action<ObjectType, object, bool> eventCUDObject, params ObjectType[] objectsType)
        {
            if (objectsType != null && objectsType.Length > 0)
            {
                foreach (ObjectType objectType in objectsType)
                {
                    if (_eventsCUDObject.ContainsKey(objectType))
                    {
                        _eventsCUDObject[objectType] -= eventCUDObject;
                    }
                }
            }
        }

        public void RunEvent(ObjectType objectType, object id, bool inserted)
        {
            Action<ObjectType, object, bool> eventCUDObject;
            if (_eventsCUDObject.TryGetValue(objectType, out eventCUDObject) && eventCUDObject != null)
            {
                eventCUDObject(objectType, id, inserted);
            }
        }
    }

    public class CUDObjectHandlerForServer
    {
        private static volatile CUDObjectHandlerForServer _singleton;
        private static readonly object _syncRoot = new object();


        private readonly Dictionary<ObjectType, Action<ObjectType, object, bool>> _envetsCUDObject =
            new Dictionary<ObjectType, Action<ObjectType, object, bool>>();

        public static CUDObjectHandlerForServer Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CUDObjectHandlerForServer();
                    }

                return _singleton;
            }
        }

        public void Register(Action<ObjectType, object, bool> eventCUDObject, params ObjectType[] objectsType)
        {
            if (objectsType != null && objectsType.Length > 0)
            {
                foreach (ObjectType objectType in objectsType)
                {
                    if (_envetsCUDObject.ContainsKey(objectType))
                    {
                        _envetsCUDObject[objectType] += eventCUDObject;
                    }
                    else
                    {
                        _envetsCUDObject.Add(objectType, eventCUDObject);
                    }
                }
            }
        }

        public void Unregister(Action<ObjectType, object, bool> eventCUDObject, params ObjectType[] objectsType)
        {
            if (objectsType != null && objectsType.Length > 0)
            {
                foreach (ObjectType objectType in objectsType)
                {
                    if (_envetsCUDObject.ContainsKey(objectType))
                    {
                        _envetsCUDObject[objectType] -= eventCUDObject;
                    }
                }
            }
        }

        public void RunEvent(ObjectType objectType, object id, bool inserted)
        {
            Action<ObjectType, object, bool> eventCUDObject;
            if (_envetsCUDObject.TryGetValue(objectType, out eventCUDObject) && eventCUDObject != null)
            {
                eventCUDObject(objectType, id, inserted);
            }
        }
    }
}
