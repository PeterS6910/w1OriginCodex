using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class TimetecSetting : AOrmObject
    {
        public virtual int IdTimetecSetting { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual int Port { get; set; }
        public virtual string LoginName { get; set; }
        public virtual string LoginPassword { get; set; }
        public virtual bool DoNotImportDepartments { get; set; }
        public virtual ICollection<CardReader> CardReaders { get; set; }
        public virtual DateTime? DefaultStartDateTime { get; set; }
        public virtual byte[] CertificateData { get; set; }

        public override bool Compare(object obj)
        {
            var timetecSetting = obj as TimetecSetting;

            return timetecSetting != null
                   && IdTimetecSetting.Equals(timetecSetting.IdTimetecSetting);
        }

        public override string GetIdString()
        {
            return IdTimetecSetting.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.TimetecSetting;
        }

        public override object GetId()
        {
            return IdTimetecSetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }

    public class TimetecCommunicationOnlineStateChangedHandler : IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile TimetecCommunicationOnlineStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<bool?> _stateChanged;

        public static TimetecCommunicationOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new TimetecCommunicationOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public TimetecCommunicationOnlineStateChangedHandler()
            : base("TimetecCommunicationOnlineStateChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<bool?> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<bool?> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(bool? isConnected)
        {
            if (_stateChanged != null)
                _stateChanged(isConnected);
        }
    }

    [Serializable]
    public class TimetecErrorEvent
    {
        public int Id { get; private set; }
        public Eventlog ErrorEventlog { get; private set; }
        public Eventlog SourceEventlog { get; private set; }

        public TimetecErrorEvent(
            int id,
            Eventlog errorEventlog,
            Eventlog sourceEventlog)
        {
            Id = id;
            ErrorEventlog = errorEventlog;
            SourceEventlog = sourceEventlog;
        }
    }
}
