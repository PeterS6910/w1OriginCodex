using System;
using System.Collections.Generic;
using System.Net;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(330)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AlarmTransmitter : AOrmObjectWithVersion, IOrmObjectWithAlarmInstructions
    {
        public const string COLUMN_ID_ALARM_TRANSMITTER = "IdAlarmTransmitter";
        public const string COLUMN_OBJECT_TYPE = "ObjectType";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IP_ADDRESS = "IpAddress";
        public const string COLUMN_CCUS = "Ccus";
        public const string COLUMN_LOCAL_ALARM_INSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdAlarmTransmitter { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual string Name { get; set; }
        [LwSerialize]
        public virtual string IpAddress { get; set; }
        
        public virtual ICollection<CCU> Ccus { get; set; }

        public virtual string LocalAlarmInstruction { get; set; }
        public virtual string Description { get; set; }

        public AlarmTransmitter()
        {
            ObjectType = (byte) Cgp.Globals.ObjectType.AlarmTransmitter;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            var alarmTerminal = obj as AlarmTransmitter;

            return alarmTerminal != null
                   && alarmTerminal.IdAlarmTransmitter.Equals(IdAlarmTransmitter);
        }

        public override string GetIdString()
        {
            return IdAlarmTransmitter.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.AlarmTransmitter;
        }

        public override object GetId()
        {
            return IdAlarmTransmitter;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new AlarmTransmitterModifyObj(this);
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class AlarmTransmittersLookupFinishedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmTransmittersLookupFinishedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<ICollection<LookupedAlarmTransmitter>, ICollection<Guid>> _lookupFinished;

        public static AlarmTransmittersLookupFinishedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmTransmittersLookupFinishedHandler();
                    }

                return _singleton;
            }
        }

        public AlarmTransmittersLookupFinishedHandler()
            : base("AlarmTransmittersLookupFinishedHandler")
        {
        }

        public void RegisterLookupFinished(Action<ICollection<LookupedAlarmTransmitter>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished += lookupFinished;
        }

        public void UnregisterLookupFinished(Action<ICollection<LookupedAlarmTransmitter>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished -= lookupFinished;
        }

        public void RunEvent(ICollection<LookupedAlarmTransmitter> lookupedAlarmTransmitters, ICollection<Guid> lookupingClients)
        {
            if (_lookupFinished != null)
                _lookupFinished(
                    lookupedAlarmTransmitters,
                    lookupingClients);
        }
    }

    public class AlarmTransmitterOnlineStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmTransmitterOnlineStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<string, OnlineState> _onlineStateChanged;

        public static AlarmTransmitterOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new AlarmTransmitterOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public AlarmTransmitterOnlineStateChangedHandler()
            : base("AlarmTransmitterOnlineStateChangedHandler")
        {
        }

        public void RegisterOnlineStateChanged(Action<string, OnlineState> onlineStateChanged)
        {
            _onlineStateChanged += onlineStateChanged;
        }

        public void UnregisterOnlineStateChanged(Action<string, OnlineState> onlineStateChanged)
        {
            _onlineStateChanged -= onlineStateChanged;
        }

        public void RunEvent(string ipAddress, OnlineState state)
        {
            if (_onlineStateChanged != null)
                _onlineStateChanged(
                    ipAddress,
                    state);
        }
    }
}
