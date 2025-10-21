using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(842)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class LprCamera : AOrmObjectWithVersion, IOrmObjectWithAlarmInstructions, IEquatable<LprCamera>
    {
        public const string COLUMNIDLPRCAMERA = "IdLprCamera";
        public const string COLUMNNAME = "Name";
        public const string COLUMNIPADDRESS = "IpAddress";
        public const string COLUMNPORT = "Port";
        public const string COLUMNSSLPORT = "PortSsl";
        public const string COLUMNMACADDRESS = "MacAddress";
        public const string COLUMNCOMMUNICATIONSCOPE = "CommunicationScope";
        public const string COLUMNLOCKED = "Locked";
        public const string COLUMNLOCKINGCLIENTIP = "LockingClientIp";
        public const string COLUMNISONLINE = "IsOnline";
        public const string COLUMNLASTHEARTBEATAT = "LastHeartbeatAt";
        public const string COLUMNLASTLICENSEPLATE = "LastLicensePlate";
        public const string COLUMNHEALTHSTATE = "HealthState";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNENABLEPARENTINFULLNAME = "EnableParentInFullName";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdLprCamera { get; set; }

        public virtual string Name { get; set; }

        [LwSerialize]
        public virtual string IpAddress { get; set; }

        [LwSerialize]
        public virtual string MacAddress { get; set; }

        [LwSerialize]
        public virtual string Port { get; set; }

        [LwSerialize]
        public virtual string PortSsl { get; set; }

        [LwSerialize]
        public virtual CommunicationScope CommunicationScope { get; set; }

        [LwSerialize]
        public virtual bool Locked { get; set; }

        [LwSerialize]
        public virtual string LockingClientIp { get; set; }

        [LwSerialize]
        public virtual bool IsOnline { get; set; }

        [LwSerialize]
        public virtual DateTime? LastHeartbeatAt { get; set; }

        [LwSerialize]
        public virtual string LastLicensePlate { get; set; }

        [LwSerialize]
        public virtual HealthState HealthState { get; set; }

        public virtual CCU CCU { get; set; }

        private Guid _guidCCU = Guid.Empty;

        [LwSerialize]
        public virtual Guid GuidCCU
        {
            get { return _guidCCU; }
            set { _guidCCU = value; }
        }

        public virtual string Description { get; set; }

        public virtual bool EnableParentInFullName { get; set; }

        public virtual Guid CkUnique { get; set; }

        public virtual string LocalAlarmInstruction { get; set; }

        public virtual byte ObjectType { get; set; }

        public LprCamera()
        {
            IpAddress = string.Empty;
            MacAddress = string.Empty;
            LockingClientIp = string.Empty;
            Port = string.Empty;
            PortSsl = string.Empty;
            CommunicationScope = CommunicationScope.CcuOnly;
            HealthState = HealthState.Ok;
            EnableParentInFullName = Support.EnableParentInFullName;
            CkUnique = Guid.NewGuid();
            ObjectType = (byte)Cgp.Globals.ObjectType.LprCamera;
        }

        public override bool Compare(object obj)
        {
            return Equals(obj as LprCamera);
        }

        public override string GetIdString()
        {
            return IdLprCamera.ToString();
        }

        public override object GetId()
        {
            return IdLprCamera;
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.LprCamera;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new LprCameraModifyObj(this);
        }

        public virtual void PrepareToSend()
        {
            GuidCCU = CCU != null ? CCU.IdCCU : Guid.Empty;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual bool Equals(LprCamera other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return other != null && !IdLprCamera.Equals(Guid.Empty) && IdLprCamera.Equals(other.IdLprCamera);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LprCamera);
        }

        public override int GetHashCode()
        {
            return IdLprCamera.Equals(Guid.Empty) ? base.GetHashCode() : IdLprCamera.GetHashCode();
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (EnableParentInFullName)
            {
                if (CCU != null)
                {
                    result += CCU + StringConstants.SLASHWITHSPACES;
                }
            }

            result += Name;
            return result;
        }

        public virtual bool IsLockedBy(string clientIp)
        {
            return Locked && !string.IsNullOrEmpty(clientIp) && string.Equals(LockingClientIp, clientIp, StringComparison.OrdinalIgnoreCase);
        }

        public virtual bool HasSecureChannel()
        {
            return !string.IsNullOrWhiteSpace(PortSsl);
        }

        public virtual bool HasValidNetworkConfiguration()
        {
            return !string.IsNullOrEmpty(IpAddress) && !string.IsNullOrWhiteSpace(Port);
        }
    }

    [LwSerialize(843)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    [Serializable]
    public class LprCameraShort : IShortObject
    {
        public const string COLUMN_ID = "IdLprCamera";
        public const string COLUMN_FULL_NAME = "FullName";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_ONLINE_STATE = "OnlineState";
        public const string COLUMN_STRING_ONLINE_STATE = "StringOnlineState";
        public const string COLUMN_LAST_LICENSE_PLATE = "LastLicensePlate";
        public const string COLUMN_LOCATION = "Location";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";
        public const string COLUMN_GUID_CCU = "GuidCCU";

        [LwSerialize]
        public Guid IdLprCamera { get; set; }
        [LwSerialize]
        public string FullName { get; set; }
        [LwSerialize]
        public string Name { get; set; }
        [LwSerialize]
        public OnlineState OnlineState { get; set; }
        [LwSerialize]
        public string StringOnlineState { get; set; }
        [LwSerialize]
        public string LastLicensePlate { get; set; }
        [LwSerialize]
        public string Location { get; set; }
        [LwSerialize]
        public Guid? GuidCCU { get; set; }
        [LwSerialize]
        public string Description { get; set; }
        [LwSerialize]
        public Image Symbol { get; set; }

        public ObjectType ObjectType
        {
            get { return ObjectType.LprCamera; }
        }

        public object Id
        {
            get { return IdLprCamera; }
        }

        public string GetSubTypeImageString(object value)
        {
            if (value is OnlineState)
            {
                try
                {
                    return (OnlineState)value == OnlineState.Online
                        ? ObjectType.LprCamera.ToString()
                        : ObjTypeHelper.CardReaderBlocked;
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        public static LprCameraShort FromLprCamera(LprCamera camera)
        {
            if (camera == null)
                return null;

            return new LprCameraShort
            {
                IdLprCamera = camera.IdLprCamera,
                Name = camera.Name,
                FullName = camera.ToString(),
                Description = camera.Description,
                GuidCCU = camera.GuidCCU,
                Location = camera.CCU != null ? camera.CCU.ToString() : null,
                LastLicensePlate = camera.LastLicensePlate,
                OnlineState = camera.IsOnline ? OnlineState.Online : OnlineState.Offline
            };
        }

        public static LprCameraShort FromUnknown(object value)
        {
            if (value == null)
                return null;

            if (value is LprCameraShort)
                return (LprCameraShort)value;

            var type = value.GetType();
            var result = new LprCameraShort();

            try
            {
                var property = type.GetProperty("IdLprCamera");
                if (property != null)
                    result.IdLprCamera = (Guid)property.GetValue(value, null);
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("Name");
                if (property != null)
                    result.Name = property.GetValue(value, null) as string;
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("FullName");
                if (property != null)
                    result.FullName = property.GetValue(value, null) as string;
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("OnlineState");
                if (property != null)
                {
                    var valueOnline = property.GetValue(value, null);
                    if (valueOnline is OnlineState)
                        result.OnlineState = (OnlineState)valueOnline;
                    else if (valueOnline is byte)
                        result.OnlineState = (OnlineState)(byte)valueOnline;
                }
            }
            catch
            {
            }

            if (result.OnlineState == OnlineState.Unknown)
            {
                try
                {
                    var property = type.GetProperty("IsOnline");
                    if (property != null)
                    {
                        var boolValue = property.GetValue(value, null) as bool?;
                        if (boolValue.HasValue)
                            result.OnlineState = boolValue.Value ? OnlineState.Online : OnlineState.Offline;
                    }
                }
                catch
                {
                }
            }

            try
            {
                var property = type.GetProperty("LastLicensePlate");
                if (property != null)
                    result.LastLicensePlate = property.GetValue(value, null) as string;
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("Location");
                if (property != null)
                    result.Location = property.GetValue(value, null) as string;
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("GuidCCU");
                if (property != null)
                {
                    var guid = property.GetValue(value, null);
                    if (guid is Guid)
                        result.GuidCCU = (Guid)guid;
                }
            }
            catch
            {
            }

            try
            {
                var property = type.GetProperty("Description");
                if (property != null)
                    result.Description = property.GetValue(value, null) as string;
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(result.FullName))
                result.FullName = result.Name;

            return result;
        }
    }

    [Serializable]
    public enum CommunicationScope : byte
    {
        CcuOnly = 0,
        ServerOnly = 1,
        ExternalNetwork = 2
    }

    [Serializable]
    public enum HealthState : byte
    {
        Ok = 0,
        Warning = 1,
        Error = 2
    }

    [Serializable]
    public class LprCameraModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.LprCamera; }
        }

        public LprCameraModifyObj(LprCamera camera)
        {
            Id = camera.IdLprCamera;
            FullName = camera.ToString();
            Description = camera.Description;
        }
    }

    [Serializable]
    public class LookupedLprCamera
    {
        public const string COLUMN_CHECKED = "IsChecked";
        public const string COLUMN_IP_ADDRESS = "IpAddress";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_PORT = "Port";
        public const string COLUMN_PORT_SSL = "PortSsl";
        public const string COLUMN_EQUIPMENT = "Equipment";
        public const string COLUMN_VERSION = "Version";
        public const string COLUMN_LOCKED = "Locked";
        public const string COLUMN_LOCKING_CLIENT_IP = "LockingClientIp";
        public const string COLUMN_MAC_ADDRESS = "MacAddress";
        public const string COLUMN_SERIAL = "Serial";
        public const string COLUMN_MODEL = "Model";
        public const string COLUMN_TYPE = "Type";
        public const string COLUMN_BUILD = "Build";
        public const string COLUMN_INTERFACE_SOURCE = "InterfaceSource";
        public const string COLUMN_UNIQUE_KEY = "UniqueKey";

        public LookupedLprCamera()
        {
            IsChecked = true;
        }

        public bool IsChecked { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }
        public string Port { get; set; }
        public string PortSsl { get; set; }
        public string Equipment { get; set; }
        public string Version { get; set; }
        public string Locked { get; set; }
        public string LockingClientIp { get; set; }
        public string MacAddress { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Build { get; set; }
        public string InterfaceSource { get; set; }
        public string UniqueKey { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Name) ? Name : IpAddress;
        }
    }

    public class LprCameraLookupFinishedHandler : ARemotingCallbackHandler
    {
        private static volatile LprCameraLookupFinishedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<ICollection<LookupedLprCamera>, ICollection<Guid>> _lookupFinished;

        public static LprCameraLookupFinishedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new LprCameraLookupFinishedHandler();
                    }

                return _singleton;
            }
        }

        private LprCameraLookupFinishedHandler()
            : base("LprCameraLookupFinishedHandler")
        {
        }

        public void RegisterLookupFinished(Action<ICollection<LookupedLprCamera>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished += lookupFinished;
        }

        public void UnregisterLookupFinished(Action<ICollection<LookupedLprCamera>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished -= lookupFinished;
        }

        public void RunEvent(
            ICollection<LookupedLprCamera> lookupedCameras,
            ICollection<Guid> lookupingClients)
        {
            _lookupFinished?.Invoke(lookupedCameras, lookupingClients);
        }
    }
}
